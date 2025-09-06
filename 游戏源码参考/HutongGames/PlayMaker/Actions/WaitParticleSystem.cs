using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Particle System")]
	[Tooltip("Waits for particle system to stop playing")]
	public sealed class WaitParticleSystem : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(ParticleSystem))]
		public FsmOwnerDefault target;

		public FsmBool stop;

		public FsmBool withChildren;

		public FsmFloat timeOut;

		private ParticleSystem particleSystem;

		private float timer;

		public override void Reset()
		{
			target = null;
			stop = null;
			withChildren = null;
			timeOut = new FsmFloat
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			particleSystem = target.GetSafe<ParticleSystem>(this);
			bool flag = true;
			if (particleSystem != null && particleSystem.isPlaying)
			{
				if (stop.Value)
				{
					particleSystem.Stop(withChildren.Value, ParticleSystemStopBehavior.StopEmitting);
				}
				timer = 0f;
				flag = false;
			}
			if (flag)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (!timeOut.IsNone)
			{
				timer += Time.deltaTime;
				if (timer >= timeOut.Value)
				{
					Finish();
					return;
				}
			}
			if (particleSystem == null)
			{
				Finish();
			}
			else if (!particleSystem.isPlaying)
			{
				Finish();
			}
		}
	}
}
