using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Crawler : MonoBehaviour, IEnemyMessageReceiver
{
	public enum CrawlerTypes
	{
		Floor = 0,
		Roof = 1,
		Wall = 2
	}

	public enum TerrainSides
	{
		Floor = 0,
		Roof = 1,
		RightWall = 2,
		LeftWall = 3
	}

	private const float RAY_SKIN_WIDTH = 0.1f;

	private const float RAY_FRONT_HEIGHT = 0.01f;

	private const float RAY_DOWN_HEIGHT = 0.5f;

	private const float RAY_DOWN_DISTANCE = 1f;

	[SerializeField]
	private float speed = 2f;

	[Space]
	[SerializeField]
	private float rayFrontDistance = 0.2f;

	[SerializeField]
	private float rayDownFrontPadding = 0.2f;

	[Space]
	[SerializeField]
	public string crawlAnimName = "Walk";

	[SerializeField]
	private bool spriteFacesRight;

	[SerializeField]
	private bool flipBeforeTurn;

	[SerializeField]
	private bool moveWhileTurning;

	[SerializeField]
	private bool doTurnAnim = true;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("doTurnAnim", true, false, false)]
	public string turnAnimName = "Turn";

	[SerializeField]
	private bool keepSpriteFacing;

	[SerializeField]
	private bool startInactive;

	[SerializeField]
	private bool ambientIdle;

	[SerializeField]
	private bool allowRoofRecoil;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("ambientIdle", true, false, false)]
	private string idleAnimName = "Ambient Idle";

	[SerializeField]
	private AudioSource loopAudioSource;

	[SerializeField]
	private bool pauseLoopDontStop;

	[SerializeField]
	private AudioSource ambientLoopAudioSource;

	private Vector2 velocity;

	private Coroutine crawlRoutine;

	private WaitForTk2dAnimatorClipFinish currentClipWait;

	private readonly WaitForFixedUpdate updateWait = new WaitForFixedUpdate();

	private bool isSetup;

	private bool isTurnScheduled;

	private Rigidbody2D body;

	private Recoil recoil;

	private tk2dSpriteAnimator anim;

	private HealthManager healthManager;

	private tk2dSprite sprite;

	private BoxCollider2D collider;

	private CrawlerTypes? type;

	private TerrainSides? terrainSide;

	public bool IsTurning { get; private set; }

	public bool IsCrawling => crawlRoutine != null;

	public float Speed
	{
		get
		{
			return speed;
		}
		set
		{
			speed = value;
			UpdateCrawlerType();
		}
	}

	public CrawlerTypes Type
	{
		get
		{
			if (!type.HasValue)
			{
				UpdateCrawlerType();
			}
			return type.Value;
		}
		private set
		{
			type = value;
		}
	}

	public TerrainSides TerrainSide
	{
		get
		{
			if (!terrainSide.HasValue)
			{
				UpdateCrawlerType();
			}
			return terrainSide.Value;
		}
		private set
		{
			terrainSide = value;
		}
	}

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		recoil = GetComponent<Recoil>();
		anim = GetComponent<tk2dSpriteAnimator>();
		sprite = GetComponent<tk2dSprite>();
		collider = GetComponent<BoxCollider2D>();
		if (ambientIdle)
		{
			healthManager = GetComponent<HealthManager>();
			if (healthManager != null)
			{
				healthManager.TookDamage += EndAmbientIdle;
			}
		}
	}

	private void OnEnable()
	{
		if (!startInactive)
		{
			StartCrawling();
			return;
		}
		StopSound();
		if ((bool)ambientLoopAudioSource)
		{
			ambientLoopAudioSource.Stop();
		}
	}

	private void OnDisable()
	{
		StopCrawling();
	}

	private IEnumerator Crawl()
	{
		double lastLoopTime = 0.0;
		anim.Play(crawlAnimName);
		while (true)
		{
			StartSound();
			while (true)
			{
				switch (Type)
				{
				case CrawlerTypes.Floor:
				case CrawlerTypes.Roof:
					body.SetVelocity(velocity.x, null);
					break;
				case CrawlerTypes.Wall:
				{
					Rigidbody2D rigidbody2D = body;
					float? y = velocity.y;
					rigidbody2D.SetVelocity(null, y);
					break;
				}
				default:
					throw new ArgumentOutOfRangeException();
				}
				if (ShouldTurn())
				{
					break;
				}
				while (recoil.IsRecoiling)
				{
					yield return updateWait;
				}
				yield return updateWait;
			}
			IEnumerator turn = Turn();
			while (turn.MoveNext())
			{
				object current = turn.Current;
				if (current is CustomYieldInstruction { keepWaiting: false })
				{
					break;
				}
				yield return current;
			}
			if (Math.Abs(Time.timeAsDouble - lastLoopTime) <= (double)Mathf.Epsilon)
			{
				yield return updateWait;
			}
			lastLoopTime = Time.timeAsDouble;
		}
	}

	private void PlayExtraAnim(string animName)
	{
		anim.Play(animName);
		currentClipWait = new WaitForTk2dAnimatorClipFinish(anim, delegate
		{
			anim.Play(crawlAnimName);
		});
	}

	private IEnumerator Turn()
	{
		IsTurning = true;
		switch (Type)
		{
		case CrawlerTypes.Floor:
		case CrawlerTypes.Roof:
			body.linearVelocity = new Vector3(0f, body.linearVelocity.y);
			break;
		case CrawlerTypes.Wall:
			body.linearVelocity = new Vector3(body.linearVelocity.x, 0f);
			break;
		}
		if (flipBeforeTurn)
		{
			FlipScaleX();
			if (doTurnAnim)
			{
				if (moveWhileTurning)
				{
					PlayExtraAnim(turnAnimName);
				}
				else
				{
					StopSound();
					anim.Play(turnAnimName);
					currentClipWait = new WaitForTk2dAnimatorClipFinish(anim, delegate
					{
						anim.Play(crawlAnimName);
					});
					yield return currentClipWait;
					StartSound();
				}
			}
		}
		else if (doTurnAnim)
		{
			if (moveWhileTurning)
			{
				PlayExtraAnim(turnAnimName);
				FlipScaleX();
			}
			else
			{
				StopSound();
				anim.Play(turnAnimName);
				currentClipWait = new WaitForTk2dAnimatorClipFinish(anim, delegate
				{
					FlipScaleX();
					anim.Play(crawlAnimName);
				});
				yield return currentClipWait;
				StartSound();
			}
		}
		else
		{
			FlipScaleX();
		}
		velocity.x *= -1f;
		velocity.y *= -1f;
		IsTurning = false;
	}

	private void FlipScaleX()
	{
		base.transform.SetScaleX(base.transform.localScale.x * -1f);
		if ((bool)sprite && keepSpriteFacing)
		{
			sprite.FlipX = base.transform.localScale.x < 0f;
		}
	}

	private bool ShouldTurn(bool drawGizmos = false)
	{
		if (isTurnScheduled)
		{
			isTurnScheduled = false;
			return true;
		}
		Vector2 offset = collider.offset;
		Vector2 self = collider.size / 2f;
		int num = (spriteFacesRight ? 1 : (-1));
		Vector2 vector = offset - self.MultiplyElements(num, null);
		Vector2 vector2 = offset + self.MultiplyElements(num, null);
		Vector2 vector3 = new Vector2(vector2.x - 0.1f * (float)num, vector.y + 0.01f);
		Vector2 vector4 = Vector2.right.MultiplyElements(num, null);
		float num2 = Mathf.Max(rayFrontDistance, Mathf.Abs(velocity.x * Time.deltaTime)) + 0.1f;
		float y = vector.y + 0.5f + 0.1f;
		Vector2 vector5 = new Vector2(vector2.x + rayDownFrontPadding * (float)num, y);
		Vector2 vector6 = new Vector2(vector.x, y);
		Vector2 down = Vector2.down;
		float num3 = 1.1f;
		if (drawGizmos)
		{
			Gizmos.DrawLine(vector3, vector3 + vector4 * num2);
			Gizmos.DrawLine(vector5, vector5 + down * num3);
			Gizmos.DrawLine(vector6, vector6 + down * num3);
		}
		else
		{
			if (!IsRayHittingLocal(vector6, down, num3))
			{
				return false;
			}
			if (IsRayHittingLocal(vector3, vector4, num2))
			{
				return true;
			}
			if (!IsRayHittingLocal(vector5, down, num3))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsRayHittingLocal(Vector2 originLocal, Vector2 directionLocal, float length)
	{
		return base.transform.IsRayHittingLocalNoTriggers(originLocal, directionLocal, length, 33024);
	}

	private void OnDrawGizmosSelected()
	{
		if (collider == null)
		{
			collider = GetComponent<BoxCollider2D>();
		}
		if (!(collider == null))
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			ShouldTurn(drawGizmos: true);
		}
	}

	public void StartCrawling()
	{
		StartCrawling(scheduleTurn: false);
	}

	public void StartCrawling(bool scheduleTurn)
	{
		isTurnScheduled = scheduleTurn;
		if (!isSetup)
		{
			isSetup = true;
			recoil.OnCancelRecoil += delegate
			{
				if (IsCrawling)
				{
					body.linearVelocity = velocity;
				}
			};
		}
		UpdateCrawlerType();
		switch (Type)
		{
		case CrawlerTypes.Floor:
			body.gravityScale = 1f;
			recoil.FreezeInPlace = false;
			break;
		case CrawlerTypes.Roof:
		case CrawlerTypes.Wall:
			body.gravityScale = 0f;
			if (!allowRoofRecoil)
			{
				recoil.FreezeInPlace = true;
				recoil.SetRecoilSpeed(0f);
			}
			else
			{
				recoil.IsDownBlocked = true;
				recoil.IsUpBlocked = true;
			}
			break;
		}
		if (!ambientIdle)
		{
			EndAmbientIdle();
			return;
		}
		anim.Play(idleAnimName);
		StopSound();
		if ((bool)ambientLoopAudioSource)
		{
			ambientLoopAudioSource.Play();
		}
	}

	private void UpdateCrawlerType()
	{
		Transform obj = base.transform;
		float z = obj.eulerAngles.z;
		Vector3 localScale = obj.localScale;
		float num = Mathf.Sign(localScale.x) * (float)(spriteFacesRight ? 1 : (-1));
		if (z <= 225f)
		{
			if (!(z >= 45f))
			{
				goto IL_011c;
			}
			if (z <= 135f)
			{
				Type = CrawlerTypes.Wall;
				velocity = new Vector2(0f, num * speed);
				TerrainSide = ((localScale.y > 0f) ? TerrainSides.RightWall : TerrainSides.LeftWall);
			}
			else
			{
				Type = ((localScale.y > 0f) ? CrawlerTypes.Roof : CrawlerTypes.Floor);
				velocity = new Vector2((0f - num) * speed, 0f);
				TerrainSide = ((localScale.y > 0f) ? TerrainSides.Roof : TerrainSides.Floor);
			}
		}
		else
		{
			if (!(z <= 315f))
			{
				goto IL_011c;
			}
			Type = CrawlerTypes.Wall;
			velocity = new Vector2(0f, (0f - num) * speed);
			TerrainSide = ((localScale.y > 0f) ? TerrainSides.LeftWall : TerrainSides.RightWall);
		}
		goto IL_0162;
		IL_011c:
		Type = ((!(localScale.y > 0f)) ? CrawlerTypes.Roof : CrawlerTypes.Floor);
		velocity = new Vector2(num * speed, 0f);
		TerrainSide = ((!(localScale.y > 0f)) ? TerrainSides.Roof : TerrainSides.Floor);
		goto IL_0162;
		IL_0162:
		recoil.IsUpBlocked = false;
		recoil.IsDownBlocked = false;
		recoil.IsLeftBlocked = false;
		recoil.IsRightBlocked = false;
		body.constraints = RigidbodyConstraints2D.FreezeRotation;
		switch (TerrainSide)
		{
		case TerrainSides.Roof:
			recoil.IsUpBlocked = true;
			recoil.IsDownBlocked = true;
			body.constraints |= RigidbodyConstraints2D.FreezePositionY;
			break;
		case TerrainSides.RightWall:
		case TerrainSides.LeftWall:
			recoil.IsLeftBlocked = true;
			recoil.IsRightBlocked = true;
			body.constraints |= RigidbodyConstraints2D.FreezePositionX;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case TerrainSides.Floor:
			break;
		}
	}

	public void StopCrawling()
	{
		StopSound();
		if (crawlRoutine != null)
		{
			StopCoroutine(crawlRoutine);
			switch (Type)
			{
			case CrawlerTypes.Floor:
			case CrawlerTypes.Roof:
				body.linearVelocity = new Vector3(0f, body.linearVelocity.y);
				break;
			case CrawlerTypes.Wall:
				body.linearVelocity = new Vector3(body.linearVelocity.x, 0f);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			crawlRoutine = null;
		}
		body.constraints = RigidbodyConstraints2D.FreezeRotation;
		currentClipWait?.Cancel();
	}

	public void EndAmbientIdle()
	{
		ambientIdle = false;
		if ((bool)ambientLoopAudioSource)
		{
			ambientLoopAudioSource.Stop();
		}
		if (crawlRoutine == null)
		{
			crawlRoutine = StartCoroutine(Crawl());
		}
	}

	private void StartSound()
	{
		if ((bool)loopAudioSource)
		{
			loopAudioSource.Play();
			if (pauseLoopDontStop)
			{
				loopAudioSource.UnPause();
			}
		}
	}

	private void StopSound()
	{
		if ((bool)loopAudioSource)
		{
			if (pauseLoopDontStop)
			{
				loopAudioSource.Pause();
			}
			else
			{
				loopAudioSource.Stop();
			}
		}
	}

	public void ReceiveEnemyMessage(string eventName)
	{
		if (IsTurning || !IsCrawling)
		{
			return;
		}
		Vector2 linearVelocity = body.linearVelocity;
		switch (Type)
		{
		case CrawlerTypes.Floor:
		case CrawlerTypes.Roof:
			if (linearVelocity.x > 0f)
			{
				if (eventName == "GO LEFT")
				{
					isTurnScheduled = true;
				}
			}
			else if (eventName == "GO RIGHT")
			{
				isTurnScheduled = true;
			}
			break;
		case CrawlerTypes.Wall:
			if (linearVelocity.y > 0f)
			{
				if (eventName == "GO DOWN")
				{
					isTurnScheduled = true;
				}
			}
			else if (eventName == "GO UP")
			{
				isTurnScheduled = true;
			}
			break;
		}
	}
}
