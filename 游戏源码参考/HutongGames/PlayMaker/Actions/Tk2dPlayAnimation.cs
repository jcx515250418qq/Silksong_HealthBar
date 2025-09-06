using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Plays a sprite animation. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	public class Tk2dPlayAnimation : FsmStateAction
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

		private tk2dSpriteAnimator _sprite;

		private IHeroAnimationController heroAnim;

		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				_sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
				heroAnim = ownerDefaultTarget.GetComponent<IHeroAnimationController>();
			}
		}

		public override void Reset()
		{
			gameObject = null;
			animLibName = null;
			clipName = null;
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
				return;
			}
			animLibName.Value.Equals("");
			if (!string.IsNullOrWhiteSpace(clipName.Value))
			{
				tk2dSpriteAnimationClip clip = ((heroAnim != null) ? heroAnim.GetClip(clipName.Value) : _sprite.GetClipByName(clipName.Value));
				_sprite.Play(clip);
			}
		}
	}
}
