using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Text Field. Optionally send an event if the text has been edited.")]
	public class GUILayoutTextField : GUILayoutAction
	{
		[UIHint(UIHint.Variable)]
		[Tooltip("Link the text field to a String Variable.")]
		public FsmString text;

		[Tooltip("The max number of characters that can be entered.")]
		public FsmInt maxLength;

		[Tooltip("Optional named style in the current GUISkin")]
		public FsmString style;

		[Tooltip("An optional Event to send when the text field value changes.")]
		public FsmEvent changedEvent;

		public override void Reset()
		{
			base.Reset();
			text = null;
			maxLength = 25;
			style = "TextField";
			changedEvent = null;
		}

		public override void OnGUI()
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			text.Value = GUILayout.TextField(text.Value, maxLength.Value, style.Value, base.LayoutOptions);
			if (GUI.changed)
			{
				base.Fsm.Event(changedEvent);
				GUIUtility.ExitGUI();
			}
			else
			{
				GUI.changed = changed;
			}
		}
	}
}
