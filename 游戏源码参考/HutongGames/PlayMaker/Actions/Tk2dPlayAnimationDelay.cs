using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Plays a sprite animation. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	public class Tk2dPlayAnimationDelay : FsmStateAction
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

		public FsmFloat delay;

		private float timer;

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
			delay = null;
		}

		public override void OnEnter()
		{
			timer = 0f;
			_getSprite();
		}

		public override void OnUpdate()
		{
			if (timer < delay.Value)
			{
				timer += Time.deltaTime;
				return;
			}
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
			_sprite.Play(clipName.Value);
		}
	}
}
