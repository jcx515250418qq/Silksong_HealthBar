using TeamCherry.SharedUtils;
using UnityEngine;

public class SpinSelf : MonoBehaviour
{
	[SerializeField]
	private float spinFactor = -7.5f;

	[SerializeField]
	private bool randomiseOnEnable = true;

	private int stepCounter;

	private bool spun;

	private Rigidbody2D body;

	public float SpinFactor
	{
		get
		{
			return spinFactor;
		}
		set
		{
			spinFactor = value;
		}
	}

	public bool RandomiseOnEnable
	{
		get
		{
			return randomiseOnEnable;
		}
		set
		{
			randomiseOnEnable = value;
		}
	}

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
	}

	private void OnEnable()
	{
		ComponentSingleton<SpinSelfCallbackHooks>.Instance.OnFixedUpdate += OnFixedUpdate;
		if (RandomiseOnEnable)
		{
			Transform obj = base.transform;
			Vector3 localEulerAngles = obj.localEulerAngles;
			float? z = Random.Range(0, 360);
			obj.localEulerAngles = localEulerAngles.Where(null, null, z);
		}
		spun = false;
		if (!body)
		{
			base.enabled = false;
		}
	}

	private void OnDisable()
	{
		ComponentSingleton<SpinSelfCallbackHooks>.Instance.OnFixedUpdate -= OnFixedUpdate;
	}

	private void OnFixedUpdate()
	{
		if (!spun)
		{
			if (stepCounter >= 1)
			{
				float torque = body.linearVelocity.x * SpinFactor;
				body.AddTorque(torque);
				spun = true;
			}
			stepCounter++;
		}
	}

	public void ReSpin()
	{
		spun = false;
		stepCounter = 0;
	}

	public void StopBounce()
	{
		spun = true;
		body.angularVelocity = 0f;
	}
}
