using System;
using UnityEngine;

public class PointAtPivot : MonoBehaviour
{
	[SerializeField]
	private Transform pivot;

	[SerializeField]
	private float angleOffset;

	private void Awake()
	{
		if (pivot == null)
		{
			base.enabled = false;
		}
	}

	private void Start()
	{
	}

	private void LateUpdate()
	{
		Transform transform = base.transform;
		Vector3 position = pivot.position;
		Vector3 position2 = transform.position;
		float y = position.y - position2.y;
		float x = position.x - position2.x;
		float num;
		for (num = Mathf.Atan2(y, x) * (180f / MathF.PI) + angleOffset; num < 0f; num += 360f)
		{
		}
		transform.eulerAngles = new Vector3(0f, 0f, num);
	}
}
