using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("GUI Label.")]
	public class GUILabel : GUIContentAction
	{
		public override void OnGUI()
		{
			base.OnGUI();
			if (string.IsNullOrEmpty(style.Value))
			{
				GUI.Label(rect, content);
			}
			else
			{
				GUI.Label(rect, content, style.Value);
			}
		}
	}
}
