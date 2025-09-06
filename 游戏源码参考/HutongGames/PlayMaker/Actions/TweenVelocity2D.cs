using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class TweenVelocity2D : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault Target;

		public FsmVector2 FromVelocityVector;

		[HideIf("ShowFromComponents")]
		public FsmFloat FromVelocityX;

		[HideIf("ShowFromComponents")]
		public FsmFloat FromVelocityY;

		public FsmVector2 ToVelocityVector;

		[HideIf("ShowToComponents")]
		public FsmFloat ToVelocityX;

		[HideIf("ShowToComponents")]
		public FsmFloat ToVelocityY;

		public FsmAnimationCurve Curve;

		public FsmFloat Duration;

		private float elapsed;

		private Rigidbody2D body;

		public bool ShowFromComponents()
		{
			return !FromVelocityVector.IsNone;
		}

		public bool ShowToComponents()
		{
			return !ToVelocityVector.IsNone;
		}

		public override void Reset()
		{
			Target = null;
			FromVelocityVector = null;
			FromVelocityX = null;
			FromVelocityY = null;
			ToVelocityVector = null;
			ToVelocityX = null;
			ToVelocityY = null;
			Curve = new FsmAnimationCurve
			{
				curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f)
			};
			Duration = 1f;
		}

		public override void Awake()
		{
			OnPreprocess();
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (!safe)
			{
				Finish();
				return;
			}
			body = safe.GetComponent<Rigidbody2D>();
			if (!body)
			{
				Finish();
				return;
			}
			elapsed = 0f;
			DoSetVelocity();
		}

		public override void OnFixedUpdate()
		{
			elapsed += Time.fixedDeltaTime;
			DoSetVelocity();
		}

		private void DoSetVelocity()
		{
			if (elapsed > Duration.Value)
			{
				SetVelocity(1f);
				Finish();
			}
			else
			{
				float velocity = Curve.curve.Evaluate(elapsed / Duration.Value);
				SetVelocity(velocity);
			}
		}

		private void SetVelocity(float curveTime)
		{
			Vector2 a = CollapseVectorVariables(FromVelocityVector, FromVelocityX, FromVelocityY);
			Vector2 b = CollapseVectorVariables(ToVelocityVector, ToVelocityX, ToVelocityY);
			Vector2 linearVelocity = Vector2.Lerp(a, b, curveTime);
			if (ToVelocityVector.IsNone)
			{
				Vector2 linearVelocity2 = body.linearVelocity;
				if (ToVelocityX.IsNone)
				{
					linearVelocity.x = linearVelocity2.x;
				}
				if (ToVelocityY.IsNone)
				{
					linearVelocity.y = linearVelocity2.y;
				}
			}
			body.linearVelocity = linearVelocity;
		}

		private Vector2 CollapseVectorVariables(FsmVector2 vector, FsmFloat x, FsmFloat y)
		{
			if (!vector.IsNone)
			{
				return vector.Value;
			}
			return new Vector2(x.Value, y.Value);
		}
	}
}
