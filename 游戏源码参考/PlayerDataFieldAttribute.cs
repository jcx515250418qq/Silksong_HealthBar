using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class PlayerDataFieldAttribute : PropertyAttribute
{
	public Type FieldType { get; private set; }

	public bool IsRequired { get; private set; }

	public PlayerDataFieldAttribute(Type fieldType, bool isRequired = true)
	{
		FieldType = fieldType;
		IsRequired = isRequired;
	}
}
