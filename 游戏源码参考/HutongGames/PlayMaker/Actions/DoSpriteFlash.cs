using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class DoSpriteFlash : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmString FlashMethod;

		public FsmBool IsRepeating;

		[HideIf("HideCancelOnExit")]
		public FsmBool CancelOnExit;

		private SpriteFlash spriteFlash;

		private SpriteFlash.FlashHandle? flashHandle;

		[UsedImplicitly]
		private bool HideCancelOnExit()
		{
			return !IsRepeating.Value;
		}

		public override void Reset()
		{
			Target = null;
			FlashMethod = null;
			IsRepeating = null;
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
			MethodInfo method = typeof(SpriteFlash).GetMethod(FlashMethod.Value);
			flashHandle = ((method != null) ? (method.Invoke(spriteFlash, null) as SpriteFlash.FlashHandle?) : ((SpriteFlash.FlashHandle?)null));
			if (!IsRepeating.Value)
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
