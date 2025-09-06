using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("GUI base action - don't use!")]
	public abstract class GUIContentAction : GUIAction
	{
		[Tooltip("Optional image to display.")]
		public FsmTexture image;

		[Tooltip("Optional text to display.")]
		public FsmString text;

		[Tooltip("Optional tooltip. Accessed by {{GUI Tooltip}}")]
		public FsmString tooltip;

		[Tooltip("Optional named style in the current GUISkin")]
		public FsmString style;

		internal GUIContent content;

		public override void Reset()
		{
			base.Reset();
			image = null;
			text = "";
			tooltip = "";
			style = "";
		}

		public override void OnGUI()
		{
			base.OnGUI();
			content = new GUIContent(text.Value, image.Value, tooltip.Value);
		}
	}
}
