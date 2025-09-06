using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Particle System")]
	[Tooltip("Set particle emission on or off on an object with a particle emitter")]
	public class PlayParticleEmitter : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		public FsmInt emit;

		public bool resetIfPlaying;

		public override void Reset()
		{
			gameObject = null;
			emit = new FsmInt(0);
			resetIfPlaying = false;
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
						if (emit.Value <= 0)
						{
							if (resetIfPlaying && component.isPlaying)
							{
								component.Stop();
							}
							component.Play();
						}
						else
						{
							component.Emit(emit.Value);
						}
					}
				}
			}
			Finish();
		}
	}
}
