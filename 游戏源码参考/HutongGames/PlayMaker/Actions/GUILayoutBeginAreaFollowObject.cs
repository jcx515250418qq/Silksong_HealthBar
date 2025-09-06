using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("Begin a GUILayout area that follows the specified game object. Useful for overlays (e.g., playerName). NOTE: Block must end with a corresponding GUILayoutEndArea.")]
	public class GUILayoutBeginAreaFollowObject : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to follow.")]
		public FsmGameObject gameObject;

		[RequiredField]
		[Tooltip("Left screen offset.")]
		public FsmFloat offsetLeft;

		[RequiredField]
		[Tooltip("Screen offset up.")]
		public FsmFloat offsetTop;

		[RequiredField]
		[Tooltip("Width of area.")]
		public FsmFloat width;

		[RequiredField]
		[Tooltip("Height of area.")]
		public FsmFloat height;

		[Tooltip("Use normalized screen coordinates (0-1).")]
		public FsmBool normalized;

		[Tooltip("Optional named style in the current GUISkin")]
		public FsmString style;

		public override void Reset()
		{
			gameObject = null;
			offsetLeft = 0f;
			offsetTop = 0f;
			width = 1f;
			height = 1f;
			normalized = true;
			style = "";
		}

		public override void OnGUI()
		{
			GameObject value = gameObject.Value;
			if (value == null || Camera.main == null)
			{
				DummyBeginArea();
				return;
			}
			Vector3 position = value.transform.position;
			if (Camera.main.transform.InverseTransformPoint(position).z < 0f)
			{
				DummyBeginArea();
				return;
			}
			Vector2 vector = Camera.main.WorldToScreenPoint(position);
			float x = vector.x + (normalized.Value ? (offsetLeft.Value * (float)Screen.width) : offsetLeft.Value);
			float y = vector.y + (normalized.Value ? (offsetTop.Value * (float)Screen.width) : offsetTop.Value);
			Rect screenRect = new Rect(x, y, width.Value, height.Value);
			if (normalized.Value)
			{
				screenRect.width *= Screen.width;
				screenRect.height *= Screen.height;
			}
			screenRect.y = (float)Screen.height - screenRect.y;
			GUILayout.BeginArea(screenRect, style.Value);
		}

		private static void DummyBeginArea()
		{
			GUILayout.BeginArea(default(Rect));
		}
	}
}
