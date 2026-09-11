using System;

namespace Fox.Docx
{
	public interface IContentItem : IEquatable<IContentItem>
	{
		string Name { get; set; }
	}
}
