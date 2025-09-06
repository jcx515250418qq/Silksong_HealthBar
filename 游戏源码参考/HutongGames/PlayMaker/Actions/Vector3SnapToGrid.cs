using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Snap Vector3 coordinates to grid points.")]
	public class Vector3SnapToGrid : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Vector3 Variable to snap.")]
		public FsmVector3 vector3Variable;

		[Tooltip("Grid Size.")]
		public FsmFloat gridSize;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			vector3Variable = null;
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
				Vector3 value = vector3Variable.Value;
				float value2 = gridSize.Value;
				value /= value2;
				value.Set(Mathf.Round(value.x), Mathf.Round(value.y), Mathf.Round(value.z));
				vector3Variable.Value = value * value2;
			}
		}
	}
}
