using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class PlayParticleEffectsInState : FsmStateAction
	{
		[CheckForComponent(typeof(PlayParticleEffects))]
		public FsmOwnerDefault Target;

		public FsmFloat StartDelay;

		private PlayParticleEffects playEffects;

		private float delayTimeLeft;

		public override void Reset()
		{
			Target = null;
			StartDelay = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (!safe)
			{
				Finish();
				return;
			}
			playEffects = safe.GetComponent<PlayParticleEffects>();
			if (!playEffects)
			{
				Finish();
				return;
			}
			delayTimeLeft = StartDelay.Value;
			if (delayTimeLeft <= 0f)
			{
				playEffects.PlayParticleSystems();
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (delayTimeLeft > 0f)
			{
				delayTimeLeft -= Time.deltaTime;
				if (delayTimeLeft <= 0f)
				{
					playEffects.PlayParticleSystems();
					Finish();
				}
			}
		}

		public override void OnExit()
		{
			playEffects.StopParticleSystems();
		}
	}
}
