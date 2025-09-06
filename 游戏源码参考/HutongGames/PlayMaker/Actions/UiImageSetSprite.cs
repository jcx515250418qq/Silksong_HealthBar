using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the source image sprite of a UI Image component.")]
	public class UiImageSetSprite : ComponentAction<Image>
	{
		[RequiredField]
		[CheckForComponent(typeof(Image))]
		[Tooltip("The GameObject with the Image UI component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The source sprite of the UI Image component.")]
		[ObjectType(typeof(Sprite))]
		public FsmObject sprite;

		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		private Image image;

		private Sprite originalSprite;

		public override void Reset()
		{
			gameObject = null;
			resetOnExit = false;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				image = cachedComponent;
			}
			originalSprite = image.sprite;
			DoSetImageSourceValue();
			Finish();
		}

		private void DoSetImageSourceValue()
		{
			if (!(image == null))
			{
				image.sprite = sprite.Value as Sprite;
			}
		}

		public override void OnExit()
		{
			if (!(image == null) && resetOnExit.Value)
			{
				image.sprite = originalSprite;
			}
		}
	}
}
