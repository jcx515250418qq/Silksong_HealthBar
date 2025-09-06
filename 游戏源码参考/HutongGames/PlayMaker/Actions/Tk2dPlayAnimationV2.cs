using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Plays a sprite animation. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	public class Tk2dPlayAnimationV2 : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		[Tooltip("The anim Lib name. Leave empty to use the one current selected")]
		public FsmString animLibName;

		[RequiredField]
		[Tooltip("The clip name to play")]
		public FsmString clipName;

		[Tooltip("If true and requested anim clip is same as current clip, don't replay clip from the start")]
		public bool doNotResetCurrentClip;

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
			animLibName = null;
			clipName = null;
			doNotResetCurrentClip = false;
		}

		public override void OnEnter()
		{
			_getSprite();
			DoPlayAnimation();
			Finish();
		}

		private void DoPlayAnimation()
		{
			if (_sprite == null)
			{
				LogWarning("Missing tk2dSpriteAnimator component");
			}
			else if (!doNotResetCurrentClip || !(clipName.Value == _sprite.CurrentClip.name))
			{
				_sprite.Play(clipName.Value);
			}
		}
	}
}
