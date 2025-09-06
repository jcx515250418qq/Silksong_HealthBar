using System;
using Ara;
using UnityEngine;

public class TrailThread : MonoBehaviour
{
	public float speedMin;

	public float speedMax;

	public float turnAccelMax;

	public float turnMax;

	public float trailStartPause;

	private Rigidbody2D rb;

	private AraTrail trailRenderer;

	private float angle;

	private float speed;

	private float turnSpeed;

	private float turnAccel;

	private float timer;

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
		speed = UnityEngine.Random.Range(speedMin, speedMax);
		angle = UnityEngine.Random.Range(0f, 360f);
		turnSpeed = 0f;
		turnAccel = UnityEngine.Random.Range(0f, turnAccelMax);
		if (UnityEngine.Random.Range(1, 100) > 50)
		{
			turnAccel *= -1f;
		}
		timer = trailStartPause;
		trailRenderer.Clear();
		trailRenderer.enabled = false;
	}

	private void Update()
	{
		angle += turnSpeed * Time.deltaTime;
		while (angle < 0f)
		{
			angle += 360f;
		}
		while (angle > 360f)
		{
			angle -= 360f;
		}
		turnSpeed += turnAccel * Time.deltaTime;
		if (angle > turnMax)
		{
			angle = turnMax;
		}
		ApplyMovement();
		if (timer > 0f)
		{
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				trailRenderer.enabled = true;
			}
		}
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
}
