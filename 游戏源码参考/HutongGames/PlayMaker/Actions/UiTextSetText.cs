using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the text value of a UI Text component.")]
	public class UiTextSetText : ComponentAction<Text>
	{
		[RequiredField]
		[CheckForComponent(typeof(Text))]
		[Tooltip("The GameObject with the UI Text component.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.TextArea)]
		[Tooltip("The text of the UI Text component.")]
		public FsmString text;

		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		private Text uiText;

		private string originalString;

		public override void Reset()
		{
			gameObject = null;
			text = null;
			resetOnExit = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				uiText = cachedComponent;
			}
			originalString = uiText.text;
			DoSetTextValue();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetTextValue();
		}

		private void DoSetTextValue()
		{
			if (!(uiText == null))
			{
				uiText.text = text.Value;
			}
		}

		public override void OnExit()
		{
			if (!(uiText == null) && resetOnExit.Value)
			{
				uiText.text = originalString;
			}
		}
	}
}
