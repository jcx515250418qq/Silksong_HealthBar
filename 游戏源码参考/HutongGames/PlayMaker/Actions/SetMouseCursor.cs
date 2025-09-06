using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("Controls the appearance of Mouse Cursor.")]
	public class SetMouseCursor : FsmStateAction
	{
		[Tooltip("The texture to use for the cursor.")]
		public FsmTexture cursorTexture;

		[Tooltip("Hide the cursor.")]
		public FsmBool hideCursor;

		[Tooltip("Lock the cursor to the center of the screen. Useful in first person controllers.")]
		public FsmBool lockCursor;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			cursorTexture = null;
			hideCursor = false;
			lockCursor = false;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			PlayMakerGUI.LockCursor = lockCursor.Value;
			PlayMakerGUI.HideCursor = hideCursor.Value;
			PlayMakerGUI.MouseCursor = cursorTexture.Value;
			UpdateCursorState();
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void UpdateCursorState()
		{
			Cursor.lockState = (lockCursor.Value ? CursorLockMode.Locked : CursorLockMode.None);
			Cursor.visible = !hideCursor.Value;
		}

		public override void OnUpdate()
		{
			UpdateCursorState();
		}

		public override void OnGUI()
		{
			if (!PlayMakerGUI.Exists)
			{
				Texture value = cursorTexture.Value;
				if (value != null)
				{
					Vector3 mousePosition = ActionHelpers.GetMousePosition();
					GUI.DrawTexture(new Rect(mousePosition.x - (float)value.width * 0.5f, (float)Screen.height - mousePosition.y - (float)value.height * 0.5f, value.width, value.height), value);
				}
			}
		}
	}
}
