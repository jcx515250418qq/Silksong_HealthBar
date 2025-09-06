using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public sealed class HeroControllerMethods : FsmStateAction
	{
		public enum Method
		{
			SetAllowNailChargingWhileRelinquished = 0,
			RelinquishControlNotVelocity = 1,
			StopAnimationControl = 2,
			FlipSprite = 3,
			ShouldHardLand = 4,
			StartAnimationControlToIdle = 5,
			RegainControl = 6,
			RelinquishControl = 7,
			DoHardLanding = 8,
			SetAllowRecoilWhileRelinquished = 9,
			ResetGravity = 10,
			TryFsmCancelToWallSlide = 11,
			DashCooldownReady = 12,
			SetStartWithDash = 13,
			CouldJumpCancel = 14,
			SetStartWithAnyJump = 15,
			AffectedByGravity = 16,
			StartAnimationControl = 17,
			IncrementAttackCounter = 18,
			AllowShuttleCock = 19,
			SetSilkRegenBlocked = 20,
			MaxHealthKeepBlue = 21,
			StopPlayingAudio = 22,
			CancelAttack = 23,
			RefreshAnimationEvents = 24,
			CancelQueuedBounces = 25,
			SetToolCooldown = 26,
			ForceClampTerminalVelocity = 27,
			IsParryInvulnerable = 28,
			TrySpawnSoftLandEffect = 29,
			IsParryingActive = 30,
			BlockSteepSlopes = 31,
			PlayIdle = 32,
			IsHurt = 33,
			ThrowToolCooldownReady = 34,
			CanTryHarpoonDash = 35,
			StartAnimationControlToIdleForcePlay = 36
		}

		public static class HeroMethods
		{
			public static readonly Dictionary<Method, HeroMethod> methodInfos;

			static HeroMethods()
			{
				methodInfos = new Dictionary<Method, HeroMethod>();
				methodInfos.Add(Method.SetAllowNailChargingWhileRelinquished, new HeroMethod(new VariableType[1] { VariableType.Bool }, VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.SetAllowNailChargingWhileRelinquished(action.parameters[0].boolValue);
					}
				}));
				methodInfos.Add(Method.RelinquishControlNotVelocity, new HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.RelinquishControlNotVelocity();
					}
				}));
				methodInfos.Add(Method.StopAnimationControl, new HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.StopAnimationControl();
					}
				}));
				methodInfos.Add(Method.FlipSprite, new HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.FlipSprite();
					}
				}));
				methodInfos.Add(Method.ShouldHardLand, new HeroMethod(new VariableType[1] { VariableType.GameObject }, VariableType.Bool, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.storeValue.boolValue = action.heroController.ShouldHardLand(action.parameters[0].gameObjectValue);
					}
				}));
				methodInfos.Add(Method.StartAnimationControlToIdle, new HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.StartAnimationControlToIdle();
					}
				}));
				methodInfos.Add(Method.StartAnimationControlToIdleForcePlay, new HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.StartAnimationControlToIdleForcePlay();
					}
				}));
				methodInfos.Add(Method.RegainControl, new HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.RegainControl();
					}
				}));
				methodInfos.Add(Method.RelinquishControl, new HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.RelinquishControl();
					}
				}));
				methodInfos.Add(Method.DoHardLanding, new HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.DoHardLanding();
					}
				}));
				methodInfos.Add(Method.SetAllowRecoilWhileRelinquished, new HeroMethod(new VariableType[1] { VariableType.Bool }, VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.SetAllowRecoilWhileRelinquished(action.parameters[0].boolValue);
					}
				}));
				methodInfos.Add(Method.ResetGravity, new HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.ResetGravity();
					}
				}));
				methodInfos.Add(Method.TryFsmCancelToWallSlide, new HeroMethod(Array.Empty<VariableType>(), VariableType.Bool, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.storeValue.boolValue = action.heroController.TryFsmCancelToWallSlide();
					}
				}));
				methodInfos.Add(Method.DashCooldownReady, new HeroMethod(Array.Empty<VariableType>(), VariableType.Bool, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.storeValue.boolValue = action.heroController.DashCooldownReady();
					}
				}));
				methodInfos.Add(Method.SetStartWithDash, new HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.SetStartWithDash();
					}
				}));
				methodInfos.Add(Method.CouldJumpCancel, new HeroMethod(Array.Empty<VariableType>(), VariableType.Bool, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.storeValue.boolValue = action.heroController.CouldJumpCancel();
					}
				}));
				methodInfos.Add(Method.SetStartWithAnyJump, new HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.SetStartWithAnyJump();
					}
				}));
				methodInfos.Add(Method.AffectedByGravity, new HeroMethod(new VariableType[1] { VariableType.Bool }, VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.AffectedByGravity(action.parameters[0].boolValue);
					}
				}));
				methodInfos.Add(Method.StartAnimationControl, new HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.StartAnimationControl();
					}
				}));
				methodInfos.Add(Method.IncrementAttackCounter, new HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.IncrementAttackCounter();
					}
				}));
				methodInfos.Add(Method.AllowShuttleCock, new HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.AllowShuttleCock();
					}
				}));
				methodInfos.Add(Method.SetSilkRegenBlocked, new HeroMethod(new VariableType[1] { VariableType.Bool }, VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.SetSilkRegenBlocked(action.parameters[0].boolValue);
					}
				}));
				methodInfos.Add(Method.MaxHealthKeepBlue, new HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.MaxHealthKeepBlue();
					}
				}));
				methodInfos.Add(Method.StopPlayingAudio, new HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.StopPlayingAudio();
					}
				}));
				methodInfos.Add(Method.CancelAttack, new HeroMethod(new VariableType[1] { VariableType.Bool }, VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.CancelAttack(action.parameters[0].boolValue);
					}
				}));
				methodInfos.Add(Method.RefreshAnimationEvents, new HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.StopPlayingAudio();
					}
				}));
				methodInfos.Add(Method.CancelQueuedBounces, new HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.CancelQueuedBounces();
					}
				}));
				methodInfos.Add(Method.SetToolCooldown, new HeroMethod(new VariableType[1], VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.SetToolCooldown(action.parameters[0].floatValue);
					}
				}));
				methodInfos.Add(Method.ForceClampTerminalVelocity, new HeroMethod(new VariableType[1] { VariableType.Bool }, VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.ForceClampTerminalVelocity = action.parameters[0].boolValue;
					}
				}));
				methodInfos.Add(Method.IsParryInvulnerable, new HeroMethod(Array.Empty<VariableType>(), VariableType.Bool, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.storeValue.boolValue = action.heroController.IsParrying();
					}
				}));
				methodInfos.Add(Method.TrySpawnSoftLandEffect, new HeroMethod(Array.Empty<VariableType>(), VariableType.Bool, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.storeValue.boolValue = action.heroController.TrySpawnSoftLandingPrefab();
					}
				}));
				methodInfos.Add(Method.IsParryingActive, new HeroMethod(Array.Empty<VariableType>(), VariableType.Bool, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.storeValue.boolValue = action.heroController.IsParryingActive();
					}
				}));
				methodInfos.Add(Method.BlockSteepSlopes, new HeroMethod(new VariableType[1] { VariableType.Bool }, VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.SetBlockSteepSlopes(action.parameters[0].boolValue);
					}
				}));
				methodInfos.Add(Method.PlayIdle, new HeroMethod(Array.Empty<VariableType>(), VariableType.Unknown, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.heroController.AnimCtrl.PlayIdle();
					}
				}));
				methodInfos.Add(Method.IsHurt, new HeroMethod(Array.Empty<VariableType>(), VariableType.Bool, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.storeValue.boolValue = action.heroController.AnimCtrl.IsHurt();
					}
				}));
				methodInfos.Add(Method.ThrowToolCooldownReady, new HeroMethod(Array.Empty<VariableType>(), VariableType.Bool, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.storeValue.boolValue = action.heroController.ThrowToolCooldownReady();
					}
				}));
				methodInfos.Add(Method.CanTryHarpoonDash, new HeroMethod(Array.Empty<VariableType>(), VariableType.Bool, delegate(HeroControllerMethods action)
				{
					if (action.hasHero)
					{
						action.storeValue.boolValue = action.heroController.CanTryHarpoonDash();
					}
				}));
			}
		}

		public sealed class HeroMethod
		{
			public Action<HeroControllerMethods> heroControllerAction;

			public VariableType[] parameters { get; }

			public VariableType returnType { get; }

			public void SendMethod(HeroControllerMethods action)
			{
				heroControllerAction?.Invoke(action);
			}

			public HeroMethod(VariableType[] parameters = null, VariableType returnType = VariableType.Unknown, Action<HeroControllerMethods> heroControllerAction = null)
			{
				if (parameters == null)
				{
					this.parameters = Array.Empty<VariableType>();
				}
				else
				{
					this.parameters = parameters;
				}
				this.returnType = returnType;
				this.heroControllerAction = heroControllerAction;
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

		private bool hasHero;

		private HeroController heroController;

		public override void Reset()
		{
			parameters = null;
			everyFrame = null;
			storeValue = new FsmVar();
			isTrue = null;
			isFalse = null;
		}

		public override void OnPreprocess()
		{
			ValidateParams();
		}

		public override void OnEnter()
		{
			hasHero = heroController;
			if (!hasHero)
			{
				heroController = HeroController.instance;
				hasHero = heroController;
			}
			if (!hasHero || !everyFrame.Value)
			{
				SendMethod();
				Finish();
			}
			if (!hasHero)
			{
				Debug.LogError("Failed to find hero controller.");
			}
		}

		public override void OnUpdate()
		{
			SendMethod();
		}

		private void SendMethod()
		{
			if (!hasHero)
			{
				return;
			}
			HeroMethod value;
			if (SendMethodFaster())
			{
				SendTestEvents();
			}
			else if (HeroMethods.methodInfos.TryGetValue(method, out value))
			{
				try
				{
					value.SendMethod(this);
					SendTestEvents();
				}
				catch (Exception)
				{
					Finish();
				}
			}
		}

		private void SendTestEvents()
		{
			if (storeValue.Type == VariableType.Bool)
			{
				base.Fsm.Event(storeValue.boolValue ? isTrue : isFalse);
			}
		}

		private bool GetBool(int index = 0)
		{
			bool result = false;
			if (parameters != null && parameters.Length >= index + 1)
			{
				FsmVar fsmVar = parameters[index];
				if (fsmVar != null)
				{
					result = fsmVar.boolValue;
				}
			}
			return result;
		}

		private GameObject GetGameObject(int index = 0)
		{
			GameObject result = null;
			if (parameters != null && parameters.Length >= index + 1)
			{
				FsmVar fsmVar = parameters[index];
				if (fsmVar != null)
				{
					result = fsmVar.gameObjectValue;
				}
			}
			return result;
		}

		private float GetFloat(int index = 0)
		{
			float result = 0f;
			if (parameters != null && parameters.Length >= index + 1)
			{
				FsmVar fsmVar = parameters[index];
				if (fsmVar != null)
				{
					result = fsmVar.floatValue;
				}
			}
			return result;
		}

		private bool SendMethodFaster()
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
				switch (method)
				{
				case Method.SetAllowNailChargingWhileRelinquished:
					heroController.SetAllowNailChargingWhileRelinquished(GetBool());
					return true;
				case Method.RelinquishControlNotVelocity:
					heroController.RelinquishControlNotVelocity();
					return true;
				case Method.StopAnimationControl:
					heroController.StopAnimationControl();
					return true;
				case Method.FlipSprite:
					heroController.FlipSprite();
					return true;
				case Method.ShouldHardLand:
				{
					bool flag10 = heroController.ShouldHardLand(GetGameObject());
					storeValue.SetValue(flag10);
					return true;
				}
				case Method.StartAnimationControlToIdle:
					heroController.StartAnimationControlToIdle();
					return true;
				case Method.StartAnimationControlToIdleForcePlay:
					heroController.StartAnimationControlToIdleForcePlay();
					return true;
				case Method.RegainControl:
					heroController.RegainControl();
					return true;
				case Method.RelinquishControl:
					heroController.RelinquishControl();
					return true;
				case Method.DoHardLanding:
					heroController.DoHardLanding();
					return true;
				case Method.SetAllowRecoilWhileRelinquished:
					heroController.SetAllowRecoilWhileRelinquished(GetBool());
					return true;
				case Method.ResetGravity:
					heroController.ResetGravity();
					return true;
				case Method.TryFsmCancelToWallSlide:
				{
					bool flag9 = heroController.TryFsmCancelToWallSlide();
					storeValue.SetValue(flag9);
					return true;
				}
				case Method.DashCooldownReady:
				{
					bool flag8 = heroController.DashCooldownReady();
					storeValue.SetValue(flag8);
					return true;
				}
				case Method.SetStartWithDash:
					heroController.SetStartWithDash();
					return true;
				case Method.CouldJumpCancel:
				{
					bool flag7 = heroController.CouldJumpCancel();
					storeValue.SetValue(flag7);
					return true;
				}
				case Method.SetStartWithAnyJump:
					heroController.SetStartWithAnyJump();
					return true;
				case Method.AffectedByGravity:
					heroController.AffectedByGravity(GetBool());
					return true;
				case Method.StartAnimationControl:
					heroController.StartAnimationControl();
					return true;
				case Method.IncrementAttackCounter:
					heroController.IncrementAttackCounter();
					return true;
				case Method.AllowShuttleCock:
					heroController.AllowShuttleCock();
					return true;
				case Method.SetSilkRegenBlocked:
					heroController.SetSilkRegenBlocked(GetBool());
					return true;
				case Method.MaxHealthKeepBlue:
					heroController.MaxHealthKeepBlue();
					return true;
				case Method.StopPlayingAudio:
					heroController.StopPlayingAudio();
					return true;
				case Method.CancelAttack:
					heroController.CancelAttack(GetBool());
					return true;
				case Method.RefreshAnimationEvents:
					heroController.AnimCtrl.RefreshAnimationEvents();
					return true;
				case Method.CancelQueuedBounces:
					heroController.CancelQueuedBounces();
					break;
				case Method.SetToolCooldown:
					heroController.SetToolCooldown(GetFloat());
					break;
				case Method.ForceClampTerminalVelocity:
					heroController.ForceClampTerminalVelocity = GetBool();
					break;
				case Method.IsParryInvulnerable:
				{
					bool flag6 = heroController.IsParrying();
					storeValue.SetValue(flag6);
					break;
				}
				case Method.TrySpawnSoftLandEffect:
				{
					bool flag5 = heroController.TrySpawnSoftLandingPrefab();
					storeValue.SetValue(flag5);
					break;
				}
				case Method.IsParryingActive:
				{
					bool flag4 = heroController.IsParryingActive();
					storeValue.SetValue(flag4);
					break;
				}
				case Method.BlockSteepSlopes:
					heroController.SetBlockSteepSlopes(GetBool());
					break;
				case Method.PlayIdle:
					heroController.AnimCtrl.PlayIdle();
					break;
				case Method.IsHurt:
				{
					bool flag3 = heroController.AnimCtrl.IsHurt();
					storeValue.SetValue(flag3);
					break;
				}
				case Method.ThrowToolCooldownReady:
				{
					bool flag2 = heroController.ThrowToolCooldownReady();
					storeValue.SetValue(flag2);
					break;
				}
				case Method.CanTryHarpoonDash:
				{
					bool flag = heroController.CanTryHarpoonDash();
					storeValue.SetValue(flag);
					break;
				}
				}
			}
			catch (Exception)
			{
			}
			return false;
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

		public void ValidateParams()
		{
			if (HeroMethods.methodInfos.TryGetValue(method, out var value))
			{
				EnsureParams(value.parameters);
				EnsureReturnValue(value.returnType);
			}
		}

		private void EnsureParams(params VariableType[] types)
		{
			if (types == null || types.Length == 0)
			{
				parameters = Array.Empty<FsmVar>();
				return;
			}
			if (parameters == null)
			{
				parameters = new FsmVar[types.Length];
			}
			else if (parameters.Length != types.Length)
			{
				Array.Resize(ref parameters, types.Length);
			}
			for (int i = 0; i < parameters.Length; i++)
			{
				FsmVar fsmVar = parameters[i];
				if (fsmVar == null)
				{
					fsmVar = (parameters[i] = new FsmVar());
				}
				fsmVar.Type = types[i];
				parameters[i] = fsmVar;
			}
		}

		private void EnsureReturnValue(VariableType variableType)
		{
			if (storeValue != null)
			{
				storeValue.Type = variableType;
			}
		}
	}
}
