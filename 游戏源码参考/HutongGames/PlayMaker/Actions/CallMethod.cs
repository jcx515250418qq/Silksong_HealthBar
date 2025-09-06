using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Lets you call a method in a script on a Game Object.\nUnlike {{Invoke Method}}, or {{Send Message}}, you can use multiple parameters and get a return value.")]
	public class CallMethod : FsmStateAction
	{
		[ObjectType(typeof(Component))]
		[Tooltip("The behaviour on a Game Object that has the method you want to call. Drag the script component from the Unity inspector into this slot. HINT: Use Lock if the script is on another GameObject.\n\nNOTE: Unity Object fields show the GameObject name, so for clarity we show the Behaviour name as well below.")]
		public FsmObject behaviour;

		[Tooltip("Select from a list of available methods.\n\nNOTE: The full method signature is visible below.")]
		public FsmString methodName;

		[Tooltip("Method parameters.\n\nNOTE: This UI is built automatically when you select the method.")]
		public FsmVar[] parameters;

		[ActionSection("Store Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("If the Method has a return, use this to store it in a variable.")]
		public FsmVar storeResult;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		[Tooltip("Revert to the old Manual UI where all parameters had to be configured manually.")]
		public bool manualUI;

		private FsmObject cachedBehaviour;

		private FsmString cachedMethodName;

		private Type cachedType;

		private MethodInfo cachedMethodInfo;

		private ParameterInfo[] cachedParameterInfo;

		private object[] parametersArray;

		private string errorString;

		public override void Reset()
		{
			behaviour = null;
			methodName = null;
			parameters = null;
			storeResult = null;
			everyFrame = false;
		}

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
			if (behaviour.Value == null)
			{
				Finish();
				return;
			}
			if (NeedToUpdateCache() && !DoCache())
			{
				Debug.LogError(errorString);
				Finish();
				return;
			}
			object value;
			if (cachedParameterInfo.Length == 0)
			{
				value = cachedMethodInfo.Invoke(cachedBehaviour.Value, null);
			}
			else
			{
				for (int i = 0; i < parameters.Length; i++)
				{
					FsmVar fsmVar = parameters[i];
					fsmVar.UpdateValue();
					if (fsmVar.Type == VariableType.Array)
					{
						fsmVar.UpdateValue();
						object[] array = fsmVar.GetValue() as object[];
						Array array2 = Array.CreateInstance(cachedParameterInfo[i].ParameterType.GetElementType(), array.Length);
						for (int j = 0; j < array.Length; j++)
						{
							array2.SetValue(array[j], j);
						}
						parametersArray[i] = array2;
					}
					else
					{
						fsmVar.UpdateValue();
						parametersArray[i] = fsmVar.GetValue();
					}
				}
				value = cachedMethodInfo.Invoke(cachedBehaviour.Value, parametersArray);
			}
			if (storeResult != null && !storeResult.IsNone && storeResult.Type != VariableType.Unknown)
			{
				storeResult.SetValue(value);
			}
		}

		private bool NeedToUpdateCache()
		{
			if (cachedBehaviour != null && cachedMethodName != null && !(cachedBehaviour.Value != behaviour.Value) && !(cachedBehaviour.Name != behaviour.Name) && !(cachedMethodName.Value != methodName.Value))
			{
				return cachedMethodName.Name != methodName.Name;
			}
			return true;
		}

		private void ClearCache()
		{
			cachedBehaviour = null;
			cachedMethodName = null;
			cachedType = null;
			cachedMethodInfo = null;
			cachedParameterInfo = null;
		}

		private bool DoCache()
		{
			ClearCache();
			errorString = string.Empty;
			cachedBehaviour = new FsmObject(behaviour);
			cachedMethodName = new FsmString(methodName);
			if (cachedBehaviour.Value == null)
			{
				if (!behaviour.UsesVariable || Application.isPlaying)
				{
					errorString += "Behaviour is invalid!\n";
				}
				Finish();
				return false;
			}
			cachedType = behaviour.Value.GetType();
			List<Type> list = new List<Type>(parameters.Length);
			FsmVar[] array = parameters;
			foreach (FsmVar fsmVar in array)
			{
				list.Add(fsmVar.RealType);
			}
			cachedMethodInfo = cachedType.GetMethod(methodName.Value, list.ToArray());
			if (cachedMethodInfo == null)
			{
				errorString = errorString + "Invalid Method Name or Parameters: " + methodName.Value + "\n";
				Finish();
				return false;
			}
			cachedParameterInfo = cachedMethodInfo.GetParameters();
			return true;
		}

		public override string ErrorCheck()
		{
			if (Application.isPlaying)
			{
				return errorString;
			}
			if (!DoCache())
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
