using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class NamedArrayAttribute : PropertyAttribute
{
	public string MethodName { get; }

	public NamedArrayAttribute(string methodName)
	{
		MethodName = methodName;
	}
}
