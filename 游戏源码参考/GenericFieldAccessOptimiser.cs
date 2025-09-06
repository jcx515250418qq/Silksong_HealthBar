using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

public sealed class GenericFieldAccessOptimiser
{
	private sealed class FieldAccessInfo<T>
	{
		public volatile bool IsCompiled;

		public FieldInfo fieldInfo;

		public Action<object, T> Setter { get; private set; }

		public Func<object, T> Getter { get; private set; }

		public FieldAccessInfo(Type targetType, string fieldName)
		{
			FieldAccessInfo<T> fieldAccessInfo = this;
			try
			{
				fieldInfo = targetType.GetField(fieldName);
				if (!(fieldInfo == null) && !(fieldInfo.FieldType != typeof(T)))
				{
					Task.Run(delegate
					{
						fieldAccessInfo.CompileAndCacheDelegate(targetType);
					});
				}
			}
			catch (Exception)
			{
			}
		}

		public FieldAccessInfo(object target, string fieldName)
		{
			FieldAccessInfo<T> fieldAccessInfo = this;
			try
			{
				Type targetType = target.GetType();
				fieldInfo = targetType.GetField(fieldName);
				if (!(fieldInfo == null) && !(fieldInfo.FieldType != typeof(T)))
				{
					Task.Run(delegate
					{
						fieldAccessInfo.CompileAndCacheDelegate(targetType);
					});
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}

		private void CompileAndCacheDelegate(Type targetType)
		{
			try
			{
				ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "target");
				ParameterExpression parameterExpression2 = Expression.Parameter(typeof(T), "value");
				MemberExpression memberExpression = Expression.Field(Expression.Convert(parameterExpression, targetType), fieldInfo);
				Setter = Expression.Lambda<Action<object, T>>(Expression.Assign(memberExpression, parameterExpression2), new ParameterExpression[2] { parameterExpression, parameterExpression2 }).Compile();
				Getter = Expression.Lambda<Func<object, T>>(Expression.Convert(memberExpression, typeof(T)), new ParameterExpression[1] { parameterExpression }).Compile();
				IsCompiled = true;
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}
	}

	private readonly Dictionary<string, object> accessCache = new Dictionary<string, object>();

	public void SetField<T>(object target, string fieldName, T value)
	{
		if (!EnsureCompiled(target, fieldName, out FieldAccessInfo<T> info))
		{
			SetFieldUsingReflection(target, info?.fieldInfo, value);
			return;
		}
		if (Platform.UseFieldInfoCache)
		{
			FieldInfo fieldInfo = info.fieldInfo;
			if (fieldInfo == null)
			{
				return;
			}
			try
			{
				fieldInfo.SetValue(target, value);
				return;
			}
			catch (Exception)
			{
				return;
			}
		}
		try
		{
			info.Setter(target, value);
		}
		catch (Exception)
		{
		}
	}

	public T GetField<T>(object target, string fieldName)
	{
		if (!EnsureCompiled(target, fieldName, out FieldAccessInfo<T> info))
		{
			return GetFieldUsingReflection<T>(target, info?.fieldInfo);
		}
		if (Platform.UseFieldInfoCache)
		{
			FieldInfo fieldInfo = info.fieldInfo;
			if (fieldInfo == null)
			{
				return default(T);
			}
			try
			{
				return (T)fieldInfo.GetValue(target);
			}
			catch (Exception)
			{
				return default(T);
			}
		}
		try
		{
			return info.Getter(target);
		}
		catch (Exception)
		{
			return default(T);
		}
	}

	private bool EnsureCompiled<T>(Type targetType, string fieldName, out FieldAccessInfo<T> info)
	{
		info = null;
		if (!accessCache.TryGetValue(fieldName, out var value))
		{
			try
			{
				value = new FieldAccessInfo<T>(targetType, fieldName);
				accessCache[fieldName] = value;
			}
			catch (Exception)
			{
				return false;
			}
		}
		try
		{
			info = (FieldAccessInfo<T>)value;
			return info.IsCompiled;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private bool EnsureCompiled<T>(object target, string fieldName, out FieldAccessInfo<T> info)
	{
		info = null;
		if (!accessCache.TryGetValue(fieldName, out var value))
		{
			try
			{
				value = new FieldAccessInfo<T>(target, fieldName);
				accessCache[fieldName] = value;
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				return false;
			}
		}
		try
		{
			info = (FieldAccessInfo<T>)value;
			return info.IsCompiled;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private bool SetFieldUsingReflection<T>(object target, FieldInfo fieldInfo, T value)
	{
		if (fieldInfo == null)
		{
			return false;
		}
		try
		{
			fieldInfo.SetValue(target, value);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private T GetFieldUsingReflection<T>(object target, FieldInfo fieldInfo)
	{
		if (fieldInfo == null)
		{
			return default(T);
		}
		try
		{
			return (T)fieldInfo.GetValue(target);
		}
		catch (Exception)
		{
			return default(T);
		}
	}

	public bool FieldExists<T>(Type targetType, string fieldName)
	{
		EnsureCompiled(targetType, fieldName, out FieldAccessInfo<T> info);
		if (info != null)
		{
			return info.fieldInfo != null;
		}
		return false;
	}

	public bool FieldExists<T>(object target, string fieldName)
	{
		EnsureCompiled(target, fieldName, out FieldAccessInfo<T> info);
		if (info != null)
		{
			return info.fieldInfo != null;
		}
		return false;
	}
}
