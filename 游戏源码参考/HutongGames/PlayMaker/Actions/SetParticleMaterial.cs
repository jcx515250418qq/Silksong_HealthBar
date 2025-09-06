using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Particle System")]
	public class SetParticleMaterial : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		public FsmMaterial material;

		private ParticleSystem emitter;

		public override void Reset()
		{
			gameObject = null;
			material = null;
		}

		public override void OnEnter()
		{
			if (gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if (ownerDefaultTarget != null)
				{
					emitter = ownerDefaultTarget.GetComponent<ParticleSystem>();
					ownerDefaultTarget.GetComponent<ParticleSystemRenderer>().material = material.Value;
				}
			}
			Finish();
		}
	}
}
