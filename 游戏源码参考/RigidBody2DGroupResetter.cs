using UnityEngine;

public class RigidBody2DGroupResetter : MonoBehaviour
{
	private struct BodyData
	{
		public Vector2 Position;

		public float Angle;
	}

	[SerializeField]
	private bool setRadialVelocity;

	[SerializeField]
	private Vector2 radialVelocityCentre;

	[SerializeField]
	private float radialVelocityMagnitude;

	private BodyData[] initialBodyDatas;

	private Rigidbody2D[] bodies;

	private void OnDrawGizmosSelected()
	{
		if (setRadialVelocity)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(base.transform.TransformPoint(radialVelocityCentre), radialVelocityMagnitude);
		}
	}

	private void Awake()
	{
		bodies = GetComponentsInChildren<Rigidbody2D>();
		initialBodyDatas = new BodyData[bodies.Length];
	}

	private void OnEnable()
	{
		Vector3 vector = base.transform.TransformPoint(radialVelocityCentre);
		for (int i = 0; i < bodies.Length; i++)
		{
			Rigidbody2D rigidbody2D = bodies[i];
			Transform transform = rigidbody2D.transform;
			initialBodyDatas[i] = new BodyData
			{
				Position = transform.localPosition,
				Angle = transform.localEulerAngles.z
			};
			if (setRadialVelocity && !rigidbody2D.isKinematic)
			{
				Vector3 normalized = (transform.position - vector).normalized;
				rigidbody2D.linearVelocity = normalized * radialVelocityMagnitude;
			}
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < bodies.Length; i++)
		{
			Rigidbody2D obj = bodies[i];
			Transform transform = obj.transform;
			BodyData bodyData = initialBodyDatas[i];
			obj.linearVelocity = Vector2.zero;
			obj.angularVelocity = 0f;
			transform.localPosition = bodyData.Position;
			transform.SetLocalRotation2D(bodyData.Angle);
		}
	}
}
