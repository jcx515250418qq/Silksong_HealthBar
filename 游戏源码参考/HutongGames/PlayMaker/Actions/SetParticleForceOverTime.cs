using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Particle System")]
	public class SetParticleForceOverTime : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		public FsmBool enabled;

		public bool resetOnExit;

		public override void Reset()
		{
			gameObject = null;
			enabled = false;
			resetOnExit = false;
		}

		public override void OnEnter()
		{
			if (gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if (ownerDefaultTarget != null)
				{
					ParticleSystem.ForceOverLifetimeModule forceOverLifetime = ownerDefaultTarget.GetComponent<ParticleSystem>().forceOverLifetime;
					forceOverLifetime.enabled = enabled.Value;
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
					ParticleSystem.ForceOverLifetimeModule forceOverLifetime = ownerDefaultTarget.GetComponent<ParticleSystem>().forceOverLifetime;
					forceOverLifetime.enabled = !enabled.Value;
				}
			}
		}
	}
}
