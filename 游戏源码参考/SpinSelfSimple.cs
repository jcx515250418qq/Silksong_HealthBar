using UnityEngine;

public class SpinSelfSimple : MonoBehaviour
{
	public bool randomStartRotation;

	public float spinFactor;

	public bool waitForCall;

	public Rigidbody2D rb;

	private bool timing;

	private float timer;

	private void Update()
	{
		if (timing && !waitForCall)
		{
			if (timer > 0f)
			{
				timer -= Time.deltaTime;
				return;
			}
			timing = false;
			DoSpin();
		}
	}

	private void OnEnable()
	{
		if (rb == null)
		{
			rb = GetComponent<Rigidbody2D>();
		}
		if (randomStartRotation)
		{
			base.transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
		}
		timing = true;
		timer = 0.01f;
	}

	public void DoSpin()
	{
		float torque = 0f - rb.linearVelocity.x * spinFactor;
		rb.AddTorque(torque, ForceMode2D.Force);
	}
}
