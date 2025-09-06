using System;
using System.Collections.Generic;
using System.Reflection;

namespace InControl
{
	public static class Reflector
	{
		private static readonly string[] ignoreAssemblies = new string[12]
		{
			"Unity", "UnityEngine", "UnityEditor", "mscorlib", "Microsoft", "System", "Mono", "JetBrains", "nunit", "ExCSS",
			"ICSharpCode", "AssetStoreTools"
		};

		private static IEnumerable<Type> assemblyTypes;

		public static IEnumerable<Type> AllAssemblyTypes => assemblyTypes ?? (assemblyTypes = GetAllAssemblyTypes());

		private static bool IgnoreAssemblyWithName(string assemblyName)
		{
			string[] array = ignoreAssemblies;
			foreach (string value in array)
			{
				if (assemblyName.StartsWith(value))
				{
					return true;
				}
			}
			return false;
		}

		private static IEnumerable<Type> GetAllAssemblyTypes()
		{
			List<Type> list = new List<Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				if (!IgnoreAssemblyWithName(assembly.GetName().Name))
				{
					Type[] array = null;
					try
					{
						array = assembly.GetTypes();
					}
					catch
					{
					}
					if (array != null)
					{
						list.AddRange(array);
					}
				}
			}
			return list;
		}
	}
}
