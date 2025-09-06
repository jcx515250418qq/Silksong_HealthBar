using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.ScriptControl)]
	public class CallMethodProper : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The game object that owns the Behaviour.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[UIHint(UIHint.Behaviour)]
		[Tooltip("The Behaviour that contains the method to start as a coroutine.")]
		public FsmString behaviour;

		[UIHint(UIHint.Method)]
		[Tooltip("Name of the method to call on the component")]
		public FsmString methodName;

		[Tooltip("Method paramters. NOTE: these must match the method's signature!")]
		public FsmVar[] parameters;

		[ActionSection("Store Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result of the method call.")]
		public FsmVar storeResult;

		public bool EveryFrame;

		private Type cachedType;

		private MethodInfo cachedMethodInfo;

		private ParameterInfo[] cachedParameterInfo;

		private object[] parametersArray;

		private string errorString;

		private MonoBehaviour component;

		public override void Awake()
		{
			if (Application.isPlaying)
			{
				PreCache();
			}
		}

		public override void OnEnter()
		{
			parametersArray = new object[parameters.Length];
			DoMethodCall();
			if (!EveryFrame)
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
			if (string.IsNullOrEmpty(behaviour.Value) || string.IsNullOrEmpty(methodName.Value))
			{
				Finish();
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			component = ownerDefaultTarget.GetComponent(behaviour.Value) as MonoBehaviour;
			if (component == null)
			{
				LogWarning("CallMethodProper: " + ownerDefaultTarget.name + " missing behaviour: " + behaviour.Value);
				return;
			}
			if (cachedMethodInfo == null || !cachedMethodInfo.Name.Equals(methodName.Value))
			{
				errorString = string.Empty;
				if (!DoCache())
				{
					Debug.LogError(errorString, base.Owner);
					Finish();
					return;
				}
			}
			object value = null;
			if (cachedParameterInfo.Length == 0)
			{
				value = cachedMethodInfo.Invoke(component, null);
			}
			else
			{
				for (int i = 0; i < parameters.Length; i++)
				{
					FsmVar fsmVar = parameters[i];
					fsmVar.UpdateValue();
					parametersArray[i] = fsmVar.GetValue();
				}
				try
				{
					value = cachedMethodInfo.Invoke(component, parametersArray);
				}
				catch (TargetParameterCountException)
				{
					ParameterInfo[] array = cachedMethodInfo.GetParameters();
					Debug.LogErrorFormat(base.Owner, "Count did not match. Required: {0}, Was: {1}, Method: {2}", array.Length, parametersArray.Length, cachedMethodInfo.Name);
				}
				catch (Exception ex2)
				{
					Debug.LogError("CallMethodProper error on " + base.Fsm.OwnerName + " -> " + ex2, base.Owner);
				}
			}
			if (storeResult.Type != VariableType.Unknown)
			{
				storeResult.SetValue(value);
			}
		}

		private bool DoCache()
		{
			cachedType = component.GetType();
			try
			{
				cachedMethodInfo = cachedType.GetMethod(methodName.Value);
			}
			catch (AmbiguousMatchException)
			{
				Type[] types = parameters.Select((FsmVar fsmVar) => fsmVar.RealType).ToArray();
				cachedMethodInfo = cachedType.GetMethod(methodName.Value, types);
			}
			if (cachedMethodInfo == null)
			{
				errorString = errorString + "Method Name is invalid: " + methodName.Value + "\n";
				Finish();
				return false;
			}
			cachedParameterInfo = cachedMethodInfo.GetParameters();
			return true;
		}

		private void PreCache()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			component = ownerDefaultTarget.GetComponent(behaviour.Value) as MonoBehaviour;
			if (component == null)
			{
				return;
			}
			cachedType = component.GetType();
			try
			{
				cachedMethodInfo = cachedType.GetMethod(methodName.Value);
			}
			catch (AmbiguousMatchException)
			{
				Type[] types = parameters.Select((FsmVar fsmVar) => fsmVar.RealType).ToArray();
				cachedMethodInfo = cachedType.GetMethod(methodName.Value, types);
			}
			if (cachedMethodInfo == null)
			{
				errorString = errorString + "Method Name is invalid: " + methodName.Value + "\n";
			}
			else
			{
				cachedParameterInfo = cachedMethodInfo.GetParameters();
			}
		}
	}
}
