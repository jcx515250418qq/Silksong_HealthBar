using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Get the angle from Game Object to the target. 0 is right, 90 is up etc.")]
	public class GetAngleToTarget2D : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmGameObject target;

		public FsmFloat offsetX;

		public FsmFloat offsetY;

		[RequiredField]
		public FsmFloat storeAngle;

		public FsmFloat pause;

		public bool everyFrame;

		private FsmGameObject self;

		private FsmFloat x;

		private FsmFloat y;

		private float timer;

		private bool didGetAngle;

		public override void Reset()
		{
			gameObject = null;
			target = null;
			offsetX = null;
			offsetY = null;
			storeAngle = null;
			everyFrame = false;
			pause = null;
		}

		public override void OnEnter()
		{
			if (!pause.IsNone)
			{
				timer = pause.Value;
			}
			else
			{
				timer = 0f;
			}
			self = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (offsetX.IsNone)
			{
				offsetX.Value = 0f;
			}
			if (offsetY.IsNone)
			{
				offsetY.Value = 0f;
			}
			if (timer == 0f)
			{
				DoGetAngle();
			}
		}

		public override void OnUpdate()
		{
			if (timer > 0f)
			{
				timer -= Time.deltaTime;
				if (timer < 0f)
				{
					timer = 0f;
				}
			}
			else
			{
				DoGetAngle();
			}
		}

		private void DoGetAngle()
		{
			if (!(target.Value == null))
			{
				float num = target.Value.transform.position.y + offsetY.Value - self.Value.transform.position.y;
				float num2 = target.Value.transform.position.x + offsetX.Value - self.Value.transform.position.x;
				float num3;
				for (num3 = Mathf.Atan2(num, num2) * (180f / MathF.PI); num3 < 0f; num3 += 360f)
				{
				}
				storeAngle.Value = num3;
				if (!everyFrame)
				{
					Finish();
				}
				didGetAngle = true;
			}
		}

		public override void OnExit()
		{
			if (!didGetAngle)
			{
				DoGetAngle();
			}
		}
	}
}
