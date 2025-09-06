using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Particle System")]
	[Tooltip("Set particle emission on or off on an object with a particle emitter")]
	public class StopParticleEmittersInChildrenV2 : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault Target;

		public ParticleSystemStopBehavior StopBehaviour;

		public override void Reset()
		{
			Target = null;
			StopBehaviour = ParticleSystemStopBehavior.StopEmitting;
		}

		public override void OnEnter()
		{
			if (Target != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(Target);
				if (ownerDefaultTarget != null)
				{
					ParticleSystem[] componentsInChildren = ownerDefaultTarget.GetComponentsInChildren<ParticleSystem>();
					foreach (ParticleSystem particleSystem in componentsInChildren)
					{
						if (particleSystem.isPlaying)
						{
							particleSystem.Stop(withChildren: true, StopBehaviour);
						}
					}
				}
			}
			Finish();
		}
	}
}
