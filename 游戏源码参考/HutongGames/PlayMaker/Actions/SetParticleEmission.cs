using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Particle System")]
	[Tooltip("Set particle emission on or off on an object with a particle emitter")]
	public class SetParticleEmission : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		public FsmBool emission;

		public bool resetOnExit;

		public override void Reset()
		{
			gameObject = null;
			emission = false;
			resetOnExit = false;
		}

		public override void OnEnter()
		{
			if (gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if (ownerDefaultTarget != null)
				{
					ParticleSystem component = ownerDefaultTarget.GetComponent<ParticleSystem>();
					if ((bool)component)
					{
						ParticleSystem.EmissionModule emissionModule = component.emission;
						emissionModule.enabled = emission.Value;
					}
				}
			}
			Finish();
		}

		public override void OnExit()
		{
			if (resetOnExit && gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if (ownerDefaultTarget != null)
				{
					ParticleSystem.EmissionModule emissionModule = ownerDefaultTarget.GetComponent<ParticleSystem>().emission;
					emissionModule.enabled = !emission.Value;
				}
			}
		}
	}
}
