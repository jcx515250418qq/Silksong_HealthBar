using System;
using UnityEngine;

public class SetAngleToVelocity : MonoBehaviour
{
	public Rigidbody2D rb;

	public float angleOffset;

	private void Update()
	{
		Vector2 linearVelocity = rb.linearVelocity;
		float z = Mathf.Atan2(linearVelocity.y, linearVelocity.x) * (180f / MathF.PI) + angleOffset;
		base.transform.localEulerAngles = new Vector3(0f, 0f, z);
	}
}
