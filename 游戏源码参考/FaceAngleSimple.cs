using System;
using UnityEngine;

public class FaceAngleSimple : MonoBehaviour
{
	public float angleOffset;

	public bool everyFrame;

	public bool reflectY;

	private Rigidbody2D rb2d;

	private float yScale;

	private void Awake()
	{
		yScale = base.transform.GetScaleY();
	}

	private void OnEnable()
	{
		rb2d = GetComponent<Rigidbody2D>();
		DoAngle();
	}

	private void Update()
	{
		if (everyFrame)
		{
			DoAngle();
		}
	}

	private void DoAngle()
	{
		Vector2 linearVelocity = rb2d.linearVelocity;
		if (linearVelocity.x == 0f || linearVelocity.y == 0f)
		{
			return;
		}
		float num = Mathf.Atan2(linearVelocity.y, linearVelocity.x) * (180f / MathF.PI) + angleOffset;
		if (num < 0f)
		{
			num += 360f;
		}
		if (num > 360f)
		{
			num -= 360f;
		}
		if (reflectY)
		{
			if (num > 90f && num < 270f)
			{
				if (base.transform.localScale.y != 0f - yScale)
				{
					base.transform.SetScaleY(0f - yScale);
				}
			}
			else if (base.transform.localScale.y != yScale)
			{
				base.transform.SetScaleY(yScale);
			}
		}
		base.transform.localEulerAngles = new Vector3(0f, 0f, num);
	}
}
