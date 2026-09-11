using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Fox.Docx.Errors;

namespace Fox.Docx.Processors
{
	internal class ListProcessor:IProcessor
	{
		private bool _isNeedToRemoveContentControls;
		private readonly ProcessContext _context;
	

		private class PropagationProcessResult : ProcessResult
		{
			internal IEnumerable<XElement> Result { get; set; }
		}
		
		private class Prototype
		{
			private readonly ProcessContext _context;
			
			private Prototype(ProcessContext context, IEnumerable<XElement> prototypeItems)
			{
				_context = context;
				PrototypeItems = prototypeItems.ToList();
			}

			public Prototype(ProcessContext context, XElement listContentControl, IEnumerable<string> fieldNames)
			{
				_context = context;
				if (listContentControl.Name != W.sdt)
					throw new Exception("List content control is not a content control element");

				fieldNames = fieldNames.ToList();

				
				var listItems = listContentControl
					.Element(W.sdtContent)
					.Elements()
					.ToList();

				var tagsInPrototype = listItems.DescendantsAndSelf(W.sdt)
					.Select(sdt => sdt.SdtTagName());

			
				if (fieldNames.Any(fn => !tagsInPrototype.Contains(fn)))
				{
					IsValid = false;
					return;
				}

				IsValid = true;
				PrototypeItems = listItems;
			}

			public bool IsValid { get; private set; }
			public List<XElement> PrototypeItems { get; private set; }

			public Prototype Exclude(LevelPrototype prototypeForExclude)
			{
				return new Prototype(_context, PrototypeItems
					.Where(itemPrototype => !prototypeForExclude
						.PrototypeItems
						.Contains(itemPrototype)));
			}

			
			public LevelPrototype CurrentLevelPrototype(IEnumerable<string> fieldNames)
			{
				return new LevelPrototype(_context, PrototypeItems, fieldNames);
			}
		}

		
		private class LevelPrototype
		{
			private readonly ProcessContext _context;
			public bool IsValid { get; private set; }
			public LevelPrototype(ProcessContext context, IEnumerable<XElement> prototypeItems, IEnumerable<string> fieldNames)
			{
				_context = context;
				var currentLevelPrototype = new List<XElement>();

			
				var maybeNeedToAdd = new List<XElement>();
				var numberingElementReached = false;

				foreach (var prototypeItem in prototypeItems)
				{
					
					if (!numberingElementReached)
					{
						var paragraph = prototypeItem.DescendantsAndSelf(W.p).FirstOrDefault();
						if (paragraph != null &&
							ListItemRetriever.RetrieveListItem(
							context.Document.NumberingPart, context.Document.StylesPart, paragraph)
							.IsListItem)
							numberingElementReached = true;
						else
							continue;
					}
					if ((!prototypeItem.FirstLevelDescendantsAndSelf(W.sdt).Any() && prototypeItem.Value != "") ||
						(prototypeItem
						.FirstLevelDescendantsAndSelf(W.sdt)
						.Any(sdt => fieldNames.Contains(sdt.SdtTagName()))))
					{
						currentLevelPrototype.AddRange(maybeNeedToAdd);
						currentLevelPrototype.Add(prototypeItem);
					}

					else
					{
						maybeNeedToAdd.Add(prototypeItem);
					}
				}
				if (!currentLevelPrototype.Any()) return;

				PrototypeItems = currentLevelPrototype;

				if (fieldNames.Any(fn => !SdtTags.Contains(fn)))
				{
					IsValid = false;
					return;
				}

				IsValid = true;
				PrototypeItems = currentLevelPrototype;

			}

			private LevelPrototype(ProcessContext context, IEnumerable<XElement> prototypeItems)
			{
				_context = context;
				PrototypeItems = prototypeItems.ToList();
			}
			public List<XElement> PrototypeItems { get; private set; }

			private List<string> SdtTags
			{
				get
				{
					return PrototypeItems == null
						? new List<string>() 
						: PrototypeItems.DescendantsAndSelf(W.sdt)
						.Select(sdt => sdt.SdtTagName())
						.ToList();
				}
			}

			public LevelPrototype Clone()
			{
				return new LevelPrototype(_context, PrototypeItems.ToList());
			}
		}

		public ListProcessor(ProcessContext context)
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
				processResult.Merge(itemProcessResult);

				if (!itemProcessResult.Handled) continue;

				handled = true;
			}

			if (!handled) return processResult;

			if (processResult.Success && _isNeedToRemoveContentControls)
			{
				foreach (var sdt in contentControl.Descendants(W.sdt).ToList())
				{
				
					sdt.RemoveContentControl();
				}
				contentControl.RemoveContentControl();
			}
			return processResult;
		}

		private ProcessResult FillContent(XElement contentControl, IContentItem item)
		{
			var processResult = ProcessResult.NotHandledResult; 
			if (!(item is ListContent))
			{
				return ProcessResult.NotHandledResult;
			}

			var list = item as ListContent;

			
			if (contentControl == null)
			{
				processResult.AddError(new ContentControlNotFoundError(list));

				return processResult;
			}

	
			var itemsContentControl = contentControl
				.Descendants(W.sdt)
				.FirstOrDefault();

			if (itemsContentControl == null)
			{
				processResult.AddError(
					new CustomContentItemError(list, "doesn't contain content controls in items"));
			
				return processResult;
			}

			var fieldNames = list.FieldNames.ToList();

		
			var prototype = new Prototype(_context, contentControl, fieldNames);

			if (!prototype.IsValid)
			{
				processResult.AddError(
					new CustomContentItemError(list, 
						""));

				return processResult;
			}

			new NumberingAccessor(_context.Document.NumberingPart, _context.LastNumIds)
					.ResetNumbering(prototype.PrototypeItems);

			
			var propagationResult = PropagatePrototype(prototype, list.Items);

			processResult.Merge(propagationResult);
			
		
			prototype.PrototypeItems.Last().AddAfterSelf(propagationResult.Result);
			prototype.PrototypeItems.Remove();

			processResult.AddItemToHandled(list);
			
			return processResult;
		}

		
		private PropagationProcessResult PropagatePrototype(Prototype prototype, 
			IEnumerable<ListItemContent> content)
		{
			var processResult = new PropagationProcessResult();
			var newRows = new List<XElement>();

			foreach (var contentItem in content)
			{
				var currentLevelPrototype = prototype.CurrentLevelPrototype(contentItem.FieldNames);
				
				if (currentLevelPrototype == null || !currentLevelPrototype.IsValid)
				{
					processResult.AddError(new CustomError(
						string.Format("Prototype for list item '{0}' not found", 
							string.Join(", ", contentItem.FieldNames))));

					continue;
				}
				
		
				var newItemEntry = currentLevelPrototype.Clone();

				foreach (var xElement in newItemEntry.PrototypeItems)
				{
					var newElement = new XElement(xElement);
					if (!newElement.DescendantsAndSelf(W.sdt).Any())
					{
						newRows.Add(newElement);
						continue;
					}

					foreach (var sdt in newElement.FirstLevelDescendantsAndSelf(W.sdt).ToList())
					{
						var fieldContent = contentItem.GetContentItem(sdt.SdtTagName());
						if (fieldContent == null)
						{
							processResult.AddError(new CustomError(
								string.Format("Field content for field '{0}' not found", 
								sdt.SdtTagName())));

							continue;
						}
						
						var contentProcessResult = new ContentProcessor(_context)
							.SetRemoveContentControls(_isNeedToRemoveContentControls)
							.FillContent(sdt, fieldContent);

						processResult.Merge(contentProcessResult);
						
						
					}
					newRows.Add(newElement);
					
				}
				
		
				if (contentItem.NestedFields != null)
				{
					var filledNestedFields = PropagatePrototype(
						prototype.Exclude(currentLevelPrototype), 
						contentItem.NestedFields);

					newRows.AddRange(filledNestedFields.Result);					
				}		
			}
			processResult.Result = newRows;
			return processResult;
		}
	}
}
