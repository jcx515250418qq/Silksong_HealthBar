using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sends an event when a UI Button is clicked.")]
	public class UiButtonOnClickEvent : ComponentAction<Button>
	{
		[RequiredField]
		[CheckForComponent(typeof(Button))]
		[Tooltip("The GameObject with the UI Button component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		[Tooltip("Send this event when Clicked.")]
		public FsmEvent sendEvent;

		private Button button;

		public override void Reset()
		{
			gameObject = null;
			sendEvent = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				if (button != null)
				{
					button.onClick.RemoveListener(DoOnClick);
				}
				button = cachedComponent;
				if (button != null)
				{
					button.onClick.AddListener(DoOnClick);
				}
				else
				{
					LogError("Missing UI.Button on " + ownerDefaultTarget.name);
				}
			}
			else
			{
				LogError("Missing GameObject ");
			}
		}

		public override void OnExit()
		{
			if (button != null)
			{
				button.onClick.RemoveListener(DoOnClick);
			}
		}

		public void DoOnClick()
		{
			SendEvent(eventTarget, sendEvent);
		}
	}
}
