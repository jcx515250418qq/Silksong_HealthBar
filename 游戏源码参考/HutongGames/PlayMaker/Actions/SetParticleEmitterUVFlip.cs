using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Particle System")]
	public class SetParticleEmitterUVFlip : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		public FsmFloat flipU;

		public FsmFloat flipV;

		private ParticleSystem emitter;

		public override void Reset()
		{
			gameObject = null;
			flipU = null;
			flipV = null;
		}

		public override void OnEnter()
		{
			if (gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if (ownerDefaultTarget != null)
				{
					emitter = ownerDefaultTarget.GetComponent<ParticleSystem>();
					ParticleSystem.TextureSheetAnimationModule textureSheetAnimation = emitter.textureSheetAnimation;
					if (!flipU.IsNone)
					{
						textureSheetAnimation.flipU = flipU.Value;
					}
					if (!flipV.IsNone)
					{
						textureSheetAnimation.flipV = flipV.Value;
					}
				}
			}
			Finish();
		}
	}
}
