using HutongGames.PlayMaker;
using UnityEngine;

public class CheckIsCharacterGrounded : FsmStateAction
{
	[CheckForComponent(typeof(Collider2D))]
	public FsmOwnerDefault Target;

	public FsmInt RayCount;

	public FsmFloat GroundDistance;

	public FsmFloat SkinWidth;

	public FsmFloat SkinHeight;

	[UIHint(UIHint.Variable)]
	public FsmBool StoreResult;

	public FsmEvent GroundedEvent;

	public FsmEvent NotGroundedEvent;

	public bool EveryFrame;

	private Collider2D collider;

	private Rigidbody2D body;

	private bool hasCollider;

	private bool hasRB;

	private int layerMask;

	private bool isHero;

	private HeroController hero;

	public override void Reset()
	{
		Target = null;
		RayCount = new FsmInt(3);
		GroundDistance = new FsmFloat(0.2f);
		SkinWidth = new FsmFloat(-0.05f);
		SkinHeight = new FsmFloat(0.1f);
		StoreResult = null;
		GroundedEvent = null;
		NotGroundedEvent = null;
		EveryFrame = false;
	}

	public override void OnEnter()
	{
		GameObject safe = Target.GetSafe(this);
		if ((bool)safe)
		{
			collider = safe.GetComponent<Collider2D>();
			hasCollider = collider;
			body = safe.GetComponent<Rigidbody2D>();
			hasRB = body != null;
			if (!hasRB && hasCollider)
			{
				body = collider.attachedRigidbody;
				hasRB = body != null;
			}
			hero = safe.GetComponent<HeroController>();
			isHero = hero;
			layerMask = 256;
			DoAction();
			if (!EveryFrame)
			{
				Finish();
			}
		}
	}

	public override void OnUpdate()
	{
		DoAction();
	}

	private void DoAction()
	{
		if (hasRB && body.linearVelocity.y > 0.01f)
		{
			base.Fsm.Event(NotGroundedEvent);
			return;
		}
		bool flag = IsGrounded();
		StoreResult.Value = flag;
		base.Fsm.Event(flag ? GroundedEvent : NotGroundedEvent);
	}

	private bool IsGrounded()
	{
		if (isHero)
		{
			return hero.CheckTouchingGround();
		}
		if (!hasCollider || !collider.enabled)
		{
			return false;
		}
		float value = SkinWidth.Value;
		float value2 = SkinHeight.Value;
		float length = (hasRB ? Mathf.Max(GroundDistance.Value, (0f - body.linearVelocity.y) * Time.deltaTime) : GroundDistance.Value) + value2;
		Bounds bounds = collider.bounds;
		Vector2 vector = bounds.min;
		float a = vector.x + value;
		float b = bounds.max.x - value;
		float y = vector.y + value2;
		int num = RayCount.Value - 1;
		for (int i = 0; i < RayCount.Value; i++)
		{
			if (Helper.IsRayHittingNoTriggers(new Vector2(Mathf.Lerp(a, b, (float)i / (float)num), y), Vector2.down, length, layerMask))
			{
				return true;
			}
		}
		return false;
	}
}
