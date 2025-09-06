using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class DoSpriteFlashCustom : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmColor FlashColour;

		public FsmFloat Amount;

		public FsmFloat TimeUp;

		public FsmFloat StayTime;

		public FsmFloat TimeDown;

		public FsmFloat StayDownTime;

		public FsmBool Repeating;

		public FsmInt RepeatTimes;

		public FsmInt RepeatingPriority;

		[HideIf("HideCancelOnExit")]
		public FsmBool CancelOnExit;

		private SpriteFlash spriteFlash;

		private SpriteFlash.FlashHandle? flashHandle;

		[UsedImplicitly]
		private bool HideCancelOnExit()
		{
			return !Repeating.Value;
		}

		public override void Reset()
		{
			Target = null;
			FlashColour = null;
			Amount = null;
			TimeUp = null;
			StayTime = null;
			TimeDown = null;
			StayDownTime = null;
			Repeating = null;
			RepeatTimes = null;
			RepeatingPriority = null;
			CancelOnExit = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			spriteFlash = (safe ? safe.GetComponent<SpriteFlash>() : null);
			if (spriteFlash == null)
			{
				flashHandle = null;
				Finish();
				return;
			}
			flashHandle = spriteFlash.Flash(FlashColour.Value, Amount.Value, TimeUp.Value, StayTime.Value, TimeDown.Value, StayDownTime.Value, Repeating.Value, RepeatTimes.Value, RepeatingPriority.Value);
			if (!Repeating.Value)
			{
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
	}
}
