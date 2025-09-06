using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("Resets the GUI matrix. Useful if you've rotated or scaled the GUI and now want to reset it.")]
	public class ResetGUIMatrix : FsmStateAction
	{
		public override void OnGUI()
		{
			PlayMakerGUI.GUIMatrix = (GUI.matrix = Matrix4x4.identity);
		}
	}
}
