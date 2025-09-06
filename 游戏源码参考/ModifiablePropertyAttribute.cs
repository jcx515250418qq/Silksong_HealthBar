using System;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class ModifiablePropertyAttribute : PropertyAttribute
{
	public List<PropertyModifierAttribute> modifiers;
}
