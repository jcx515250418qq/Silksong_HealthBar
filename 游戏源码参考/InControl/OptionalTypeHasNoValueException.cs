using System;

namespace InControl
{
	public class OptionalTypeHasNoValueException : SystemException
	{
		public OptionalTypeHasNoValueException(string message)
			: base(message)
		{
		}
	}
}
