using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Particle System")]
	[Tooltip("Waits for particle system to stop playing")]
	public sealed class WaitParticleSystemsInChildren : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault target;

		public FsmBool stop;

		public FsmBool stopLoops;

		public FsmBool withChildren;

		public FsmFloat timeOut;

		private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

		private float timer;

		public override void Reset()
		{
			target = null;
			stop = null;
			stopLoops = null;
			withChildren = null;
			timeOut = new FsmFloat
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			bool flag = true;
			if (safe != null)
			{
				particleSystems.Clear();
				particleSystems.AddRange(safe.GetComponentsInChildren<ParticleSystem>());
				if (particleSystems.Count > 0)
				{
					flag = false;
					if (stop.Value)
					{
						foreach (ParticleSystem particleSystem in particleSystems)
						{
							particleSystem.Stop(withChildren.Value, ParticleSystemStopBehavior.StopEmitting);
						}
					}
					else if (stopLoops.Value)
					{
						foreach (ParticleSystem particleSystem2 in particleSystems)
						{
							if (particleSystem2.main.loop)
							{
								particleSystem2.Stop(withChildren.Value, ParticleSystemStopBehavior.StopEmitting);
							}
						}
					}
				}
			}
			if (flag)
			{
				Finish();
			}
		}

		public override void OnExit()
		{
			particleSystems.Clear();
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
			particleSystems.RemoveAll((ParticleSystem o) => o == null || !o.isPlaying);
			if (particleSystems.Count == 0)
			{
				Finish();
			}
		}
	}
}
