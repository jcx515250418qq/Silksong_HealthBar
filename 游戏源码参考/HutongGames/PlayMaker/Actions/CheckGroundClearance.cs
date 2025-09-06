using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CheckGroundClearance : FsmStateAction
	{
		private const int LAYER_MASK = 256;

		private const float GROUND_PADDING = 0.3f;

		public FsmOwnerDefault RelativeTo;

		public FsmVector2 Position;

		[RequiredField]
		public FsmFloat GroundWidth;

		public FsmEvent IsClearEvent;

		public FsmEvent NotClearEvent;

		public override void Reset()
		{
			RelativeTo = null;
			Position = null;
			GroundWidth = null;
			IsClearEvent = null;
			NotClearEvent = null;
		}

		public override void OnEnter()
		{
			base.Fsm.Event(IsClear() ? IsClearEvent : NotClearEvent);
			Finish();
		}

		private bool IsClear()
		{
			GameObject safe = RelativeTo.GetSafe(this);
			RaycastHit2D raycastHit2D = Helper.Raycast2D(safe ? ((Vector2)safe.transform.TransformPoint(Position.Value)) : Position.Value, Vector2.down, 10f, 256);
			if (!raycastHit2D)
			{
				return false;
			}
			Vector2 point = raycastHit2D.point;
			point.y += 0.3f;
			float length = GroundWidth.Value / 2f;
			if (!IsClearDirectional(point, -1f, length, 256))
			{
				return false;
			}
			if (!IsClearDirectional(point, 1f, length, 256))
			{
				return false;
			}
			return true;
		}

		private static bool IsClearDirectional(Vector2 origin, float direction, float length, int layerMask)
		{
			Vector2 vector = new Vector2(direction, 0f);
			if ((bool)Helper.Raycast2D(origin, vector, length, layerMask))
			{
				return false;
			}
			while (length > 0f)
			{
				if (!Helper.Raycast2D(origin + vector * length, Vector2.down, 0.6f, 256))
				{
					return false;
				}
				length -= 0.5f;
			}
			return true;
		}
	}
}
