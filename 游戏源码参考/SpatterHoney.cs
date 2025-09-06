using System;
using UnityEngine;

public class SpatterHoney : MonoBehaviour
{
	public Rigidbody2D rb2d;

	public CircleCollider2D circleCollider;

	public SpriteRenderer spriteRenderer;

	private float stretchFactor = 1.4f;

	private float stretchMinX = 0.7f;

	private float stretchMaxY = 1.75f;

	private float scaleModifier;

	public float scaleModifierMin = 0.7f;

	public float scaleModifierMax = 1.3f;

	private float state;

	private float idleTimer;

	private void Start()
	{
		scaleModifier = UnityEngine.Random.Range(scaleModifierMin, scaleModifierMax);
	}

	private void OnEnable()
	{
		rb2d.isKinematic = false;
		circleCollider.enabled = true;
		idleTimer = 0f;
	}

	private void Update()
	{
		FaceAngle();
		ProjectileSquash();
		idleTimer += Time.deltaTime;
		if (idleTimer > 3f)
		{
			Impact();
		}
	}

	private void Impact()
	{
		if (idleTimer > 0.1f)
		{
			base.gameObject.Recycle();
		}
	}

	private void FaceAngle()
	{
		Vector2 linearVelocity = rb2d.linearVelocity;
		float z = Mathf.Atan2(linearVelocity.y, linearVelocity.x) * (180f / MathF.PI);
		base.transform.localEulerAngles = new Vector3(0f, 0f, z);
	}

	private void ProjectileSquash()
	{
		float num = 1f - rb2d.linearVelocity.magnitude * stretchFactor * 0.01f;
		float num2 = 1f + rb2d.linearVelocity.magnitude * stretchFactor * 0.01f;
		if (num2 < stretchMinX)
		{
			num2 = stretchMinX;
		}
		if (num > stretchMaxY)
		{
			num = stretchMaxY;
		}
		num *= scaleModifier;
		num2 *= scaleModifier;
		base.transform.localScale = new Vector3(num2, num, base.transform.localScale.z);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Impact();
	}
}
