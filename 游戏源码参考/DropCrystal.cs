using UnityEngine;

public class DropCrystal : MonoBehaviour
{
	public float bounceFactor;

	public float speedThreshold = 1f;

	private float speed;

	private float animTimer;

	private Vector2 velocity;

	private Vector2 lastPos;

	private Rigidbody2D rb;

	private int chooser;

	private int stepCounter;

	private float xSpeed;

	private float ySpeed;

	private bool onConveyor;

	private Vector3 startPos;

	private void Start()
	{
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, Random.Range(-0.01f, 0.01f));
		float num = Random.Range(0.4f, 1f);
		base.transform.localScale = new Vector3(num, num, num);
		startPos = base.transform.position;
		rb = GetComponent<Rigidbody2D>();
	}

	public void OnEnable()
	{
		onConveyor = false;
		base.transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
	}

	private void FixedUpdate()
	{
		if (stepCounter >= 10)
		{
			Vector2 vector = new Vector2(base.transform.position.x, base.transform.position.y);
			velocity = vector - lastPos;
			lastPos = vector;
			speed = rb.linearVelocity.magnitude;
			if (base.transform.position.y < 4f)
			{
				base.transform.position = startPos;
				rb.linearVelocity = new Vector2(0f, 0f);
			}
			stepCounter = 0;
		}
		else
		{
			stepCounter++;
		}
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		if (speed > speedThreshold)
		{
			Vector3 inNormal = col.GetSafeContact().Normal;
			Vector3 normalized = Vector3.Reflect(velocity.normalized, inNormal).normalized;
			rb.linearVelocity = new Vector2(normalized.x, normalized.y) * (speed * (bounceFactor * Random.Range(0.8f, 1.2f)));
		}
	}

	private void LateUpdate()
	{
		if (onConveyor && xSpeed != 0f)
		{
			base.transform.position = new Vector3(base.transform.position.x + xSpeed * Time.deltaTime, base.transform.position.y, base.transform.position.z);
		}
	}

	public void StartConveyorMove(float c_xSpeed, float c_ySpeed)
	{
		onConveyor = true;
		xSpeed = c_xSpeed;
		ySpeed = c_ySpeed;
	}

	public void StopConveyorMove()
	{
		onConveyor = false;
	}
}
