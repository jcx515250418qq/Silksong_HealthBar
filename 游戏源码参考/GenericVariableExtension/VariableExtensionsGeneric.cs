using System;
using System.Collections.Generic;
using System.Reflection;

namespace GenericVariableExtension
{
	public static class VariableExtensionsGeneric
	{
		private sealed class FieldCache
		{
			public sealed class TypeCache
			{
				private bool cachedFieldInfos;

				private FieldInfo[] fieldInfos;

				private bool cachedPropertyInfos;

				private PropertyInfo[] propertyInfos;

				public Dictionary<string, VariableCache> lookup = new Dictionary<string, VariableCache>();

				public FieldInfo[] GetFieldInfos(Type objType)
				{
					if (!cachedFieldInfos)
					{
						cachedFieldInfos = true;
						fieldInfos = objType.GetFields();
					}
					return fieldInfos;
				}

				public PropertyInfo[] GetPropertyInfos(Type objType)
				{
					if (!cachedPropertyInfos)
					{
						cachedPropertyInfos = true;
						propertyInfos = objType.GetProperties();
					}
					return propertyInfos;
				}
			}

			public abstract class VariableCache
			{
				public abstract bool IsCorrectType(Type expectedType);

				public abstract Type GetVariableType();

				public abstract object GetVariable(object obj);

				public abstract void SetVariable(object obj, object value);
			}

			private sealed class FieldInfoCache : VariableCache
			{
				private FieldInfo fieldInfo;

				public FieldInfoCache(FieldInfo fieldInfo)
				{
					this.fieldInfo = fieldInfo;
				}

				public override bool IsCorrectType(Type expectedType)
				{
					return fieldInfo.FieldType == expectedType;
				}

				public override Type GetVariableType()
				{
					return fieldInfo.FieldType;
				}

				public override object GetVariable(object obj)
				{
					return fieldInfo.GetValue(obj);
				}

				public override void SetVariable(object obj, object value)
				{
					fieldInfo.SetValue(obj, value);
				}
			}

			private sealed class PropertyInfoCache : VariableCache
			{
				private PropertyInfo propertyInfo;

				public PropertyInfoCache(PropertyInfo propertyInfo)
				{
					this.propertyInfo = propertyInfo;
				}

				public override bool IsCorrectType(Type expectedType)
				{
					return propertyInfo.PropertyType == expectedType;
				}

				public override Type GetVariableType()
				{
					return propertyInfo.PropertyType;
				}

				public override object GetVariable(object obj)
				{
					return propertyInfo.GetValue(obj, null);
				}

				public override void SetVariable(object obj, object value)
				{
					propertyInfo.SetValue(obj, value, null);
				}
			}

			private sealed class InvalidInfoCache : VariableCache
			{
				public override bool IsCorrectType(Type expectedType)
				{
					return false;
				}

				public override Type GetVariableType()
				{
					return null;
				}

				public override object GetVariable(object obj)
				{
					return null;
				}

				public override void SetVariable(object obj, object value)
				{
				}
			}

			private Dictionary<Type, TypeCache> typeCaches = new Dictionary<Type, TypeCache>();

			public TypeCache GetTypeCache(Type type)
			{
				if (!typeCaches.TryGetValue(type, out var value))
				{
					value = new TypeCache();
					typeCaches.Add(type, value);
				}
				return value;
			}

			public void ClearCache()
			{
				typeCaches.Clear();
			}

			private VariableCache GetVariableCache(Type objType, string fieldName)
			{
				TypeCache typeCache = GetTypeCache(objType);
				if (!typeCache.lookup.TryGetValue(fieldName, out var value))
				{
					FieldInfo field = objType.GetField(fieldName);
					if (field != null)
					{
						value = new FieldInfoCache(field);
					}
					else
					{
						PropertyInfo property = objType.GetProperty(fieldName);
						value = ((!(property != null)) ? ((VariableCache)new InvalidInfoCache()) : ((VariableCache)new PropertyInfoCache(property)));
					}
					typeCache.lookup.Add(fieldName, value);
				}
				return value;
			}

			public object GetVariable(object obj, string fieldName, Type expectedType)
			{
				Type type = obj.GetType();
				VariableCache variableCache = GetVariableCache(type, fieldName);
				if (!variableCache.IsCorrectType(expectedType))
				{
					return null;
				}
				return variableCache.GetVariable(obj);
			}

			public void SetVariable(object obj, string fieldName, object value, Type expectedType)
			{
				Type type = obj.GetType();
				VariableCache variableCache = GetVariableCache(type, fieldName);
				if (variableCache.IsCorrectType(expectedType))
				{
					variableCache.SetVariable(obj, value);
				}
			}

			public bool VariableExists(Type objType, string fieldName, Type expectedType)
			{
				return GetVariableCache(objType, fieldName).IsCorrectType(expectedType);
			}
		}

		private static readonly FieldCache variableCache = new FieldCache();

		public static void ClearCache()
		{
			variableCache.ClearCache();
		}

		public static void SetVariable<T>(this object obj, string fieldName, T value)
		{
			obj.SetVariable(fieldName, value, typeof(T));
		}

		public static T GetVariable<T>(this object obj, string fieldName)
		{
			object variable = obj.GetVariable(fieldName, typeof(T));
			if (variable != null)
			{
				return (T)variable;
			}
			return default(T);
		}

		public static List<T> GetVariables<T>(this object obj, string fieldNameContains)
		{
			Func<string, bool> predicate = ((!string.IsNullOrEmpty(fieldNameContains)) ? ((Func<string, bool>)((string name) => name.Contains(fieldNameContains))) : null);
			return obj.GetVariables<T>(predicate);
		}

		public static List<T> GetVariables<T>(this object obj, Func<string, bool> predicate)
		{
			Type type = obj.GetType();
			FieldCache.TypeCache typeCache = variableCache.GetTypeCache(type);
			FieldInfo[] fieldInfos = typeCache.GetFieldInfos(type);
			PropertyInfo[] propertyInfos = typeCache.GetPropertyInfos(type);
			List<T> list = new List<T>();
			FieldInfo[] array = fieldInfos;
			foreach (FieldInfo fieldInfo in array)
			{
				if (fieldInfo.FieldType == typeof(T) && (predicate == null || predicate(fieldInfo.Name)))
				{
					list.Add((T)fieldInfo.GetValue(obj));
				}
			}
			PropertyInfo[] array2 = propertyInfos;
			foreach (PropertyInfo propertyInfo in array2)
			{
				if (propertyInfo.PropertyType == typeof(T) && (predicate == null || predicate(propertyInfo.Name)))
				{
					list.Add((T)propertyInfo.GetValue(obj, null));
				}
			}
			return list;
		}

		public static void SetVariable(this object obj, string fieldName, object value, Type type)
		{
			variableCache.SetVariable(obj, fieldName, value, type);
		}

		public static object GetVariable(this object obj, string fieldName, Type type)
		{
			return variableCache.GetVariable(obj, fieldName, type);
		}

		public static bool VariableExists<TVar, TContainer>(string fieldName) where TContainer : class
		{
			return VariableExists<TContainer>(fieldName, typeof(TVar));
		}

		public static bool VariableExists<TContainer>(string fieldName, Type type) where TContainer : class
		{
			return variableCache.VariableExists(typeof(TContainer), fieldName, type);
		}
	}
}
