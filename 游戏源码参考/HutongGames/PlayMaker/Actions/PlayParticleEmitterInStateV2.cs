using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Particle System")]
	[Tooltip("Set particle emission on or off on an object with a particle emitter")]
	public class PlayParticleEmitterInStateV2 : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		public FsmBool withChildren;

		[ObjectType(typeof(ParticleSystemStopBehavior))]
		public FsmEnum stopAction;

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
					ParticleSystem component = ownerDefaultTarget.GetComponent<ParticleSystem>();
					if ((bool)component)
					{
						component.Play();
					}
				}
			}
			Finish();
		}

		public override void OnExit()
		{
			if (gameObject == null)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != null)
			{
				ParticleSystem component = ownerDefaultTarget.GetComponent<ParticleSystem>();
				if ((bool)component && component.isPlaying)
				{
					component.Stop(withChildren.Value, (ParticleSystemStopBehavior)(object)stopAction.Value);
				}
			}
		}
	}
}
