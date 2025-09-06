using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Tween a sprite color \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
	public class Tk2dSpriteTweenColor : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[UIHint(UIHint.FsmColor)]
		public FsmColor color;

		[RequiredField]
		public FsmFloat duration;

		private float elapsed;

		private Color startColor;

		private tk2dBaseSprite sprite;

		public override void Reset()
		{
			gameObject = null;
			color = null;
			duration = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				sprite = ownerDefaultTarget.GetComponent<tk2dBaseSprite>();
				if (!sprite)
				{
					Finish();
					return;
				}
				elapsed = 0f;
				startColor = sprite.color;
			}
		}

		public override void OnUpdate()
		{
			if (elapsed <= duration.Value && duration.Value > 0f)
			{
				sprite.color = Color.Lerp(startColor, color.Value, elapsed / duration.Value);
				elapsed += Time.deltaTime;
			}
			else
			{
				sprite.color = color.Value;
				Finish();
			}
		}

		public override void OnExit()
		{
			sprite.color = color.Value;
		}
	}
}
