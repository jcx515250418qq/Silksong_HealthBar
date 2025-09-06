using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class ArrayForEnumAttribute : PropertyAttribute
{
	public readonly Type EnumType;

	public readonly int EnumLength;

	public bool IsValid
	{
		get
		{
			if (EnumType != null && EnumType.IsEnum)
			{
				return EnumLength > 0;
			}
			return false;
		}
	}

	public ArrayForEnumAttribute(Type enumType)
	{
		EnumType = enumType;
		if (enumType != null && enumType.IsEnum)
		{
			EnumLength = GetArrayLength(enumType);
		}
		else
		{
			EnumLength = 0;
		}
	}

	public static void EnsureArraySize<T>(ref T[] array, Type enumType)
	{
		int arrayLength = GetArrayLength(enumType);
		if (array != null)
		{
			if (array.Length != arrayLength)
			{
				T[] array2 = array;
				array = new T[arrayLength];
				for (int i = 0; i < Mathf.Min(arrayLength, array2.Length); i++)
				{
					array[i] = array2[i];
				}
			}
		}
		else
		{
			array = new T[arrayLength];
		}
	}

	public static int GetArrayLength(Type enumType)
	{
		int num = 0;
		Array values = Enum.GetValues(enumType);
		for (int i = 0; i < values.Length; i++)
		{
			int num2 = (int)values.GetValue(i);
			if (num2 >= 0)
			{
				int num3 = num2 + 1;
				if (num < num3)
				{
					num = num3;
				}
			}
		}
		return num;
	}
}
