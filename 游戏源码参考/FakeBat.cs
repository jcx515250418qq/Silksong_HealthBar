using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(tk2dSpriteAnimator))]
public class FakeBat : MonoBehaviour
{
	private enum States
	{
		WaitingForBossAwake = 0,
		Dormant = 1,
		In = 2,
		Out = 3
	}

	private Rigidbody2D body;

	private MeshRenderer meshRenderer;

	private tk2dSpriteAnimator spriteAnimator;

	private States state;

	private float turnCooldown;

	[SerializeField]
	private Transform grimm;

	private const float Z = 0f;

	private static List<FakeBat> fakeBats;

	protected void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		meshRenderer = GetComponent<MeshRenderer>();
		spriteAnimator = GetComponent<tk2dSpriteAnimator>();
		state = States.WaitingForBossAwake;
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	protected static void Init()
	{
		fakeBats = new List<FakeBat>();
	}

	protected void OnEnable()
	{
		fakeBats.Add(this);
	}

	protected void OnDisable()
	{
		fakeBats.Remove(this);
	}

	protected void Start()
	{
		float num = Random.Range(0.7f, 0.9f);
		base.transform.SetScaleX(num);
		base.transform.SetScaleY(num);
		base.transform.SetPositionZ(0f);
	}

	protected void Update()
	{
		turnCooldown -= Time.deltaTime;
	}

	public static void NotifyAllBossAwake()
	{
		foreach (FakeBat fakeBat in fakeBats)
		{
			if (!(fakeBat == null))
			{
				fakeBat.NotifyBossAwake();
			}
		}
	}

	public void NotifyBossAwake()
	{
		state = States.Dormant;
	}

	public static void SendAllOut()
	{
		foreach (FakeBat fakeBat in fakeBats)
		{
			if (!(fakeBat == null))
			{
				fakeBat.SendOut();
			}
		}
	}

	public void SendOut()
	{
		if (state == States.Dormant)
		{
			StartCoroutine("SendOutRoutine");
			StopCoroutine("BringInRoutine");
		}
	}

	protected IEnumerator SendOutRoutine()
	{
		state = States.Out;
		base.transform.SetPosition2D(grimm.transform.position);
		base.transform.SetPositionZ(0f);
		meshRenderer.enabled = true;
		spriteAnimator.Play("Bat Fly");
		int? selectedDirection = null;
		while (true)
		{
			float minInclusive;
			float maxInclusive;
			float minInclusive2;
			float maxInclusive2;
			int num;
			float num2;
			switch (selectedDirection ?? Random.Range(0, 4))
			{
			case 1:
				minInclusive = 1f;
				maxInclusive = 4f;
				minInclusive2 = 2f;
				maxInclusive2 = 3f;
				num = 1;
				num2 = 0.3f;
				break;
			case 3:
				minInclusive = 1f;
				maxInclusive = 4f;
				minInclusive2 = -3f;
				maxInclusive2 = -2f;
				num = 1;
				num2 = 0.3f;
				break;
			case 2:
				minInclusive = -5f;
				maxInclusive = -3f;
				minInclusive2 = 0.5f;
				maxInclusive2 = 2f;
				num = 0;
				num2 = 0.5f;
				break;
			default:
				minInclusive = 3f;
				maxInclusive = 5f;
				minInclusive2 = 0.5f;
				maxInclusive2 = 2f;
				num = 0;
				num2 = 0.5f;
				break;
			}
			int index = (num + 1) % 2;
			Vector2 accel = new Vector2(Random.Range(minInclusive, maxInclusive), Random.Range(minInclusive2, maxInclusive2));
			if (Random.Range(0, 1) == 0)
			{
				accel[index] = 0f - accel[index];
			}
			Vector2 linearVelocity = body.linearVelocity;
			linearVelocity[num] *= num2;
			body.linearVelocity = linearVelocity;
			accel *= 0.5f;
			Vector2 maxSpeed = accel * 10f;
			maxSpeed.x = Mathf.Abs(maxSpeed.x);
			maxSpeed.y = Mathf.Abs(maxSpeed.y);
			float timer;
			for (timer = 0.2f; timer > 0f; timer -= Time.deltaTime)
			{
				FaceDirection((body.linearVelocity.x > 0f) ? 1 : (-1), snap: false);
				Accelerate(accel, new Vector2(15f, 10f));
				yield return null;
			}
			selectedDirection = null;
			timer = Random.Range(0.5f, 1.5f);
			while (!selectedDirection.HasValue && timer > 0f)
			{
				FaceDirection((body.linearVelocity.x > 0f) ? 1 : (-1), snap: false);
				Accelerate(accel, maxSpeed);
				Vector2 vector = base.transform.position;
				if (vector.x < 73f)
				{
					selectedDirection = 0;
					break;
				}
				if (vector.x > 99f)
				{
					selectedDirection = 2;
					break;
				}
				if (vector.y < 8f)
				{
					selectedDirection = 1;
					break;
				}
				if (vector.y > 15f)
				{
					selectedDirection = 3;
					break;
				}
				yield return null;
				timer -= Time.deltaTime;
			}
		}
	}

	public static void BringAllIn()
	{
		foreach (FakeBat fakeBat in fakeBats)
		{
			if (!(fakeBat == null))
			{
				fakeBat.BringIn();
			}
		}
	}

	public void BringIn()
	{
		StartCoroutine("BringInRoutine");
		StopCoroutine("SendOutRoutine");
	}

	protected IEnumerator BringInRoutine()
	{
		state = States.In;
		int sign = ((grimm.transform.position.x - body.linearVelocity.x > 0f) ? 1 : (-1));
		FaceDirection(sign, snap: true);
		body.linearVelocity = Vector2.zero;
		while (true)
		{
			Vector2 current = base.transform.position;
			Vector2 vector = grimm.transform.position;
			Vector2 vector2 = Vector2.MoveTowards(current, vector, 25f * Time.deltaTime);
			base.transform.SetPosition2D(vector2);
			if (Vector2.Distance(vector2, vector) < Mathf.Epsilon)
			{
				break;
			}
			yield return null;
		}
		spriteAnimator.Play("Bat End");
		while (spriteAnimator.ClipTimeSeconds < spriteAnimator.CurrentClip.Duration - Mathf.Epsilon)
		{
			yield return null;
		}
		meshRenderer.enabled = false;
		base.transform.SetPositionY(-50f);
		state = States.Dormant;
	}

	private void FaceDirection(int sign, bool snap)
	{
		float num = Mathf.Abs(base.transform.localScale.x) * (float)sign;
		if (!Mathf.Approximately(base.transform.localScale.x, num) && (snap || turnCooldown <= 0f))
		{
			if (!snap)
			{
				spriteAnimator.Play("Bat TurnToFly");
				spriteAnimator.PlayFromFrame(0);
				turnCooldown = 0.5f;
			}
			base.transform.SetScaleX(num);
		}
	}

	private void Accelerate(Vector2 fixedAcceleration, Vector2 speedLimit)
	{
		Vector2 vector = fixedAcceleration / Time.fixedDeltaTime;
		Vector2 linearVelocity = body.linearVelocity;
		linearVelocity += vector * Time.deltaTime;
		linearVelocity.x = Mathf.Clamp(linearVelocity.x, 0f - speedLimit.x, speedLimit.x);
		linearVelocity.y = Mathf.Clamp(linearVelocity.y, 0f - speedLimit.y, speedLimit.y);
		body.linearVelocity = linearVelocity;
	}
}
