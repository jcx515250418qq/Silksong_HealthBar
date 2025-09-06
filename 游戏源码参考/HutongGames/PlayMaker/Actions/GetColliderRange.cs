using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	[Tooltip("Returns the X/Y Min and max bounds for a box2d collider (in world space)")]
	public class GetColliderRange : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmFloat minX;

		[UIHint(UIHint.Variable)]
		public FsmFloat maxX;

		[UIHint(UIHint.Variable)]
		public FsmFloat minY;

		[UIHint(UIHint.Variable)]
		public FsmFloat maxY;

		public bool everyFrame;

		private BoxCollider2D box;

		public override void Reset()
		{
			gameObject = null;
			minX = null;
			maxX = null;
			minY = null;
			maxY = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			box = gameObject.GetSafe<BoxCollider2D>(this);
			bool num = box != null;
			if (num)
			{
				GetRange();
			}
			if (!num || !everyFrame)
			{
				Finish();
			}
		}

		public void GetRange()
		{
			UnityEngine.Bounds bounds = box.bounds;
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;
			minY.Value = min.y;
			maxY.Value = max.y;
			minX.Value = min.x;
			maxX.Value = max.x;
		}

		public override void OnUpdate()
		{
			GetRange();
		}
	}
}
