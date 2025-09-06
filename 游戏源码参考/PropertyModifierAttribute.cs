using System;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
public abstract class PropertyModifierAttribute : Attribute
{
	public int order { get; set; }
}
