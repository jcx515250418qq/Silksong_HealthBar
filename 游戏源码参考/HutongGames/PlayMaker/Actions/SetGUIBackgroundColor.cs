using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("Sets the Tinting Color for all background elements rendered by the GUI. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
	public class SetGUIBackgroundColor : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The color for all background elements.")]
		public FsmColor backgroundColor;

		[Tooltip("Apply this setting to all GUI calls, even in other scripts.")]
		public FsmBool applyGlobally;

		public override void Reset()
		{
			backgroundColor = Color.white;
		}

		public override void OnGUI()
		{
			GUI.backgroundColor = backgroundColor.Value;
			if (applyGlobally.Value)
			{
				PlayMakerGUI.GUIBackgroundColor = GUI.backgroundColor;
			}
		}
	}
}
