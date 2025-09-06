using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TinyMossFly : MonoBehaviour
{
	private Rigidbody2D body;

	[SerializeField]
	private float waitMin;

	[SerializeField]
	private float waitMax;

	[SerializeField]
	private float speedMax;

	[SerializeField]
	private float accelerationMax;

	[SerializeField]
	private float roamingRange;

	[SerializeField]
	private float dampener;

	[SerializeField]
	private bool faceDirection;

	private Vector2 start2D;

	private Vector2 acceleration2D;

	private float waitTimer;

	private bool flyingAway;

	private bool songMode;

	private float flyAwaySpeed;

	private float flyAwaySpeedY;

	private float accelerator;

	private float endTimer = 3f;

	private float startX;

	private float startY;

	private const float InspectorAccelerationConstant = 2000f;

	protected void Reset()
	{
		waitMin = 0.75f;
		waitMax = 1f;
		speedMax = 1.75f;
		accelerationMax = 15f;
		roamingRange = 1f;
		dampener = 1.125f;
	}

	protected void Awake()
	{
		body = GetComponent<Rigidbody2D>();
	}

	protected void Start()
	{
		start2D = body.position;
		acceleration2D = Vector2.zero;
		Buzz(0f);
	}

	protected void FixedUpdate()
	{
		float deltaTime = Time.deltaTime;
		if (!flyingAway && !songMode)
		{
			Buzz(deltaTime);
		}
		if (songMode)
		{
			Vector3 position = new Vector3(startX + Random.Range(-0.06f, 0.06f), startY + Random.Range(-0.06f, 0.06f), base.transform.position.z);
			base.transform.position = position;
		}
	}

	protected void Update()
	{
		if (faceDirection)
		{
			if (body.linearVelocity.x < 0f && base.transform.localScale.x > 0f)
			{
				base.transform.localScale = new Vector3(base.transform.localScale.x * -1f, base.transform.localScale.y, base.transform.localScale.z);
			}
			else if (body.linearVelocity.x > 0f && base.transform.localScale.x < 0f)
			{
				base.transform.localScale = new Vector3(base.transform.localScale.x * -1f, base.transform.localScale.y, base.transform.localScale.z);
			}
		}
		if (flyingAway)
		{
			body.linearVelocity = new Vector2(flyAwaySpeed, flyAwaySpeedY);
			flyAwaySpeed += accelerator * Time.deltaTime;
			if (endTimer > 0f)
			{
				endTimer -= Time.deltaTime;
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}

	private void Buzz(float deltaTime)
	{
		Vector2 position = body.position;
		Vector2 linearVelocity = body.linearVelocity;
		bool flag;
		if (waitTimer <= 0f)
		{
			flag = true;
			waitTimer = Random.Range(waitMin, waitMax);
		}
		else
		{
			waitTimer -= deltaTime;
			flag = false;
		}
		for (int i = 0; i < 2; i++)
		{
			float num = linearVelocity[i];
			float num2 = start2D[i];
			float num3 = position[i] - num2;
			float num4 = acceleration2D[i];
			if (flag)
			{
				num4 = ((!(Mathf.Abs(num3) > roamingRange)) ? Random.Range(0f - accelerationMax, accelerationMax) : ((0f - Mathf.Sign(num3)) * accelerationMax));
				num4 /= 2000f;
			}
			else if (Mathf.Abs(num3) > roamingRange && num3 > 0f == num > 0f)
			{
				num4 = accelerationMax * (0f - Mathf.Sign(num3)) / 2000f;
				num /= dampener;
				waitTimer = Random.Range(waitMin, waitMax);
			}
			num += num4;
			num = Mathf.Clamp(num, 0f - speedMax, speedMax);
			linearVelocity[i] = num;
			acceleration2D[i] = num4;
		}
		body.linearVelocity = linearVelocity;
	}

	public void FlyAway()
	{
		flyAwaySpeed = Random.Range(3f, 5f) * base.transform.localScale.x;
		flyAwaySpeedY = Random.Range(1f, 5f);
		accelerator = Random.Range(8f, 11f) * base.transform.localScale.x;
		flyingAway = true;
	}

	public void SongStart()
	{
		startX = base.transform.position.x;
		startY = base.transform.position.y;
		if (base.transform.localScale.x > 0f)
		{
			base.transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(12f, 14f));
		}
		else
		{
			base.transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(-12f, -14f));
		}
		songMode = true;
	}

	public void SongStop()
	{
		base.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		songMode = false;
	}
}
