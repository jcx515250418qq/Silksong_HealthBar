using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("Scales the GUI around a pivot point. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
	public class ScaleGUI : FsmStateAction
	{
		[RequiredField]
		[Tooltip("Scale in x (1 = 100%)")]
		public FsmFloat scaleX;

		[RequiredField]
		[Tooltip("Scale in y (1 = 100%)")]
		public FsmFloat scaleY;

		[RequiredField]
		[Tooltip("Scale around this x screen coordinate.")]
		public FsmFloat pivotX;

		[RequiredField]
		[Tooltip("Scale around this y screen coordinate.")]
		public FsmFloat pivotY;

		[Tooltip("Pivot point uses normalized coordinates (0-1). E.g. 0.5 is the center of the screen.")]
		public bool normalized;

		[Tooltip("Apply to all GUI actions in all FSMs.")]
		public bool applyGlobally;

		private bool applied;

		public override void Reset()
		{
			scaleX = 1f;
			scaleY = 1f;
			pivotX = 0.5f;
			pivotY = 0.5f;
			normalized = true;
			applyGlobally = false;
		}

		public override void OnGUI()
		{
			if (!applied)
			{
				Vector2 scale = new Vector2(scaleX.Value, scaleY.Value);
				if (object.Equals(scale.x, 0))
				{
					scale.x = 0.0001f;
				}
				if (object.Equals(scale.y, 0))
				{
					scale.x = 0.0001f;
				}
				Vector2 pivotPoint = new Vector2(pivotX.Value, pivotY.Value);
				if (normalized)
				{
					pivotPoint.x *= Screen.width;
					pivotPoint.y *= Screen.height;
				}
				GUIUtility.ScaleAroundPivot(scale, pivotPoint);
				if (applyGlobally)
				{
					PlayMakerGUI.GUIMatrix = GUI.matrix;
					applied = true;
				}
			}
		}

		public override void OnUpdate()
		{
			applied = false;
		}
	}
}
