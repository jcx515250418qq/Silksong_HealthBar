using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class DoSpriteFlashNamed : FsmStateAction
	{
		[Serializable]
		private enum FlashVariants
		{
			FlashingSuperDash = 0
		}

		public FsmOwnerDefault Target;

		[ObjectType(typeof(FlashVariants))]
		public FsmEnum Flash;

		public FsmBool CancelOnExit;

		[UIHint(UIHint.Variable)]
		public FsmInt FlashID;

		public FsmBool WaitForFlash;

		public FsmEvent FinishedFlashing;

		private SpriteFlash spriteFlash;

		private SpriteFlash.FlashHandle? flashHandle;

		public override void Reset()
		{
			Target = null;
			Flash = null;
			CancelOnExit = null;
			FlashID = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			spriteFlash = (safe ? safe.GetComponent<SpriteFlash>() : null);
			if (spriteFlash == null)
			{
				flashHandle = null;
				base.Fsm.Event(FinishedFlashing);
				Finish();
				return;
			}
			if (TryDoFlash(out var value))
			{
				flashHandle = value;
				FlashID.Value = value.ID;
			}
			else
			{
				flashHandle = null;
			}
			if (!WaitForFlash.Value)
			{
				base.Fsm.Event(FinishedFlashing);
				Finish();
			}
		}

		public override void OnExit()
		{
			if (CancelOnExit.Value && flashHandle.HasValue)
			{
				spriteFlash.CancelRepeatingFlash(flashHandle.Value);
			}
		}

		public override void OnUpdate()
		{
			if (!flashHandle.HasValue || !spriteFlash.IsFlashing(flashHandle.Value.ID))
			{
				base.Fsm.Event(FinishedFlashing);
				Finish();
			}
		}

		private bool TryDoFlash(out SpriteFlash.FlashHandle flashHandle)
		{
			if ((FlashVariants)(object)Flash.Value == FlashVariants.FlashingSuperDash)
			{
				flashHandle = spriteFlash.FlashingSuperDashHandled();
				return true;
			}
			flashHandle = default(SpriteFlash.FlashHandle);
			return false;
		}
	}
}
