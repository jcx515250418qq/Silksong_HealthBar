using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Snap Vector2 coordinates to grid points.")]
	public class Vector2SnapToGrid : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Vector2 Variable to snap.")]
		public FsmVector2 vector2Variable;

		[Tooltip("Grid Size.")]
		public FsmFloat gridSize;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			vector2Variable = null;
			gridSize = new FsmFloat
			{
				Value = 1f
			};
		}

		public override void OnEnter()
		{
			DoSnapToGrid();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSnapToGrid();
		}

		private void DoSnapToGrid()
		{
			if (!(gridSize.Value < 0.001f))
			{
				Vector2 value = vector2Variable.Value;
				float value2 = gridSize.Value;
				value /= value2;
				value.Set(Mathf.Round(value.x), Mathf.Round(value.y));
				vector2Variable.Value = value * value2;
			}
		}
	}
}
