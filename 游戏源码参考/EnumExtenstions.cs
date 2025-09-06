using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class EnumExtenstions
{
	public static IEnumerable<string> GetNamesWithOrder(this Enum enumVal)
	{
		return enumVal.GetType().GetNamesWithOrder();
	}

	public static IEnumerable<string> GetNamesWithOrder(this Type type)
	{
		if (!type.IsEnum)
		{
			throw new ArgumentException("Type must be an enum");
		}
		return from field in type.GetFields()
			where field.IsStatic
			select new
			{
				field = field,
				attribute = field.GetCustomAttributes(inherit: false).OfType<EnumOrderAttribute>().FirstOrDefault()
			} into fieldInfo
			select new
			{
				name = fieldInfo.field.Name,
				order = ((fieldInfo.attribute != null) ? fieldInfo.attribute.Order : 0)
			} into field
			orderby field.order
			select field.name;
	}

	public static IEnumerable<int> GetValuesWithOrder(this Enum enumVal)
	{
		return enumVal.GetType().GetValuesWithOrder();
	}

	public static IEnumerable<int> GetValuesWithOrder(this Type type)
	{
		if (!type.IsEnum)
		{
			throw new ArgumentException("Type must be an enum");
		}
		return from field in type.GetFields()
			where field.IsStatic
			select new
			{
				field = field,
				attribute = field.GetCustomAttributes(inherit: false).OfType<EnumOrderAttribute>().FirstOrDefault()
			} into fieldInfo
			select new
			{
				value = (int)fieldInfo.field.GetRawConstantValue(),
				order = ((fieldInfo.attribute != null) ? fieldInfo.attribute.Order : 0)
			} into field
			orderby field.order
			select field.value;
	}
}
