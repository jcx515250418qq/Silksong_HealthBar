using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets pointer data Input Button on the last System event.")]
	public class UiGetLastPointerEventDataInputButton : FsmStateAction
	{
		[Tooltip("Store the Input Button pressed (Left, Right, Middle)")]
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(PointerEventData.InputButton))]
		public FsmEnum inputButton;

		[Tooltip("Event to send if Left Button clicked.")]
		public FsmEvent leftClick;

		[Tooltip("Event to send if Middle Button clicked.")]
		public FsmEvent middleClick;

		[Tooltip("Event to send if Right Button clicked.")]
		public FsmEvent rightClick;

		public override void Reset()
		{
			inputButton = PointerEventData.InputButton.Left;
			leftClick = null;
			middleClick = null;
			rightClick = null;
		}

		public override void OnEnter()
		{
			ExecuteAction();
			Finish();
		}

		private void ExecuteAction()
		{
			if (UiGetLastPointerDataInfo.lastPointerEventData != null)
			{
				if (!inputButton.IsNone)
				{
					inputButton.Value = UiGetLastPointerDataInfo.lastPointerEventData.button;
				}
				if (UiGetLastPointerDataInfo.lastPointerEventData.button == PointerEventData.InputButton.Left)
				{
					base.Fsm.Event(leftClick);
				}
				if (UiGetLastPointerDataInfo.lastPointerEventData.button == PointerEventData.InputButton.Middle)
				{
					base.Fsm.Event(middleClick);
				}
				if (UiGetLastPointerDataInfo.lastPointerEventData.button == PointerEventData.InputButton.Right)
				{
					base.Fsm.Event(rightClick);
				}
			}
		}
	}
}
