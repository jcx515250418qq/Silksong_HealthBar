using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Call a static method in a class.\nNOTE: This is an advanced action - you need to know the full method signature to use this action.")]
	public class CallStaticMethod : FsmStateAction
	{
		[Tooltip("Full path to the class that contains the static method.")]
		public FsmString className;

		[Tooltip("The static method to call.")]
		public FsmString methodName;

		[Tooltip("Method parameters. NOTE: these must match the method's signature!")]
		public FsmVar[] parameters;

		[ActionSection("Store Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result of the method call.")]
		public FsmVar storeResult;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private Type cachedType;

		private string cachedClassName;

		private string cachedMethodName;

		private MethodInfo cachedMethodInfo;

		private ParameterInfo[] cachedParameterInfo;

		private object[] parametersArray;

		private string errorString;

		public override void OnEnter()
		{
			parametersArray = new object[parameters.Length];
			DoMethodCall();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoMethodCall();
		}

		private void DoMethodCall()
		{
			if (className == null || string.IsNullOrEmpty(className.Value))
			{
				Finish();
				return;
			}
			if (cachedClassName != className.Value || cachedMethodName != methodName.Value)
			{
				errorString = string.Empty;
				if (!DoCache())
				{
					Debug.LogError(errorString);
					Finish();
					return;
				}
			}
			object obj = null;
			if (cachedParameterInfo.Length == 0)
			{
				obj = cachedMethodInfo.Invoke(null, null);
			}
			else
			{
				for (int i = 0; i < parameters.Length; i++)
				{
					FsmVar fsmVar = parameters[i];
					fsmVar.UpdateValue();
					parametersArray[i] = fsmVar.GetValue();
				}
				obj = cachedMethodInfo.Invoke(null, parametersArray);
			}
			if (!storeResult.IsNone)
			{
				storeResult.SetValue(obj);
			}
		}

		private bool DoCache()
		{
			cachedType = ReflectionUtils.GetGlobalType(className.Value);
			if (cachedType == null)
			{
				errorString = errorString + "Class is invalid: " + className.Value + "\n";
				Finish();
				return false;
			}
			cachedClassName = className.Value;
			List<Type> list = new List<Type>(parameters.Length);
			FsmVar[] array = parameters;
			foreach (FsmVar fsmVar in array)
			{
				if (fsmVar != null && fsmVar.RealType != null)
				{
					list.Add(fsmVar.RealType);
				}
			}
			try
			{
				cachedMethodInfo = cachedType.GetMethod(methodName.Value, list.ToArray());
			}
			catch (Exception ex)
			{
				errorString = errorString + ex.Message + "\n";
			}
			if (cachedMethodInfo == null)
			{
				errorString = errorString + "Invalid Method Name or Parameters: " + methodName.Value + "\n";
				Finish();
				return false;
			}
			cachedMethodName = methodName.Value;
			cachedParameterInfo = cachedMethodInfo.GetParameters();
			return true;
		}

		public override string ErrorCheck()
		{
			errorString = string.Empty;
			DoCache();
			if (!string.IsNullOrEmpty(errorString))
			{
				return errorString;
			}
			if (parameters.Length != cachedParameterInfo.Length)
			{
				return "Parameter count does not match method.\nMethod has " + cachedParameterInfo.Length + " parameters.\nYou specified " + parameters.Length + " paramaters.";
			}
			for (int i = 0; i < parameters.Length; i++)
			{
				Type realType = parameters[i].RealType;
				Type parameterType = cachedParameterInfo[i].ParameterType;
				if ((object)realType != parameterType)
				{
					return "Parameters do not match method signature.\nParameter " + (i + 1) + " (" + realType?.ToString() + ") should be of type: " + parameterType;
				}
			}
			if ((object)cachedMethodInfo.ReturnType == typeof(void))
			{
				if (!string.IsNullOrEmpty(storeResult.variableName))
				{
					return "Method does not have return.\nSpecify 'none' in Store Result.";
				}
			}
			else if ((object)cachedMethodInfo.ReturnType != storeResult.RealType)
			{
				return "Store Result is of the wrong type.\nIt should be of type: " + cachedMethodInfo.ReturnType;
			}
			return string.Empty;
		}
	}
}
