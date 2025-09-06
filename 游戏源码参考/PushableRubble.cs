using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PushableRubble : MonoBehaviour
{
	[SerializeField]
	private Vector2 forceMultiplier = Vector2.one;

	[SerializeField]
	private float torqueMultiplier = 1f;

	[SerializeField]
	private bool dontPositionToGround;

	[Space]
	[SerializeField]
	private AudioEventRandom pushAudio;

	[SerializeField]
	private RandomAudioClipTable pushAudioClipTable;

	[Space]
	[SerializeField]
	private AudioEventRandom hitAudio;

	private Rigidbody2D body;

	private const int GROUND_LAYER_MASK = 256;

	protected void Awake()
	{
		body = GetComponent<Rigidbody2D>();
	}

	private void OnEnable()
	{
		Transform transform = base.transform;
		transform.SetRotation2D(Random.Range(0f, 360f));
		if (dontPositionToGround)
		{
			return;
		}
		Vector3 position = transform.position;
		RaycastHit2D raycastHit2D = Helper.Raycast2D(position, Vector2.down, 10f, 256);
		if ((bool)raycastHit2D)
		{
			Vector2 point = raycastHit2D.point;
			Collider2D component = GetComponent<Collider2D>();
			if ((bool)component)
			{
				float num = position.y - component.bounds.min.y;
				point.y += num;
			}
			transform.SetPosition2D(point);
		}
		body.Sleep();
	}

	protected void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer == 17)
		{
			hitAudio.SpawnAndPlayOneShot(base.transform.position);
		}
		Push();
	}

	private void Push()
	{
		body.AddForce(new Vector2((float)Random.Range(-100, 100) * forceMultiplier.x, (float)Random.Range(0, 40) * forceMultiplier.y), ForceMode2D.Force);
		body.AddTorque((float)Random.Range(-50, 50) * torqueMultiplier, ForceMode2D.Force);
		pushAudio.SpawnAndPlayOneShot(base.transform.position);
		if ((bool)pushAudioClipTable)
		{
			pushAudioClipTable.SpawnAndPlayOneShot(base.transform.position);
		}
	}

	public void EndRubble()
	{
		Invoke("EndRubbleContinuation", 5f);
	}

	private void EndRubbleContinuation()
	{
		body.isKinematic = true;
		body.linearVelocity = Vector2.zero;
		Collider2D component = GetComponent<Collider2D>();
		if (component != null)
		{
			component.enabled = false;
		}
	}
}
