using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class StartFadeOutAudioSource : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmFloat Duration;

		public FsmBool RecycleOnEnd;

		public override void Reset()
		{
			Target = null;
			Duration = null;
			RecycleOnEnd = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				FadeOutAudioSource fadeOutAudioSource = safe.GetComponent<FadeOutAudioSource>() ?? safe.AddComponent<FadeOutAudioSource>();
				if ((bool)fadeOutAudioSource)
				{
					fadeOutAudioSource.StartFade(Duration.Value, RecycleOnEnd.Value ? FadeOutAudioSource.EndBehaviours.Recycle : FadeOutAudioSource.EndBehaviours.None);
				}
			}
			Finish();
		}
	}
}
