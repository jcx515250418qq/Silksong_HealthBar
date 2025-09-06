using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public sealed class CameraTargetMethods : FsmStateAction
	{
		public enum Method
		{
			SetSprint = 0,
			SetWallSprint = 1
		}

		public static class HeroMethods
		{
			public static readonly Dictionary<Method, CameraMethod> methodInfos;

			static HeroMethods()
			{
				methodInfos = new Dictionary<Method, CameraMethod>();
				methodInfos.Add(Method.SetSprint, new CameraMethod(new VariableType[1] { VariableType.Bool }, VariableType.Unknown, delegate(CameraTargetMethods action)
				{
					if (action.hasCameraTarget)
					{
						action.heroController.SetSprint(action.parameters[0].boolValue);
					}
				}));
				methodInfos.Add(Method.SetWallSprint, new CameraMethod(new VariableType[1] { VariableType.Bool }, VariableType.Unknown, delegate(CameraTargetMethods action)
				{
					if (action.hasCameraTarget)
					{
						action.heroController.SetWallSprint(action.parameters[0].boolValue);
					}
				}));
			}
		}

		public sealed class CameraMethod
		{
			public Action<CameraTargetMethods> fsmAction;

			public VariableType[] parameters { get; }

			public VariableType returnType { get; }

			public void SendMethod(CameraTargetMethods action)
			{
				fsmAction?.Invoke(action);
			}

			public CameraMethod(VariableType[] parameters = null, VariableType returnType = VariableType.Unknown, Action<CameraTargetMethods> fsmAction = null)
			{
				if (parameters == null)
				{
					this.parameters = new VariableType[0];
				}
				else
				{
					this.parameters = parameters;
				}
				this.returnType = returnType;
				this.fsmAction = fsmAction;
			}
		}

		public Method method;

		[HideIf("ShouldHideParameters")]
		public FsmVar[] parameters;

		public FsmBool everyFrame;

		[HideIf("ShouldHideStoreValue")]
		[ActionSection("Store Result")]
		[UIHint(UIHint.Variable)]
		public FsmVar storeValue;

		[ActionSection("Events")]
		[HideIf("ShouldHideTrueFalse")]
		public FsmEvent isTrue;

		[HideIf("ShouldHideTrueFalse")]
		public FsmEvent isFalse;

		private bool hasCameraTarget;

		private CameraTarget heroController;

		public override void Reset()
		{
			parameters = null;
			everyFrame = null;
			storeValue = new FsmVar();
			isTrue = null;
			isFalse = null;
		}

		public override void OnEnter()
		{
			hasCameraTarget = heroController;
			if (!hasCameraTarget)
			{
				GameCameras instance = GameCameras.instance;
				if ((bool)instance)
				{
					heroController = instance.cameraTarget;
					hasCameraTarget = heroController;
				}
			}
			if (!hasCameraTarget || !everyFrame.Value)
			{
				SendMethod();
				Finish();
			}
			if (!hasCameraTarget)
			{
				Debug.LogError("Failed to find camera target.");
			}
		}

		public override void OnUpdate()
		{
			SendMethod();
		}

		private void SendMethod()
		{
			if (!hasCameraTarget)
			{
				return;
			}
			if (HeroMethods.methodInfos.TryGetValue(method, out var value))
			{
				try
				{
					if (parameters != null)
					{
						for (int i = 0; i < parameters.Length; i++)
						{
							parameters[i].UpdateValue();
						}
					}
					value.SendMethod(this);
					if (storeValue.Type == VariableType.Bool)
					{
						if (storeValue.boolValue)
						{
							base.Fsm.Event(isTrue);
						}
						else
						{
							base.Fsm.Event(isFalse);
						}
					}
					return;
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					Finish();
					return;
				}
			}
			Debug.LogError($"Failed to find hero method for \"{method}\".");
		}

		public bool ShouldHideParameters()
		{
			if (HeroMethods.methodInfos.TryGetValue(method, out var _))
			{
				if (parameters != null)
				{
					return parameters.Length == 0;
				}
				return true;
			}
			return false;
		}

		public bool ShouldHideStoreValue()
		{
			return GetReturnType() == VariableType.Unknown;
		}

		public bool ShouldHideTrueFalse()
		{
			return GetReturnType() != VariableType.Bool;
		}

		public VariableType GetReturnType()
		{
			if (HeroMethods.methodInfos.TryGetValue(method, out var value))
			{
				return value.returnType;
			}
			return VariableType.Unknown;
		}
	}
}
