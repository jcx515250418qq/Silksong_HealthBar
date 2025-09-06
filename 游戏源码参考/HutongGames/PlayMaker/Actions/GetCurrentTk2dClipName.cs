using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W721")]
	public class GetCurrentTk2dClipName : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmString storeName;

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
			storeName = null;
		}

		public override void OnEnter()
		{
			_getSprite();
			GetClipName();
			Finish();
		}

		private void GetClipName()
		{
			if (_sprite == null)
			{
				LogWarning("Missing tk2dSpriteAnimator component");
			}
			else if (_sprite.CurrentClip != null)
			{
				if (_sprite.CurrentClip.name != null)
				{
					storeName.Value = _sprite.CurrentClip.name;
				}
			}
			else if (_sprite.DefaultClip != null && _sprite.DefaultClip.name != null)
			{
				storeName.Value = _sprite.DefaultClip.name;
			}
		}
	}
}
