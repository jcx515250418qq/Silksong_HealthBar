using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Climber : MonoBehaviour
{
	public enum WallBehaviours
	{
		Continue = 0,
		Turn = 1
	}

	public enum Direction
	{
		Right = 0,
		Down = 1,
		Left = 2,
		Up = 3
	}

	public string WalkAnim = "Walk";

	public string StunAnim = "Stun";

	[Space]
	public WallBehaviours WallBehaviour;

	public WallBehaviours EdgeBehaviour;

	[ModifiableProperty]
	[InspectorValidation("DoesAnimExist")]
	public string ContinueConvexRegularAnim;

	[ModifiableProperty]
	[InspectorValidation("DoesAnimExist")]
	public string ContinueConvexImmediateAnim;

	public GameObject ConvexDamagerOverride;

	[ModifiableProperty]
	[InspectorValidation("DoesAnimExist")]
	public string ContinueConcaveRegularAnim;

	[ModifiableProperty]
	[InspectorValidation("DoesAnimExist")]
	public string ContinueConcaveImmediateAnim;

	public GameObject ConcaveDamagerOverride;

	public bool RepositionBeforeTurn;

	public Vector2 RepositionRegularOffset;

	public Vector2 RepositionImmediateOffset;

	[Space]
	[ModifiableProperty]
	[InspectorValidation("DoesAnimExist")]
	public string TurnAroundAnim;

	public bool FlipBeforeTurn;

	public float FlipXOffset;

	[Space]
	[FormerlySerializedAs("startRight")]
	public bool SpriteFacesRight = true;

	private bool clockwise = true;

	public float speed = 2f;

	public float spinTime = 0.25f;

	[Space]
	public float wallRayPadding = 0.1f;

	[Space]
	public float minTurnDistance = 0.25f;

	private Vector2 previousTurnPos;

	private Direction currentDirection;

	private Coroutine turnRoutine;

	private YieldInstruction moveYield = new WaitForFixedUpdate();

	private Rigidbody2D body;

	private BoxCollider2D col;

	private tk2dSpriteAnimator anim;

	private Recoil recoil;

	private bool isFirstSetupDone;

	public bool IsTurning => turnRoutine != null;

	public bool IsFacingRight
	{
		get
		{
			bool flag = base.transform.localScale.x > 0f;
			if (!SpriteFacesRight)
			{
				flag = !flag;
			}
			return flag;
		}
	}

	public bool? DoesAnimExist(string animName)
	{
		if (string.IsNullOrEmpty(animName))
		{
			return null;
		}
		tk2dSpriteAnimator component = GetComponent<tk2dSpriteAnimator>();
		if (!component)
		{
			return false;
		}
		return component.GetClipByName(animName) != null;
	}

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		col = GetComponent<BoxCollider2D>();
		anim = GetComponent<tk2dSpriteAnimator>();
		recoil = GetComponent<Recoil>();
	}

	private void Start()
	{
		StickToGround();
		Setup();
		isFirstSetupDone = true;
	}

	private void OnEnable()
	{
		if (isFirstSetupDone)
		{
			Setup();
		}
	}

	private void Setup()
	{
		Vector3 localScale = base.transform.localScale;
		float num = Mathf.Sign(localScale.x);
		if (!SpriteFacesRight)
		{
			num *= -1f;
		}
		clockwise = num > 0f;
		currentDirection = GetCurrentMoveDirection(clockwise);
		if (localScale.y < 0f)
		{
			clockwise = !clockwise;
		}
		if ((bool)recoil)
		{
			recoil.SkipFreezingByController = true;
			recoil.OnHandleFreeze += Stun;
		}
		previousTurnPos = Vector2.zero;
		col.enabled = true;
		if ((bool)ConcaveDamagerOverride)
		{
			ConcaveDamagerOverride.SetActive(value: false);
		}
		if ((bool)ConvexDamagerOverride)
		{
			ConvexDamagerOverride.SetActive(value: false);
		}
		StartCoroutine(Walk());
	}

	public Direction GetCurrentMoveDirection(bool clockwise)
	{
		float num = base.transform.eulerAngles.z % 360f;
		if (num >= 45f && num <= 135f)
		{
			if (!clockwise)
			{
				return Direction.Down;
			}
			return Direction.Up;
		}
		if (num >= 135f && num <= 225f)
		{
			if (!clockwise)
			{
				return Direction.Right;
			}
			return Direction.Left;
		}
		if (num >= 225f && num <= 315f)
		{
			if (!clockwise)
			{
				return Direction.Up;
			}
			return Direction.Down;
		}
		if (!clockwise)
		{
			return Direction.Left;
		}
		return Direction.Right;
	}

	private void OnDisable()
	{
		turnRoutine = null;
		if ((bool)recoil)
		{
			recoil.OnHandleFreeze -= Stun;
		}
		StopAllCoroutines();
	}

	private IEnumerator Walk()
	{
		anim.Play(WalkAnim);
		body.linearVelocity = GetVelocity(currentDirection);
		while (true)
		{
			if (Vector3.Distance(previousTurnPos, base.transform.position) >= minTurnDistance)
			{
				if (CheckWall())
				{
					switch (WallBehaviour)
					{
					case WallBehaviours.Continue:
						yield return turnRoutine = StartCoroutine(TurnContinue(!clockwise, isImmediate: false, tweenPos: true));
						break;
					case WallBehaviours.Turn:
						yield return turnRoutine = StartCoroutine(TurnAround());
						break;
					}
				}
				else if (!CheckGround())
				{
					switch (EdgeBehaviour)
					{
					case WallBehaviours.Continue:
						yield return turnRoutine = StartCoroutine(TurnContinue(clockwise, isImmediate: false, tweenPos: false));
						break;
					case WallBehaviours.Turn:
						yield return turnRoutine = StartCoroutine(TurnAround());
						break;
					}
				}
			}
			yield return moveYield;
		}
	}

	private IEnumerator TurnContinue(bool turnClockwise, bool isImmediate, bool tweenPos)
	{
		body.linearVelocity = Vector2.zero;
		float currentRotation = base.transform.eulerAngles.z;
		float targetRotation = currentRotation + (float)(turnClockwise ? (-90) : 90);
		Vector2 currentPos = base.transform.position;
		Vector2 targetPos = currentPos + GetTweenPos(currentDirection);
		string turnAnimation = null;
		GameObject damager = null;
		bool flag = (turnClockwise && IsFacingRight) || (!turnClockwise && !IsFacingRight);
		if (base.transform.localScale.y < 0f)
		{
			flag = !flag;
		}
		if (flag)
		{
			string text = ((isImmediate && !string.IsNullOrEmpty(ContinueConvexImmediateAnim)) ? ContinueConvexImmediateAnim : ContinueConvexRegularAnim);
			if (!string.IsNullOrEmpty(text))
			{
				turnAnimation = text;
				damager = ConvexDamagerOverride;
			}
		}
		else if (!flag)
		{
			string text2 = ((isImmediate && !string.IsNullOrEmpty(ContinueConcaveImmediateAnim)) ? ContinueConcaveImmediateAnim : ContinueConcaveRegularAnim);
			if (!string.IsNullOrEmpty(text2))
			{
				turnAnimation = text2;
				damager = ConcaveDamagerOverride;
			}
		}
		if (turnAnimation == null)
		{
			for (float elapsed = 0f; elapsed < spinTime; elapsed += Time.deltaTime)
			{
				float t = elapsed / spinTime;
				base.transform.SetRotation2D(Mathf.Lerp(currentRotation, targetRotation, t));
				if (tweenPos)
				{
					base.transform.SetPosition2D(Vector2.Lerp(currentPos, targetPos, t));
				}
				yield return null;
			}
		}
		else if (!RepositionBeforeTurn)
		{
			yield return StartCoroutine(PlayCornerAnim(turnAnimation, damager));
		}
		base.transform.SetRotation2D(targetRotation);
		if (tweenPos)
		{
			base.transform.SetPosition2D(targetPos);
		}
		if (turnAnimation != null)
		{
			Vector3 vector = base.transform.TransformVector(RepositionRegularOffset);
			base.transform.position += vector;
			if (!CheckGround())
			{
				base.transform.position -= vector;
				vector = base.transform.TransformVector(RepositionImmediateOffset);
				base.transform.position += vector;
			}
			StickToGround();
			if (RepositionBeforeTurn)
			{
				yield return StartCoroutine(PlayCornerAnim(turnAnimation, damager));
			}
			anim.Play(WalkAnim);
		}
		int num = (int)currentDirection;
		num += (turnClockwise ? 1 : (-1));
		int num2 = Enum.GetNames(typeof(Direction)).Length;
		if (num < 0)
		{
			num = num2 - 1;
		}
		else if (num >= num2)
		{
			num = 0;
		}
		currentDirection = (Direction)num;
		body.linearVelocity = GetVelocity(currentDirection);
		turnRoutine = null;
	}

	private IEnumerator PlayCornerAnim(string animName, GameObject damager)
	{
		bool triggered = false;
		bool finished = false;
		anim.Play(animName);
		anim.AnimationEventTriggered = delegate
		{
			triggered = true;
		};
		anim.AnimationCompleted = delegate
		{
			finished = true;
		};
		yield return new WaitUntil(() => triggered);
		if ((bool)damager)
		{
			damager.SetActive(value: true);
			col.enabled = false;
		}
		yield return new WaitUntil(() => finished);
		if ((bool)damager)
		{
			col.enabled = true;
			damager.SetActive(value: false);
		}
		anim.AnimationEventTriggered = null;
		anim.AnimationCompleted = null;
	}

	private IEnumerator TurnAround()
	{
		body.linearVelocity = Vector2.zero;
		if (!string.IsNullOrEmpty(TurnAroundAnim))
		{
			if (FlipBeforeTurn)
			{
				FlipXScale();
			}
			yield return StartCoroutine(anim.PlayAnimWait(TurnAroundAnim));
			anim.Play(WalkAnim);
			if (!FlipBeforeTurn)
			{
				FlipXScale();
			}
		}
		switch (currentDirection)
		{
		case Direction.Right:
			currentDirection = Direction.Left;
			break;
		case Direction.Left:
			currentDirection = Direction.Right;
			break;
		case Direction.Up:
			currentDirection = Direction.Down;
			break;
		case Direction.Down:
			currentDirection = Direction.Up;
			break;
		}
		body.linearVelocity = GetVelocity(currentDirection);
		turnRoutine = null;
	}

	private void FlipXScale()
	{
		base.transform.SetScaleX(0f - base.transform.localScale.x);
		clockwise = !clockwise;
		Vector3 vector = base.transform.TransformVector(new Vector3(FlipXOffset, 0f, 0f));
		base.transform.position += vector;
	}

	private Vector2 GetVelocity(Direction direction)
	{
		Vector2 result = Vector2.zero;
		switch (direction)
		{
		case Direction.Right:
			result = new Vector2(speed, 0f);
			break;
		case Direction.Down:
			result = new Vector2(0f, 0f - speed);
			break;
		case Direction.Left:
			result = new Vector2(0f - speed, 0f);
			break;
		case Direction.Up:
			result = new Vector2(0f, speed);
			break;
		}
		return result;
	}

	private bool CheckGround()
	{
		return FireRayLocal(col.offset.Where(0f, null), Vector2.down, col.size.y / 2f + 0.5f).collider != null;
	}

	private bool CheckWall()
	{
		return FireRayLocal(col.offset, SpriteFacesRight ? Vector2.right : Vector2.left, col.size.x / 2f + wallRayPadding).collider != null;
	}

	public void Stun()
	{
		if (turnRoutine == null)
		{
			StopAllCoroutines();
			StartCoroutine(DoStun());
		}
	}

	private IEnumerator DoStun()
	{
		body.linearVelocity = Vector2.zero;
		yield return StartCoroutine(anim.PlayAnimWait(StunAnim));
		StartCoroutine(Walk());
	}

	private RaycastHit2D FireRayLocal(Vector2 origin, Vector2 direction, float length)
	{
		Vector2 vector = base.transform.TransformPoint(origin);
		Vector2 vector2 = base.transform.TransformVector(direction);
		RaycastHit2D result = Helper.Raycast2D(vector, vector2, length, 256);
		Debug.DrawLine(vector, vector + vector2 * length);
		return result;
	}

	private Vector2 GetTweenPos(Direction direction)
	{
		Vector2 result = Vector2.zero;
		float num = wallRayPadding - col.offset.x;
		switch (direction)
		{
		case Direction.Right:
			result = (clockwise ? new Vector2(col.size.x / 2f, col.size.y / 2f) : new Vector2(col.size.x / 2f, 0f - col.size.y / 2f));
			result.x += num;
			break;
		case Direction.Up:
			result = (clockwise ? new Vector2(0f - col.size.x / 2f, col.size.y / 2f) : new Vector2(col.size.x / 2f, col.size.y / 2f));
			result.y += num;
			break;
		case Direction.Down:
			result = (clockwise ? new Vector2(col.size.x / 2f, 0f - col.size.y / 2f) : new Vector2(0f - col.size.x / 2f, 0f - col.size.y / 2f));
			result.y -= num;
			break;
		case Direction.Left:
			result = (clockwise ? new Vector2(0f - col.size.x / 2f, 0f - col.size.y / 2f) : new Vector2(0f - col.size.x / 2f, col.size.y / 2f));
			result.x -= num;
			break;
		}
		return result;
	}

	private void StickToGround()
	{
		Vector2 offset = col.offset;
		offset.y -= col.size.y / 2f;
		RaycastHit2D raycastHit2D = FireRayLocal(offset + Vector2.up, Vector2.down, 2f);
		if (raycastHit2D.collider != null)
		{
			Vector2 vector = base.transform.TransformPoint(offset) - base.transform.position;
			base.transform.SetPosition2D(raycastHit2D.point - vector);
		}
	}
}
