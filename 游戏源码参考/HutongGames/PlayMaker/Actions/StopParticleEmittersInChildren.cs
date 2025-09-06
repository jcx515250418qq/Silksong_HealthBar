using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Particle System")]
	[Tooltip("Set particle emission on or off on an object with a particle emitter")]
	public class StopParticleEmittersInChildren : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		public override void Reset()
		{
			gameObject = null;
		}

		public override void OnEnter()
		{
			if (gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if (ownerDefaultTarget != null)
				{
					ParticleSystem[] componentsInChildren = ownerDefaultTarget.GetComponentsInChildren<ParticleSystem>();
					foreach (ParticleSystem particleSystem in componentsInChildren)
					{
						if (particleSystem.isPlaying)
						{
							particleSystem.Stop();
						}
					}
				}
			}
			Finish();
		}
	}
}
