using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Particle System")]
	[Tooltip("Set particle emission on or off on an object with a particle emitter")]
	public class PlayParticleEmitterConditional : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		public FsmBool conditionalBool;

		public bool stopOnExit;

		private ParticleSystem emitter;

		private bool playing;

		public override void Reset()
		{
			gameObject = null;
		}

		public override void OnEnter()
		{
			if (gameObject == null)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != null)
			{
				emitter = ownerDefaultTarget.GetComponent<ParticleSystem>();
				if (!emitter)
				{
					Finish();
				}
			}
		}

		public override void OnUpdate()
		{
			playing = emitter.isPlaying;
			if (!playing && conditionalBool.Value)
			{
				emitter.Play();
			}
			if (playing && !conditionalBool.Value)
			{
				emitter.Stop();
			}
		}

		public override void OnExit()
		{
			if (stopOnExit)
			{
				emitter.Stop();
			}
		}
	}
}
