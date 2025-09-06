using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the isOn property of a UI Toggle component.")]
	public class UiToggleSetIsOn : ComponentAction<Toggle>
	{
		[RequiredField]
		[CheckForComponent(typeof(Toggle))]
		[Tooltip("The GameObject with the UI Toggle component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("Should the toggle be on?")]
		public FsmBool isOn;

		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		private Toggle _toggle;

		private bool _originalValue;

		public override void Reset()
		{
			gameObject = null;
			isOn = null;
			resetOnExit = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				_toggle = cachedComponent;
			}
			DoSetValue();
			Finish();
		}

		private void DoSetValue()
		{
			if (_toggle != null)
			{
				_originalValue = _toggle.isOn;
				_toggle.isOn = isOn.Value;
			}
		}

		public override void OnExit()
		{
			if (!(_toggle == null) && resetOnExit.Value)
			{
				_toggle.isOn = _originalValue;
			}
		}
	}
}
