using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class WalkerV2 : MonoBehaviour
{
	[Serializable]
	public class UnityBoolEvent : UnityEvent<bool>
	{
	}

	private const float SKIN_WIDTH = 0.1f;

	private const float TOP_RAY_PADDING = 0.5f;

	private const float BOTTOM_RAY_PADDING = 0.5f;

	private const float DOWN_RAY_DISTANCE = 0.5f;

	private const int LAYERMASK = 33024;

	public bool StartActive = true;

	public string IdleAnim;

	public string TurnAnim;

	public float WallDistance = 1f;

	public float GroundAheadDistance = 0.5f;

	public float TurnCooldown;

	public string WalkAnim;

	public float WalkSpeed;

	public bool FlipBeforeTurn;

	[Space]
	public string StartleAnim;

	public string RunAnim;

	public float RunSpeed;

	[Space]
	public MinMaxFloat RestPauseTime;

	public MinMaxFloat RestCooldownTime;

	[SerializeField]
	private AudioSource walkAudioSource;

	[SerializeField]
	private AudioSource runAudioSource;

	[SerializeField]
	private float rightDirection = -1f;

	[Space]
	public AlertRange AggroRange;

	public MinMaxFloat TurnAggroCooldown = new MinMaxFloat(0.5f, 1.5f);

	[SerializeField]
	private AudioEventRandom aggroSound;

	[Space]
	public UnityBoolEvent OnWalking;

	public UnityBoolEvent OnRunning;

	private Coroutine walkRoutine;

	private bool isMoving;

	private bool queuedTurnFlip;

	private bool isTurnAnimPlaying;

	private float moveDirection;

	private bool wasInAggro;

	private bool forceTurn;

	private double nextTurnTime;

	private double nextAggroTime;

	private Transform hero;

	private tk2dSpriteAnimator animator;

	private BoxCollider2D box;

	private Rigidbody2D body;

	public float RightDirection
	{
		get
		{
			return rightDirection;
		}
		set
		{
			if (value == 0f)
			{
				rightDirection = 1f;
			}
			else
			{
				rightDirection = Mathf.Sign(value);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (!box)
		{
			box = GetComponent<BoxCollider2D>();
		}
		if ((bool)box)
		{
			IsRaysHittingWall(isDrawingGizmos: true);
			IsRaysHittingGroundFront(isDrawingGizmos: true);
			IsRaysHittingGroundCentre(isDrawingGizmos: true);
		}
	}

	private void OnValidate()
	{
		RightDirection = rightDirection;
	}

	private void Awake()
	{
		animator = GetComponent<tk2dSpriteAnimator>();
		box = GetComponent<BoxCollider2D>();
		body = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		if (!hero)
		{
			hero = HeroController.instance.transform;
			if (StartActive)
			{
				StartWalking();
			}
			else
			{
				StopWalking();
			}
		}
	}

	private void OnDisable()
	{
		if (walkRoutine != null)
		{
			StopCoroutine(walkRoutine);
			walkRoutine = null;
		}
	}

	public void StartWalking()
	{
		if (!hero)
		{
			hero = HeroController.instance.transform;
		}
		if (walkRoutine == null)
		{
			moveDirection = GetFacingDirection();
			walkRoutine = StartCoroutine(Walking());
		}
	}

	public void StopWalking()
	{
		if (walkRoutine != null)
		{
			StopCoroutine(walkRoutine);
		}
		walkRoutine = null;
		isMoving = false;
		if ((bool)walkAudioSource)
		{
			walkAudioSource.Stop();
		}
		if ((bool)runAudioSource)
		{
			runAudioSource.Stop();
		}
		body.SetVelocity(0f, null);
		OnWalking?.Invoke(arg0: false);
		OnRunning?.Invoke(arg0: false);
	}

	private IEnumerator Walking()
	{
		while (true)
		{
			isMoving = true;
			if ((bool)walkAudioSource)
			{
				walkAudioSource.Play();
			}
			float restDuration = RestCooldownTime.GetRandomValue();
			double nextRestTime = Time.timeAsDouble + (double)restDuration;
			wasInAggro = IsInAggro();
			SetMoving(moveDirection);
			while (true)
			{
				if (GetFacingDirection() != moveDirection && !isTurnAnimPlaying)
				{
					FlipScale();
				}
				if (Time.timeAsDouble >= nextRestTime && restDuration > 0f)
				{
					break;
				}
				if (isTurnAnimPlaying && !animator.IsPlaying(TurnAnim))
				{
					isTurnAnimPlaying = false;
					PlayMoveAnim();
					if (queuedTurnFlip && !FlipBeforeTurn)
					{
						queuedTurnFlip = false;
						FlipScale();
					}
				}
				if (((IsRaysHittingWall() || (!IsRaysHittingGroundFront() && IsRaysHittingGroundCentre())) && Time.timeAsDouble >= nextTurnTime) || forceTurn)
				{
					forceTurn = false;
					nextTurnTime = Time.timeAsDouble + (double)TurnCooldown;
					moveDirection = 0f - moveDirection;
					wasInAggro = IsInAggro();
					SetMoving(moveDirection);
				}
				if (IsInAggro() && !isTurnAnimPlaying)
				{
					nextRestTime = Time.timeAsDouble + (double)restDuration;
					float directionToHero = Mathf.Sign(hero.position.x - base.transform.position.x);
					if (moveDirection != directionToHero)
					{
						body.SetVelocity(0f, null);
						wasInAggro = false;
						yield return new WaitForSeconds(SetFacing(directionToHero));
						if (queuedTurnFlip && !FlipBeforeTurn)
						{
							queuedTurnFlip = false;
							FlipScale();
						}
						moveDirection = directionToHero;
					}
					if (!wasInAggro)
					{
						wasInAggro = true;
						if ((bool)walkAudioSource)
						{
							walkAudioSource.Stop();
						}
						if (OnWalking != null)
						{
							OnWalking.Invoke(arg0: false);
						}
						aggroSound.SpawnAndPlayOneShot(base.transform.position);
						body.SetVelocity(0f, null);
						yield return StartCoroutine(animator.PlayAnimWait(StartleAnim));
						SetMoving(moveDirection);
					}
				}
				yield return null;
			}
			body.SetVelocity(0f, null);
			isMoving = false;
			if ((bool)walkAudioSource)
			{
				walkAudioSource.Stop();
			}
			if ((bool)runAudioSource)
			{
				runAudioSource.Stop();
			}
			if (OnWalking != null)
			{
				OnWalking.Invoke(arg0: false);
			}
			if (OnRunning != null)
			{
				OnRunning.Invoke(arg0: false);
			}
			animator.Play(IdleAnim);
			float restTime = RestPauseTime.GetRandomValue();
			while (restTime > 0f && !IsInAggro())
			{
				yield return null;
				restTime -= Time.deltaTime;
			}
		}
	}

	private void PlayMoveAnim()
	{
		string text = (wasInAggro ? RunAnim : WalkAnim);
		if (!animator.IsPlaying(text))
		{
			animator.Play(text);
		}
	}

	private void SetMoving(float direction)
	{
		SetFacing(direction);
		float num;
		if (wasInAggro)
		{
			num = RunSpeed;
			if ((bool)walkAudioSource)
			{
				walkAudioSource.Stop();
			}
			if ((bool)runAudioSource)
			{
				runAudioSource.Play();
			}
		}
		else
		{
			num = WalkSpeed;
			if ((bool)runAudioSource)
			{
				runAudioSource.Stop();
			}
			if ((bool)walkAudioSource)
			{
				walkAudioSource.Play();
			}
		}
		if (OnWalking != null)
		{
			OnWalking.Invoke(!wasInAggro);
		}
		if (OnRunning != null)
		{
			OnRunning.Invoke(wasInAggro);
		}
		body.SetVelocity(num * direction, null);
	}

	private bool IsInAggro()
	{
		if ((bool)AggroRange && AggroRange.IsHeroInRange())
		{
			return Time.timeAsDouble > nextAggroTime;
		}
		return false;
	}

	private float SetFacing(float direction)
	{
		float num = 0f;
		direction = Mathf.Sign(direction);
		if (GetFacingDirection() != direction)
		{
			num = PlayTurnAnim();
			nextAggroTime = Time.timeAsDouble + (double)num + (double)TurnAggroCooldown.GetRandomValue();
		}
		else
		{
			animator.Play(WalkAnim);
		}
		if (queuedTurnFlip && FlipBeforeTurn)
		{
			queuedTurnFlip = false;
			FlipScale();
		}
		return num;
	}

	private float PlayTurnAnim()
	{
		queuedTurnFlip = true;
		isTurnAnimPlaying = true;
		return animator.PlayAnimGetTime(TurnAnim);
	}

	private void FlipScale()
	{
		base.transform.SetScaleX(0f - base.transform.localScale.x);
	}

	private float GetFacingDirection()
	{
		return Mathf.Sign(base.transform.localScale.x) * rightDirection;
	}

	private bool IsRaysHittingWall(bool isDrawingGizmos = false)
	{
		float movingDirection = GetMovingDirection();
		Bounds bounds = box.bounds;
		Vector2 origin = bounds.max;
		Vector2 origin2 = bounds.min;
		Vector2 direction = ((movingDirection > 0f) ? Vector2.right : Vector2.left);
		if (movingDirection < 0f)
		{
			origin.x = origin2.x;
		}
		else
		{
			origin2.x = origin.x;
		}
		origin.x -= 0.1f * movingDirection;
		origin2.x -= 0.1f * movingDirection;
		origin.y -= 0.5f;
		origin2.y += 0.5f;
		float length = (body ? Mathf.Max(WallDistance, body.linearVelocity.x * Time.fixedDeltaTime) : WallDistance) + 0.1f;
		if (isDrawingGizmos)
		{
			return false;
		}
		bool num = Helper.IsRayHittingNoTriggers(origin, direction, length, 33024);
		bool flag = Helper.IsRayHittingNoTriggers(origin2, direction, length, 33024);
		return num || flag;
	}

	private float GetMovingDirection()
	{
		if (!body || body.linearVelocity.x == 0f)
		{
			return GetFacingDirection();
		}
		return Mathf.Sign(body.linearVelocity.x);
	}

	private bool IsRaysHittingGroundFront(bool isDrawingGizmos = false)
	{
		float movingDirection = GetMovingDirection();
		Bounds bounds = box.bounds;
		Vector2 vector = bounds.min;
		Vector2 vector2 = bounds.max;
		Vector2 origin = bounds.center;
		if (movingDirection > 0f)
		{
			origin.x = vector2.x + GroundAheadDistance;
		}
		else
		{
			origin.x = vector.x - GroundAheadDistance;
		}
		float length = origin.y - vector.y + 0.5f;
		if (isDrawingGizmos)
		{
			return false;
		}
		return Helper.IsRayHittingNoTriggers(origin, Vector2.down, length, 33024);
	}

	private bool IsRaysHittingGroundCentre(bool isDrawingGizmos = false)
	{
		GetMovingDirection();
		Bounds bounds = box.bounds;
		Vector2 vector = bounds.min;
		Vector2 vector2 = bounds.center;
		float num = vector2.y - vector.y + 0.5f;
		if (isDrawingGizmos)
		{
			Gizmos.color = (isMoving ? Color.yellow : Color.green);
			Gizmos.DrawLine(vector2, vector2 + Vector2.down * num);
			return false;
		}
		return Helper.IsRayHittingNoTriggers(vector2, Vector2.down, num, 33024);
	}

	public void ForceDirection(float direction)
	{
		if (direction != 0f && Mathf.Sign(GetMovingDirection()) != Mathf.Sign(direction))
		{
			forceTurn = true;
		}
	}
}
