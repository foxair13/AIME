using System;

namespace Fox.Docx.Errors
{
	internal interface IError:IEquatable<IError>
	{
		string Message { get; }
	}
}
