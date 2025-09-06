using UnityEngine;
using UnityEngine.Events;

public class BreakWhenNotMoving : MonoBehaviour
{
	[SerializeField]
	private Renderer[] activeRenderers;

	[SerializeField]
	private ParticleSystem[] breakParticles;

	[SerializeField]
	private GameObject[] breakEffects;

	[SerializeField]
	private float movementThreshold;

	[SerializeField]
	private float waitTime;

	public UnityEvent OnBreak;

	private double breakTime;

	private Vector2 previousPosition;

	private bool isBroken;

	private Rigidbody2D body;

	public bool IsBroken => isBroken;

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
	}

	private void OnEnable()
	{
		isBroken = false;
		Renderer[] array = activeRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		ParticleSystem[] array2 = breakParticles;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Stop(withChildren: true);
		}
		breakEffects.SetAllActive(value: false);
		if ((bool)body)
		{
			body.simulated = true;
		}
	}

	private void Update()
	{
		if (isBroken)
		{
			bool flag = false;
			ParticleSystem[] array = breakParticles;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsAlive(withChildren: true))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				base.gameObject.Recycle();
			}
		}
		else
		{
			Vector3 position = base.transform.position;
			Vector2 vector = (Vector2)position - previousPosition;
			previousPosition = position;
			if ((vector / Time.deltaTime).magnitude > movementThreshold)
			{
				breakTime = Time.timeAsDouble + (double)waitTime;
			}
			if (Time.timeAsDouble > breakTime)
			{
				Break();
			}
		}
	}

	public void Break()
	{
		isBroken = true;
		Renderer[] array = activeRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		ParticleSystem[] array2 = breakParticles;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Play(withChildren: true);
		}
		breakEffects.SetAllActive(value: true);
		if ((bool)body)
		{
			body.simulated = false;
		}
		OnBreak.Invoke();
	}
}
