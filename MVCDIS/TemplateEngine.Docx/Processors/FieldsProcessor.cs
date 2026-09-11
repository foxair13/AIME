using System.Collections.Generic;
using System.Xml.Linq;
using Fox.Docx.Errors;

namespace Fox.Docx.Processors
{
	internal class FieldsProcessor:IProcessor
	{
		private bool _isNeedToRemoveContentControls;

		public IProcessor SetRemoveContentControls(bool isNeedToRemove)
		{
			_isNeedToRemoveContentControls = isNeedToRemove;
			return this;
		}

		public ProcessResult FillContent(XElement contentControl, IEnumerable<IContentItem> items)
		{
			var processResult = ProcessResult.NotHandledResult;

			foreach (var contentItem in items)
			{
				processResult.Merge(FillContent(contentControl, contentItem));
			}

			if (processResult.Success && _isNeedToRemoveContentControls)
				contentControl.RemoveContentControl();

			return processResult;
		}

		public ProcessResult FillContent(XElement contentControl, IContentItem item)
		{
			var processResult = ProcessResult.NotHandledResult; 
			if (!(item is FieldContent))
			{
				processResult = ProcessResult.NotHandledResult;
				return processResult;
			}

			var field = item as FieldContent;

			if (contentControl == null)
			{
				processResult.AddError(new ContentControlNotFoundError(field));
				return processResult;
			}
			contentControl.ReplaceContentControlWithNewValue(field.Value);

			processResult.AddItemToHandled(item);

			return processResult;
		}
	}
}
