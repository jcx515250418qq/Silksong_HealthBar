using UnityEngine;

public class Rb2dFollowWithVelocity : MonoBehaviour
{
	[SerializeField]
	private Rigidbody2D body;

	[SerializeField]
	private Transform target;

	[SerializeField]
	private bool deparent;

	[Space]
	[SerializeField]
	[Range(0f, 1f)]
	private float followLerpX = 1f;

	[SerializeField]
	[Range(0f, 1f)]
	private float followLerpY = 1f;

	private Vector2 initialTargetPos;

	private Vector2 initialOffset;

	private TargetJoint2D joint;

	public Transform Target => target;

	private void Reset()
	{
		body = GetComponent<Rigidbody2D>();
	}

	private void Awake()
	{
		if (deparent && (bool)base.transform.parent)
		{
			base.transform.SetParent(null, worldPositionStays: true);
		}
	}

	private void Start()
	{
		initialTargetPos = target.position;
		initialOffset = body.position - initialTargetPos;
		joint = body.GetComponent<TargetJoint2D>();
		if ((bool)joint)
		{
			joint.autoConfigureTarget = false;
		}
	}

	private void FixedUpdate()
	{
		Vector2 vector = default(Vector2);
		if (followLerpX > 0.99f && followLerpY > 0.99f)
		{
			vector = target.position;
		}
		else
		{
			Vector3 position = target.position;
			vector.x = Mathf.Lerp(initialTargetPos.x, position.x, followLerpX);
			vector.y = Mathf.Lerp(initialTargetPos.y, position.y, followLerpY);
		}
		Vector2 vector2 = vector + initialOffset;
		if ((bool)joint)
		{
			joint.target = vector2;
			return;
		}
		Vector2 linearVelocity = (vector2 - body.position) / Time.deltaTime;
		body.linearVelocity = linearVelocity;
	}
}
