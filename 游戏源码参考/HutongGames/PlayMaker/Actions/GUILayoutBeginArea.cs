using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("Begin a GUILayout block of GUI controls in a fixed screen area. NOTE: Block must end with a corresponding GUILayoutEndArea.")]
	public class GUILayoutBeginArea : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[Tooltip("Screen area.")]
		public FsmRect screenRect;

		[Tooltip("Left screen coordinate.")]
		public FsmFloat left;

		[Tooltip("Top screen coordinate.")]
		public FsmFloat top;

		[Tooltip("Width of area.")]
		public FsmFloat width;

		[Tooltip("Height of area.")]
		public FsmFloat height;

		[Tooltip("Use normalized screen coordinates (0-1).")]
		public FsmBool normalized;

		[Tooltip("Optional GUIStyle name in current GUISkin.")]
		public FsmString style;

		private Rect rect;

		public override void Reset()
		{
			screenRect = null;
			left = 0f;
			top = 0f;
			width = 1f;
			height = 1f;
			normalized = true;
			style = "";
		}

		public override void OnGUI()
		{
			rect = ((!screenRect.IsNone) ? screenRect.Value : default(Rect));
			if (!left.IsNone)
			{
				rect.x = left.Value;
			}
			if (!top.IsNone)
			{
				rect.y = top.Value;
			}
			if (!width.IsNone)
			{
				rect.width = width.Value;
			}
			if (!height.IsNone)
			{
				rect.height = height.Value;
			}
			if (normalized.Value)
			{
				rect.x *= Screen.width;
				rect.width *= Screen.width;
				rect.y *= Screen.height;
				rect.height *= Screen.height;
			}
			GUILayout.BeginArea(rect, GUIContent.none, style.Value);
		}
	}
}
