using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Tweens the alpha of the CanvasRenderer color associated with this Graphic.")]
	public class UiGraphicCrossFadeAlpha : ComponentAction<Graphic>
	{
		[RequiredField]
		[CheckForComponent(typeof(Graphic))]
		[Tooltip("The GameObject with an Unity UI component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The alpha target")]
		public FsmFloat alpha;

		[Tooltip("The duration of the tween")]
		public FsmFloat duration;

		[Tooltip("Should ignore Time.scale?")]
		public FsmBool ignoreTimeScale;

		private Graphic uiComponent;

		public override void Reset()
		{
			gameObject = null;
			alpha = null;
			duration = 1f;
			ignoreTimeScale = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				uiComponent = cachedComponent;
			}
			uiComponent.CrossFadeAlpha(alpha.Value, duration.Value, ignoreTimeScale.Value);
			Finish();
		}
	}
}
