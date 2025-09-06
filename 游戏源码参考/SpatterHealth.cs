using System;
using UnityEngine;

public class SpatterHealth : MonoBehaviour
{
	private enum States
	{
		Init = 0,
		Fall = 1,
		Decel = 2,
		Attract = 3
	}

	[SerializeField]
	private GameObject healthGetEffect;

	[SerializeField]
	private AudioEventRandom getAudio;

	private States currentState;

	private float scaleModifier;

	private float fallTimeLeft;

	private float fireSpeed;

	private Rigidbody2D body;

	private HeroController hc;

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
	}

	private void OnEnable()
	{
		scaleModifier = UnityEngine.Random.Range(1.3f, 1.6f);
		currentState = States.Init;
		fallTimeLeft = UnityEngine.Random.Range(0.25f, 0.45f);
		fireSpeed = 0.1f;
		if ((bool)healthGetEffect)
		{
			healthGetEffect.SetActive(value: false);
		}
		hc = HeroController.instance;
	}

	private void FixedUpdate()
	{
		FaceAngle();
		ProjectileSquash();
		switch (currentState)
		{
		case States.Init:
			base.transform.SetParent(null, worldPositionStays: true);
			currentState = States.Fall;
			break;
		case States.Fall:
			fallTimeLeft -= Time.deltaTime;
			if (fallTimeLeft <= 0f)
			{
				currentState = States.Decel;
				body.gravityScale = 0f;
			}
			break;
		case States.Decel:
		{
			Vector2 linearVelocity = body.linearVelocity;
			linearVelocity *= 0.75f;
			body.linearVelocity = linearVelocity;
			if (linearVelocity.magnitude <= 0.3f)
			{
				currentState = States.Attract;
			}
			break;
		}
		case States.Attract:
			if (Vector2.Distance(hc.transform.position, base.transform.position) <= 0.6f)
			{
				Collect();
			}
			else
			{
				FireAtTarget();
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void FaceAngle()
	{
		Vector2 linearVelocity = body.linearVelocity;
		float z = Mathf.Atan2(linearVelocity.y, linearVelocity.x) * (180f / MathF.PI);
		base.transform.localEulerAngles = new Vector3(0f, 0f, z);
	}

	private void ProjectileSquash()
	{
		float num = 1f - body.linearVelocity.magnitude * 1.4f * 0.01f;
		float num2 = 1f + body.linearVelocity.magnitude * 1.4f * 0.01f;
		if (num2 < 0.6f)
		{
			num2 = 0.6f;
		}
		if (num > 1.75f)
		{
			num = 1.75f;
		}
		num *= scaleModifier;
		num2 *= scaleModifier;
		base.transform.localScale = new Vector3(num2, num, base.transform.localScale.z);
	}

	private void FireAtTarget()
	{
		Vector3 position = hc.transform.position;
		float num = Mathf.Atan2(x: position.x - base.transform.position.x, y: position.y + -0.5f - base.transform.position.y) * (180f / MathF.PI);
		fireSpeed += 0.85f;
		if (fireSpeed > 30f)
		{
			fireSpeed = 30f;
		}
		float x2 = fireSpeed * Mathf.Cos(num * (MathF.PI / 180f));
		float y = fireSpeed * Mathf.Sin(num * (MathF.PI / 180f));
		body.linearVelocity = new Vector2(x2, y);
	}

	private void Collect()
	{
		getAudio.SpawnAndPlayOneShot(base.transform.position);
		hc.SpriteFlash.flashHealBlue();
		if ((bool)healthGetEffect)
		{
			healthGetEffect.SetActive(value: true);
			Transform obj = healthGetEffect.transform;
			obj.SetRotationZ(UnityEngine.Random.Range(0f, 360f));
			Vector3 localScale = obj.localScale;
			localScale.x = (localScale.y = UnityEngine.Random.Range(0.5f, 0.7f));
			obj.localScale = localScale;
			obj.SetParent(null, worldPositionStays: true);
		}
		base.gameObject.SetActive(value: false);
	}
}
