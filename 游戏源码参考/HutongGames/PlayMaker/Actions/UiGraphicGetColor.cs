using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the color of a UI Graphic component. (E.g. UI Sprite)")]
	public class UiGraphicGetColor : ComponentAction<Graphic>
	{
		[RequiredField]
		[CheckForComponent(typeof(Graphic))]
		[Tooltip("The GameObject with the UI component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Color of the UI component")]
		public FsmColor color;

		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		private Graphic uiComponent;

		public override void Reset()
		{
			gameObject = null;
			color = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				uiComponent = cachedComponent;
			}
			DoGetColorValue();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetColorValue();
		}

		private void DoGetColorValue()
		{
			if (uiComponent != null)
			{
				color.Value = uiComponent.color;
			}
		}
	}
}
