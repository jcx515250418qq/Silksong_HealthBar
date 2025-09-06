using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Particle System")]
	public class SetParticleColour : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		public FsmColor colour;

		private ParticleSystem emitter;

		public override void Reset()
		{
			gameObject = null;
			colour = null;
		}

		public override void OnEnter()
		{
			if (gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if (ownerDefaultTarget != null)
				{
					emitter = ownerDefaultTarget.GetComponent<ParticleSystem>();
					ParticleSystem.MainModule main = emitter.main;
					main.startColor = colour.Value;
				}
			}
			Finish();
		}
	}
}
