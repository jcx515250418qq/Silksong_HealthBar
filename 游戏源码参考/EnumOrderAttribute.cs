using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class EnumOrderAttribute : Attribute
{
	public int Order { get; private set; }

	public EnumOrderAttribute(int order)
	{
		Order = order;
	}
}
