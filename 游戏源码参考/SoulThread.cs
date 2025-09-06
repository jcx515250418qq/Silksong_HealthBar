using System;
using Ara;
using UnityEngine;

public class SoulThread : MonoBehaviour
{
	private Rigidbody2D rb;

	private Transform hero;

	private AraTrail trailRenderer;

	public float angle;

	public float angleToHero;

	private float speed;

	private float turnSpeed;

	private float timer;

	private bool moving;

	private bool clockwise;

	private Material[] materials;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		trailRenderer = GetComponent<AraTrail>();
		if (!trailRenderer)
		{
			return;
		}
		if (materials != null)
		{
			Material[] array = materials;
			foreach (Material material in array)
			{
				if ((bool)material)
				{
					UnityEngine.Object.Destroy(material);
				}
			}
		}
		materials = new Material[trailRenderer.materials.Length];
		for (int j = 0; j < materials.Length; j++)
		{
			materials[j] = new Material(trailRenderer.materials[j]);
			materials[j].renderQueue = 4000;
		}
		trailRenderer.materials = materials;
	}

	private void OnDestroy()
	{
		if (materials == null)
		{
			return;
		}
		Material[] array = materials;
		foreach (Material material in array)
		{
			if ((bool)material)
			{
				UnityEngine.Object.Destroy(material);
			}
		}
	}

	private void OnEnable()
	{
		if (hero == null)
		{
			hero = GameManager.instance.hero_ctrl.transform;
		}
		speed = UnityEngine.Random.Range(70f, 80f);
		angle = UnityEngine.Random.Range(0f, 360f);
		turnSpeed = UnityEngine.Random.Range(250f, 500f);
		timer = 0f;
		moving = true;
		trailRenderer.Clear();
		GetAngleToHero();
	}

	private void Update()
	{
		if (moving)
		{
			GetAngleToHero();
			while (angleToHero < 0f)
			{
				angleToHero += 360f;
			}
			GetTurnDirection();
			if (clockwise)
			{
				angle -= turnSpeed * Time.deltaTime;
				if (angle < angleToHero && angleToHero - angle < 180f)
				{
					angle = angleToHero;
				}
			}
			else
			{
				angle += turnSpeed * Time.deltaTime;
				if (angle > angleToHero && angle - angleToHero < 180f)
				{
					angle = angleToHero;
				}
			}
			while (angle < 0f)
			{
				angle += 360f;
			}
			while (angle > 360f)
			{
				angle -= 360f;
			}
			turnSpeed += 3000f * Time.deltaTime;
			if (speed < 75f)
			{
				speed += 30f * Time.deltaTime;
			}
			ApplyMovement();
			if ((double)timer > 0.5)
			{
				StopThread();
			}
			if (Vector3.Distance(base.transform.position, hero.position) < 1f && timer > 0.1f)
			{
				StopThread();
			}
		}
		if (!moving && timer > 0.15f)
		{
			base.gameObject.Recycle();
		}
		timer += Time.deltaTime;
	}

	private void GetAngleToHero()
	{
		float y = hero.position.y - base.transform.position.y;
		float x = hero.position.x - base.transform.position.x;
		angleToHero = Mathf.Atan2(y, x) * (180f / MathF.PI);
	}

	private void ApplyMovement()
	{
		float x = speed * Mathf.Cos(angle * (MathF.PI / 180f));
		float y = speed * Mathf.Sin(angle * (MathF.PI / 180f));
		Vector2 linearVelocity = default(Vector2);
		linearVelocity.x = x;
		linearVelocity.y = y;
		rb.linearVelocity = linearVelocity;
	}

	private void GetTurnDirection()
	{
		clockwise = true;
		if ((angleToHero > angle && angleToHero - angle < 180f) || (angleToHero < angle && angle - angleToHero > 180f))
		{
			clockwise = false;
		}
	}

	private void StopThread()
	{
		moving = false;
		rb.linearVelocity = new Vector2(0f, 0f);
		timer = 0f;
	}
}
