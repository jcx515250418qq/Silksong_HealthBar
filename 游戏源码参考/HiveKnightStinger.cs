using System;
using UnityEngine;

public class HiveKnightStinger : MonoBehaviour
{
	public float direction;

	private float speed = 20f;

	private float time = 2f;

	private float timer;

	private bool initialised;

	private Rigidbody2D rb;

	private Vector3 startPos;

	private void OnEnable()
	{
		if (!initialised)
		{
			startPos = base.transform.localPosition;
			initialised = true;
		}
		else
		{
			base.transform.localPosition = startPos;
		}
		if (rb == null)
		{
			rb = GetComponent<Rigidbody2D>();
		}
		timer = time;
	}

	private void Update()
	{
		float x = speed * Mathf.Cos(direction * (MathF.PI / 180f));
		float y = speed * Mathf.Sin(direction * (MathF.PI / 180f));
		Vector2 linearVelocity = default(Vector2);
		linearVelocity.x = x;
		linearVelocity.y = y;
		rb.linearVelocity = linearVelocity;
		if (timer > 0f)
		{
			timer -= Time.deltaTime;
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
