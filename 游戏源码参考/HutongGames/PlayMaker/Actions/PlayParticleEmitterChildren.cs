using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Particle System")]
	[Tooltip("Set particle emission on or off on an object with a particle emitter")]
	public class PlayParticleEmitterChildren : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public bool resetTimeIfPlaying;

		public bool stopOnStateExit;

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
					foreach (Transform item in ownerDefaultTarget.transform)
					{
						ParticleSystem component = item.GetComponent<ParticleSystem>();
						if ((bool)component)
						{
							component.Play();
							if (resetTimeIfPlaying)
							{
								component.time = 0f;
							}
						}
					}
				}
			}
			Finish();
		}

		public override void OnExit()
		{
			if (gameObject == null || !stopOnStateExit)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget != null))
			{
				return;
			}
			foreach (Transform item in ownerDefaultTarget.transform)
			{
				ParticleSystem component = item.GetComponent<ParticleSystem>();
				if ((bool)component)
				{
					component.Stop();
				}
			}
		}
	}
}
