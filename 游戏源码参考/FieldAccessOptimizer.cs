using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

public class FieldAccessOptimizer<TTarget, T>
{
	private sealed class FieldAccessInfo
	{
		public enum TargetType
		{
			None = 0,
			Field = 1,
			Property = 2
		}

		private readonly Action<object, T> _setValueDelegate;

		private readonly Func<object, T> _getValueDelegate;

		public readonly TargetType targetType;

		private FieldInfo fieldInfo;

		private PropertyInfo propertyInfo;

		private bool createdSetValueDelegate;

		private bool createdGetValueDelegate;

		public FieldAccessInfo(Type targetType, string fieldName)
		{
			try
			{
				fieldInfo = targetType.GetField(fieldName);
				if (fieldInfo != null)
				{
					this.targetType = TargetType.Field;
					return;
				}
				propertyInfo = targetType.GetProperty(fieldName);
				if (propertyInfo != null)
				{
					this.targetType = TargetType.Property;
				}
			}
			catch (Exception)
			{
			}
		}

		public void SetValue(object target, T value)
		{
			if (!createdSetValueDelegate)
			{
				switch (targetType)
				{
				case TargetType.Field:
					fieldInfo.SetValue(target, value);
					break;
				case TargetType.Property:
					propertyInfo.SetValue(target, value);
					break;
				}
			}
			else
			{
				_setValueDelegate(target, value);
			}
		}

		public T GetValue(object target)
		{
			if (!createdGetValueDelegate)
			{
				switch (targetType)
				{
				case TargetType.Field:
					return (T)fieldInfo.GetValue(target);
				case TargetType.Property:
					return (T)propertyInfo.GetValue(target);
				}
			}
			return _getValueDelegate(target);
		}

		private static Action<object, T> CreateFieldSetter(FieldInfo fieldInfo)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object));
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(T));
			return Expression.Lambda<Action<object, T>>(Expression.Assign(Expression.Field(Expression.Convert(parameterExpression, fieldInfo.DeclaringType), fieldInfo), Expression.Convert(parameterExpression2, fieldInfo.FieldType)), new ParameterExpression[2] { parameterExpression, parameterExpression2 }).Compile();
		}

		private static Func<object, T> CreateFieldGetter(FieldInfo fieldInfo)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object));
			return Expression.Lambda<Func<object, T>>(Expression.Convert(Expression.Field(Expression.Convert(parameterExpression, fieldInfo.DeclaringType), fieldInfo), typeof(T)), new ParameterExpression[1] { parameterExpression }).Compile();
		}

		private static Action<object, T> CreatePropertySetter(PropertyInfo propertyInfo)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object));
			ParameterExpression parameterExpression2 = Expression.Parameter(typeof(T));
			MethodInfo setMethod = propertyInfo.GetSetMethod();
			return Expression.Lambda<Action<object, T>>(Expression.Call(Expression.Convert(parameterExpression, propertyInfo.DeclaringType), setMethod, Expression.Convert(parameterExpression2, propertyInfo.PropertyType)), new ParameterExpression[2] { parameterExpression, parameterExpression2 }).Compile();
		}

		private static Func<object, T> CreatePropertyGetter(PropertyInfo propertyInfo)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object));
			MethodInfo getMethod = propertyInfo.GetGetMethod();
			return Expression.Lambda<Func<object, T>>(Expression.Convert(Expression.Call(Expression.Convert(parameterExpression, propertyInfo.DeclaringType), getMethod), typeof(T)), new ParameterExpression[1] { parameterExpression }).Compile();
		}
	}

	private readonly Dictionary<string, FieldAccessInfo> accessCache = new Dictionary<string, FieldAccessInfo>();

	public void SetField(object target, string fieldName, T value)
	{
		if (!EnsureCompiled(target, fieldName, out var info))
		{
			return;
		}
		try
		{
			info.SetValue(target, value);
		}
		catch (Exception)
		{
		}
	}

	public T GetField(object target, string fieldName)
	{
		if (!EnsureCompiled(target, fieldName, out var info))
		{
			return default(T);
		}
		try
		{
			return info.GetValue(target);
		}
		catch (Exception)
		{
			return default(T);
		}
	}

	private bool EnsureCompiled(Type targetType, string fieldName, out FieldAccessInfo info)
	{
		info = null;
		if (!accessCache.TryGetValue(fieldName, out var value))
		{
			try
			{
				value = new FieldAccessInfo(targetType, fieldName);
				if (value.targetType == FieldAccessInfo.TargetType.None)
				{
					return false;
				}
				accessCache[fieldName] = value;
			}
			catch (Exception)
			{
				return false;
			}
		}
		info = value;
		return info.targetType != FieldAccessInfo.TargetType.None;
	}

	private bool EnsureCompiled(object target, string fieldName, out FieldAccessInfo info)
	{
		if (target == null)
		{
			info = null;
			return false;
		}
		return EnsureCompiled(target.GetType(), fieldName, out info);
	}

	private bool SetFieldUsingReflection(object target, FieldInfo fieldInfo, T value)
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

	private T GetFieldUsingReflection(object target, FieldInfo fieldInfo)
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

	public bool FieldExists(Type targetType, string fieldName)
	{
		FieldAccessInfo info;
		return EnsureCompiled(targetType, fieldName, out info);
	}

	public bool FieldExists(object target, string fieldName)
	{
		FieldAccessInfo info;
		return EnsureCompiled(target, fieldName, out info);
	}
}
