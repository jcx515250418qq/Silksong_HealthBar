using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Set the current clip frames per seconds on a animated sprite. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	public class Tk2dSetAnimationFrameRate : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The frame per seconds of the current clip")]
		public FsmFloat framePerSeconds;

		[Tooltip("Repeat every Frame")]
		public bool everyFrame;

		private tk2dSpriteAnimator _sprite;

		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				_sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
			}
		}

		public override void Reset()
		{
			gameObject = null;
			framePerSeconds = 30f;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			_getSprite();
			DoSetAnimationFPS();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetAnimationFPS();
		}

		private void DoSetAnimationFPS()
		{
			if (_sprite == null)
			{
				LogWarning("Missing tk2dSpriteAnimator component");
			}
			else
			{
				_sprite.CurrentClip.fps = framePerSeconds.Value;
			}
		}
	}
}
