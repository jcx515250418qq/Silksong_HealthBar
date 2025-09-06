using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Object rotates to face direction it is travelling in.")]
	public class FaceAngleV3 : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault GameObject;

		public FsmFloat AngleOffset;

		public FsmBool WorldSpace;

		public FsmBool SpriteFacesRight;

		public bool EveryFrame;

		public bool skipOnEnter;

		private FsmGameObject target;

		private Transform transform;

		private Rigidbody2D body;

		public override void Reset()
		{
			GameObject = null;
			AngleOffset = 0f;
			EveryFrame = false;
			SpriteFacesRight = null;
		}

		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			target = base.Fsm.GetOwnerDefaultTarget(GameObject);
			transform = target.Value.transform;
			body = target.Value.GetComponent<Rigidbody2D>();
			if (!skipOnEnter)
			{
				DoAngle();
			}
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnFixedUpdate()
		{
			DoAngle();
		}

		private void DoAngle()
		{
			if (!(body.linearVelocity.magnitude < Mathf.Epsilon))
			{
				Vector2 normalized = body.linearVelocity.normalized;
				float num = AngleOffset.Value;
				if (SpriteFacesRight.Value)
				{
					num += 180f;
				}
				if (normalized.x < 0f)
				{
					num += 180f;
				}
				float num2 = Mathf.Atan2(normalized.y, normalized.x) * 57.29578f;
				num2 += num;
				if (WorldSpace.Value)
				{
					target.Value.transform.eulerAngles = new Vector3(0f, 0f, num2);
				}
				else
				{
					target.Value.transform.localEulerAngles = new Vector3(0f, 0f, num2);
				}
			}
		}
	}
}
