using System;
using System.Collections.Generic;
using System.Reflection;

public sealed class FieldCache
{
	private sealed class Cache
	{
		public bool isValid;

		public FieldInfo fieldInfo;

		public Cache(FieldInfo fieldInfo)
		{
			this.fieldInfo = fieldInfo;
			isValid = fieldInfo != null;
		}
	}

	private readonly Type type;

	private bool useBindingFlags;

	private BindingFlags bindingFlags;

	private readonly Dictionary<string, Cache> fieldInfos = new Dictionary<string, Cache>();

	public FieldCache(Type type)
	{
		this.type = type;
	}

	public FieldCache(Type type, bool useBindingFlags, BindingFlags bindingFlags)
	{
		this.type = type;
		this.useBindingFlags = useBindingFlags;
		this.bindingFlags = bindingFlags;
	}

	private bool TryGetField(string variableName, out Cache fieldCache)
	{
		if (!fieldInfos.TryGetValue(variableName, out fieldCache))
		{
			if (useBindingFlags)
			{
				fieldCache = new Cache(type.GetField(variableName, bindingFlags));
			}
			else
			{
				fieldCache = new Cache(type.GetField(variableName));
			}
			fieldInfos.Add(variableName, fieldCache);
		}
		return true;
	}

	public T GetValue<T>(string variableName)
	{
		if (TryGetField(variableName, out var fieldCache) && fieldCache.isValid)
		{
			FieldInfo fieldInfo = fieldCache.fieldInfo;
			if (fieldInfo.FieldType == typeof(T))
			{
				return (T)fieldInfo.GetRawConstantValue();
			}
		}
		return default(T);
	}
}
