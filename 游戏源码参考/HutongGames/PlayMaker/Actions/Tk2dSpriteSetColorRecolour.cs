using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Set the color of a sprite. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
	public class Tk2dSpriteSetColorRecolour : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;

		[Tooltip("The color")]
		[UIHint(UIHint.FsmColor)]
		public FsmColor color;

		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		public FsmBool enableRecolour;

		private tk2dBaseSprite _sprite;

		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				_sprite = ownerDefaultTarget.GetComponent<tk2dBaseSprite>();
			}
		}

		public override void Reset()
		{
			gameObject = null;
			color = null;
			everyframe = false;
		}

		public override void OnEnter()
		{
			_getSprite();
			DoSetSpriteColor();
			if (!everyframe)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetSpriteColor();
		}

		private void DoSetSpriteColor()
		{
			if (_sprite == null)
			{
				LogWarning("Missing tk2dBaseSprite component");
				return;
			}
			if (enableRecolour.Value)
			{
				base.Fsm.GetOwnerDefaultTarget(gameObject).GetComponent<tk2dSprite>().EnableKeyword("RECOLOUR");
			}
			else
			{
				base.Fsm.GetOwnerDefaultTarget(gameObject).GetComponent<tk2dSprite>().DisableKeyword("RECOLOUR");
			}
			if (_sprite.color != color.Value)
			{
				_sprite.color = color.Value;
			}
		}
	}
}
