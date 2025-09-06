using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class InspectorValidationAttribute : PropertyModifierAttribute
{
	private Color initialColor;

	public string MethodName { get; private set; }

	public InspectorValidationAttribute(string methodName)
	{
		MethodName = methodName;
	}

	public InspectorValidationAttribute()
	{
		MethodName = null;
	}
}
