using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets various properties of a UI Layout Element component.")]
	public class UiLayoutElementGetValues : ComponentAction<LayoutElement>
	{
		[RequiredField]
		[CheckForComponent(typeof(LayoutElement))]
		[Tooltip("The GameObject with the UI LayoutElement component.")]
		public FsmOwnerDefault gameObject;

		[ActionSection("Values")]
		[Tooltip("Is this element use Layout constraints")]
		[UIHint(UIHint.Variable)]
		public FsmBool ignoreLayout;

		[Tooltip("The minimum width enabled state")]
		[UIHint(UIHint.Variable)]
		public FsmBool minWidthEnabled;

		[Tooltip("The minimum width this layout element should have.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat minWidth;

		[Tooltip("The minimum height enabled state")]
		[UIHint(UIHint.Variable)]
		public FsmBool minHeightEnabled;

		[Tooltip("The minimum height this layout element should have.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat minHeight;

		[Tooltip("The preferred width enabled state")]
		[UIHint(UIHint.Variable)]
		public FsmBool preferredWidthEnabled;

		[Tooltip("The preferred width this layout element should have before additional available width is allocated.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat preferredWidth;

		[Tooltip("The preferred height enabled state")]
		[UIHint(UIHint.Variable)]
		public FsmBool preferredHeightEnabled;

		[Tooltip("The preferred height this layout element should have before additional available height is allocated.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat preferredHeight;

		[Tooltip("The flexible width enabled state")]
		[UIHint(UIHint.Variable)]
		public FsmBool flexibleWidthEnabled;

		[Tooltip("The relative amount of additional available width this layout element should fill out relative to its siblings.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat flexibleWidth;

		[Tooltip("The flexible height enabled state")]
		[UIHint(UIHint.Variable)]
		public FsmBool flexibleHeightEnabled;

		[Tooltip("The relative amount of additional available height this layout element should fill out relative to its siblings.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat flexibleHeight;

		[ActionSection("Options")]
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		private LayoutElement layoutElement;

		public override void Reset()
		{
			gameObject = null;
			ignoreLayout = null;
			minWidthEnabled = null;
			minHeightEnabled = null;
			preferredWidthEnabled = null;
			preferredHeightEnabled = null;
			flexibleWidthEnabled = null;
			flexibleHeightEnabled = null;
			minWidth = null;
			minHeight = null;
			preferredWidth = null;
			preferredHeight = null;
			flexibleWidth = null;
			flexibleHeight = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				layoutElement = cachedComponent;
			}
			DoGetValues();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetValues();
		}

		private void DoGetValues()
		{
			if (!(layoutElement == null))
			{
				if (!ignoreLayout.IsNone)
				{
					ignoreLayout.Value = layoutElement.ignoreLayout;
				}
				if (!minWidthEnabled.IsNone)
				{
					minWidthEnabled.Value = layoutElement.minWidth != 0f;
				}
				if (!minWidth.IsNone)
				{
					minWidth.Value = layoutElement.minWidth;
				}
				if (!minHeightEnabled.IsNone)
				{
					minHeightEnabled.Value = layoutElement.minHeight != 0f;
				}
				if (!minHeight.IsNone)
				{
					minHeight.Value = layoutElement.minHeight;
				}
				if (!preferredWidthEnabled.IsNone)
				{
					preferredWidthEnabled.Value = layoutElement.preferredWidth != 0f;
				}
				if (!preferredWidth.IsNone)
				{
					preferredWidth.Value = layoutElement.preferredWidth;
				}
				if (!preferredHeightEnabled.IsNone)
				{
					preferredHeightEnabled.Value = layoutElement.preferredHeight != 0f;
				}
				if (!preferredHeight.IsNone)
				{
					preferredHeight.Value = layoutElement.preferredHeight;
				}
				if (!flexibleWidthEnabled.IsNone)
				{
					flexibleWidthEnabled.Value = layoutElement.flexibleWidth != 0f;
				}
				if (!flexibleWidth.IsNone)
				{
					flexibleWidth.Value = layoutElement.flexibleWidth;
				}
				if (!flexibleHeightEnabled.IsNone)
				{
					flexibleHeightEnabled.Value = layoutElement.flexibleHeight != 0f;
				}
				if (!flexibleHeight.IsNone)
				{
					flexibleHeight.Value = layoutElement.flexibleHeight;
				}
			}
		}
	}
}
