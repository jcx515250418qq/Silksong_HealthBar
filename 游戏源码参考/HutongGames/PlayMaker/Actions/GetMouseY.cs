using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Gets the Y Position of the mouse and stores it in a Float Variable.")]
	public class GetMouseY : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store in a float variable.")]
		public FsmFloat storeResult;

		[Tooltip("Normalized coordinates are in the range 0 to 1 (0 = left, 1 = right). Otherwise the coordinate is in pixels. Normalized coordinates are useful for resolution independent functions.")]
		public bool normalize;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			storeResult = null;
			normalize = true;
			everyFrame = true;
		}

		public override void OnEnter()
		{
			DoGetMouseY();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetMouseY();
		}

		private void DoGetMouseY()
		{
			if (storeResult != null)
			{
				float num = Input.mousePosition.y;
				if (normalize)
				{
					num /= (float)Screen.height;
				}
				storeResult.Value = num;
			}
		}
	}
}
