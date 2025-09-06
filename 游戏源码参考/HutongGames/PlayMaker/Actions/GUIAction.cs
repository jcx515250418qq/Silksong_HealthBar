using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("GUI base action - don't use!")]
	public abstract class GUIAction : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[Tooltip("Screen rectangle.")]
		public FsmRect screenRect;

		[Tooltip("Left coordinate of screen rectangle.")]
		public FsmFloat left;

		[Tooltip("Top coordinate of screen rectangle.")]
		public FsmFloat top;

		[Tooltip("Width of screen rectangle.")]
		public FsmFloat width;

		[Tooltip("Height of screen rectangle.")]
		public FsmFloat height;

		[RequiredField]
		[Tooltip("Use normalized screen coordinates (0-1).")]
		public FsmBool normalized;

		internal Rect rect;

		public override void Reset()
		{
			screenRect = null;
			left = 0f;
			top = 0f;
			width = 1f;
			height = 1f;
			normalized = true;
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
		}
	}
}
