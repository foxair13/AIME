using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fox.Docx.Errors;

namespace Fox.Docx.Processors
{
	internal class TableProcessor : IProcessor
	{
		private bool _isNeedToRemoveContentControls;
		private readonly ProcessContext _context;

		public TableProcessor(ProcessContext context)
		{
			_context = context;
		}
		public IProcessor SetRemoveContentControls(bool isNeedToRemove)
		{
			_isNeedToRemoveContentControls = isNeedToRemove;
			return this;
		}
		
		public ProcessResult FillContent(XElement contentControl, IEnumerable<IContentItem> items)
		{
			var processResult = ProcessResult.NotHandledResult; 
			var handled = false;
			
			foreach (var contentItem in items)
			{
				var itemProcessResult = FillContent(contentControl, contentItem);
				if (!itemProcessResult.Handled) continue;

				handled = true;

				processResult.Merge(itemProcessResult);
			}

			if (!handled) return ProcessResult.NotHandledResult;

			if (processResult.Success && _isNeedToRemoveContentControls)
			{
				
				foreach (var xElement in contentControl.AncestorsAndSelf(W.sdt))
				{
					xElement.RemoveContentControl();
				}
			}

			return processResult;
		}

	
		private ProcessResult FillContent(XContainer contentControl, IContentItem item)
		{
			if (!(item is TableContent))
				return ProcessResult.NotHandledResult;

			var processResult = ProcessResult.NotHandledResult; 

			var table = item as TableContent;

		
			if (contentControl == null)
			{
				processResult.AddError(new ContentControlNotFoundError(table));

				return processResult;
			}

			
			var cellContentControl = contentControl
				.Descendants(W.sdt)
				.FirstOrDefault();
			if (cellContentControl == null)
			{
				processResult.AddError(new CustomContentItemError(table,
					string.Format("doesn't contain content controls in cells")));

				return processResult;
			}

			var fieldNames = table.FieldNames.ToList();

			var prototypeRows = GetPrototype(contentControl, fieldNames);

			
			var contentControlTagNames = prototypeRows
				.Descendants(W.sdt)
				.Select(sdt => sdt.SdtTagName())
				.Where(fieldNames.Contains)
				.ToList();


	
			if (contentControlTagNames.Intersect(fieldNames).Count() != fieldNames.Count())
			{
				var invalidFileNames = fieldNames
					.Where(fn => !contentControlTagNames.Contains(fn))
					.ToList();

				processResult.AddError(
					new CustomContentItemError(table, 
					String.Format("doesn't contain rows with cell content {0} {1}",
						invalidFileNames.Count > 1 ? "controls" : "control",
						string.Join(", ", invalidFileNames.Select(fn => string.Format("'{0}'", fn))))));

			}


		
			var newRows = new List<List<XElement>>();
			foreach (var row in table.Rows)
			{
				
				var newRowsEntry = prototypeRows.Select(prototypeRow => new XElement(prototypeRow)).ToList();

				
				foreach (var sdt in newRowsEntry.FirstLevelDescendantsAndSelf(W.sdt).ToList())
				{
				
					var fieldName = sdt.SdtTagName();

					var content = row.GetContentItem(fieldName);

					if (content != null)
					{
						var contentProcessResult = new ContentProcessor(_context)
							.SetRemoveContentControls(_isNeedToRemoveContentControls)
							.FillContent(sdt, content);

						processResult.Merge(contentProcessResult);
					}
				}

			
				newRows.Add(newRowsEntry);
			}

			prototypeRows.Last().AddAfterSelf(newRows);

		
			prototypeRows.Remove();

			processResult.AddItemToHandled(table);

			return processResult;
		}

		
		private List<XElement> GetPrototype(XContainer tableContentControl, IEnumerable<string> fieldNames)
		{
			var rowsWithContentControl = tableContentControl
				.Descendants(W.tr)
				.Where(tr =>
					tr.Descendants(W.sdt)
						.Any(sdt =>
							fieldNames.Contains(
								sdt.SdtTagName())))
				.ToList();


			return GetIntermediateAndMergedRows(rowsWithContentControl.First(), rowsWithContentControl.Last(),
				tableContentControl);
		}

		private List<XElement> GetIntermediateAndMergedRows(XElement firstRow, XElement lastRow, XContainer tableContentControl)
		{
			var resultRows = new List<XElement>();

			var mergeVector = new bool[lastRow.Descendants(W.tc).Count()];

			var firstRowReached = false;
			var lastRowReached = false;

		
			foreach (var tableRow in tableContentControl.Descendants(W.tr))
			{
				if (tableRow == firstRow)
				{
					resultRows.Add(tableRow);
					firstRowReached = true;
				}
				if (!firstRowReached) continue;

				if (!lastRowReached)
				{
					if (tableRow == lastRow)
					{
						if (firstRow != lastRow)
							resultRows.Add(tableRow);

						var lastRowCells = lastRow.Descendants(W.tc).ToArray();
						for (var i = 0; i < lastRowCells.Count(); i++)
						{
							var cell = lastRowCells[i];
							var cellFormatting = cell.Element(W.tcPr);
							if (cellFormatting != null && cellFormatting.Element(W.vMerge) != null)
							{
								mergeVector[i] = true;
							}
						}
						lastRowReached = true;
						continue;
					}

					if (tableRow != firstRow)
						resultRows.Add(tableRow);
				}

			
				if (mergeVector.Any(r => r))
				{
					var rowCells = tableRow.Descendants(W.tc).ToArray();
					for (var i = 0; i < rowCells.Count(); i++)
					{
						var cell = rowCells[i];
						var cellFormatting = cell.Element(W.tcPr);
						if (cellFormatting != null && cellFormatting.Element(W.vMerge) != null &&
							(cellFormatting.Element(W.vMerge).Attribute(W.val) == null ||
							 cellFormatting.Element(W.vMerge).Attribute(W.val).Value == "continue"))
						{
							resultRows.Add(tableRow);
							mergeVector[i] = true;
						}
						else
						{
							mergeVector[i] = false;
						}
					}
				}
				else if (lastRowReached)
					break;
			}


			return resultRows;
		}
	}
}
