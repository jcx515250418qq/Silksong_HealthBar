using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("GUI Box.")]
	public class GUIBox : GUIContentAction
	{
		public override void OnGUI()
		{
			base.OnGUI();
			if (string.IsNullOrEmpty(style.Value))
			{
				GUI.Box(rect, content);
			}
			else
			{
				GUI.Box(rect, content, style.Value);
			}
		}
	}
}
