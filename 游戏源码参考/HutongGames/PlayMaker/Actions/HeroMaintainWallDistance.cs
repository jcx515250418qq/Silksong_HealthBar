using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public sealed class HeroMaintainWallDistance : FsmStateAction
	{
		public FsmFloat lowPointX;

		public FsmFloat highPointX;

		public FsmFloat direction;

		public FsmFloat distance;

		public FsmBool everyFixedUpdate;

		private HeroController hc;

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void Reset()
		{
			lowPointX = null;
			highPointX = null;
			direction = 1f;
			distance = 0.005f;
			everyFixedUpdate = false;
		}

		public override void OnEnter()
		{
			hc = HeroController.instance;
			if (hc != null)
			{
				MaintainWallDistance();
			}
			if (!everyFixedUpdate.Value || hc == null)
			{
				Finish();
			}
		}

		public override void OnFixedUpdate()
		{
			if (hc != null)
			{
				MaintainWallDistance();
			}
			else
			{
				Finish();
			}
		}

		private void MaintainWallDistance()
		{
			float num = Mathf.Sign(direction.Value);
			float num2 = lowPointX.Value;
			float num3 = highPointX.Value;
			if (lowPointX.IsNone && highPointX.IsNone)
			{
				Vector2 origin = hc.transform.position;
				Vector2 vector = new Vector2(num, 0f);
				RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, vector);
				if (!raycastHit2D)
				{
					return;
				}
				num2 = raycastHit2D.point.x;
				num3 = raycastHit2D.point.x;
			}
			UnityEngine.Bounds bounds = hc.Bounds;
			if (num > 0f)
			{
				float num4 = float.MinValue;
				if (!lowPointX.IsNone)
				{
					num4 = num2;
				}
				if (!highPointX.IsNone && num4 > num3)
				{
					num4 = num3;
				}
				num4 -= distance.Value;
				if (bounds.max.x > num4)
				{
					float x = num4 - bounds.max.x;
					hc.Body.MovePosition(hc.transform.position + new Vector3(x, 0f));
				}
			}
			else
			{
				float num5 = float.MaxValue;
				if (!lowPointX.IsNone)
				{
					num5 = num2;
				}
				if (!highPointX.IsNone && num5 < num3)
				{
					num5 = num3;
				}
				num5 += distance.Value;
				if (bounds.min.x < num5)
				{
					float x2 = num5 - bounds.min.x;
					hc.Body.MovePosition(hc.transform.position + new Vector3(x2, 0f));
				}
			}
		}
	}
}
