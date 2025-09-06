using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the texture of a UI RawImage component.")]
	public class UiRawImageSetTexture : ComponentAction<RawImage>
	{
		[RequiredField]
		[CheckForComponent(typeof(RawImage))]
		[Tooltip("The GameObject with the UI RawImage component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The texture of the UI RawImage component.")]
		public FsmTexture texture;

		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		private RawImage _texture;

		private Texture _originalTexture;

		public override void Reset()
		{
			gameObject = null;
			texture = null;
			resetOnExit = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				_texture = cachedComponent;
			}
			_originalTexture = _texture.texture;
			DoSetValue();
			Finish();
		}

		private void DoSetValue()
		{
			if (_texture != null)
			{
				_texture.texture = texture.Value;
			}
		}

		public override void OnExit()
		{
			if (!(_texture == null) && resetOnExit.Value)
			{
				_texture.texture = _originalTexture;
			}
		}
	}
}
