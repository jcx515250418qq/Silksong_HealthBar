using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	public class Swoop : FsmStateAction
	{
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault Swooper;

		private Transform transform;

		private Rigidbody2D body;

		public FsmGameObject Target;

		private Transform target;

		[HideIf("IsTargetSpecified")]
		public FsmVector2 TargetPosition;

		public FsmFloat Speed;

		private float duration;

		private float elapsed;

		public FsmAnimationCurve SwoopCurveX;

		public FsmAnimationCurve SwoopCurveY;

		public FsmEvent EndEvent;

		private Vector2 startPosition;

		private Vector2 targetPosition;

		public override void Reset()
		{
			Swooper = null;
			Target = null;
			TargetPosition = null;
			Speed = null;
			SwoopCurveX = null;
			SwoopCurveY = null;
			EndEvent = null;
		}

		public bool IsTargetSpecified()
		{
			return !Target.IsNone;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void Awake()
		{
			OnPreprocess();
		}

		public override void OnEnter()
		{
			if ((bool)Target.Value)
			{
				target = Target.Value.transform;
			}
			GameObject safe = Swooper.GetSafe(this);
			if ((bool)safe)
			{
				transform = safe.transform;
				body = safe.GetComponent<Rigidbody2D>();
			}
			if (body == null || Speed.Value <= 0f)
			{
				End();
				return;
			}
			elapsed = 0f;
			startPosition = transform.position;
			targetPosition = (target ? ((Vector2)target.position) : TargetPosition.Value);
			duration = Vector2.Distance(startPosition, targetPosition) / Speed.Value;
			Evaluate(0f);
		}

		public override void OnFixedUpdate()
		{
			if (elapsed < duration)
			{
				float t = elapsed / duration;
				Evaluate(t);
			}
			else
			{
				End();
			}
			elapsed += Time.fixedDeltaTime;
		}

		private void Evaluate(float t)
		{
			Vector2 vector = transform.position;
			float t2 = SwoopCurveX.curve.Evaluate(t);
			float t3 = SwoopCurveY.curve.Evaluate(t);
			float x = Mathf.LerpUnclamped(startPosition.x, targetPosition.x, t2);
			float y = Mathf.LerpUnclamped(startPosition.y, targetPosition.y, t3);
			Vector2 linearVelocity = (new Vector2(x, y) - vector) / Time.fixedDeltaTime;
			body.linearVelocity = linearVelocity;
		}

		private void End()
		{
			base.Fsm.Event(EndEvent);
			Finish();
		}
	}
}
