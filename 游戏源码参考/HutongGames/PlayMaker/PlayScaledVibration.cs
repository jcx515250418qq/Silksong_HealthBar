using UnityEngine;

namespace HutongGames.PlayMaker
{
	public sealed class PlayScaledVibration : FsmStateAction
	{
		[ObjectType(typeof(ScaledVibration))]
		[RequiredField]
		public FsmOwnerDefault target;

		public FsmFloat fadeInDuration;

		[Space]
		public FsmBool stopOnStateExit;

		public FsmFloat fadeOutDuration;

		private ScaledVibration vibrationPlayer;

		public override void Reset()
		{
			target = null;
			stopOnStateExit = null;
		}

		public override void OnEnter()
		{
			vibrationPlayer = target.GetSafe<ScaledVibration>(this);
			if (vibrationPlayer != null)
			{
				vibrationPlayer.PlayVibration(fadeInDuration.Value);
			}
			Finish();
		}

		public override void OnExit()
		{
			if (stopOnStateExit.Value && vibrationPlayer != null)
			{
				if (fadeOutDuration.Value > 0f)
				{
					vibrationPlayer.FadeOut(fadeOutDuration.Value);
					vibrationPlayer = null;
				}
				else
				{
					vibrationPlayer.StopVibration();
					vibrationPlayer = null;
				}
			}
		}
	}
}
