using System.Collections.Generic;
using TeamCherry.Splines;
using UnityEngine;
using UnityEngine.Serialization;

public class SplineBladeTrap : CompoundSpline
{
	private struct BladeTracker
	{
		public Rigidbody2D Blade;

		public float EmergeDelayLeft;

		public float CurrentDistance;
	}

	[SerializeField]
	[FormerlySerializedAs("blade")]
	private Rigidbody2D bladeTemplate;

	[SerializeField]
	private int poolBlades;

	[SerializeField]
	private float bladeEmergeDelay;

	[SerializeField]
	private float bladeSpeed;

	[Space]
	[SerializeField]
	private TrapPressurePlate pressurePlate;

	[SerializeField]
	private float pressurePlateResetTime;

	[Space]
	[SerializeField]
	private ParticleSystem emergeAnticParticles;

	[SerializeField]
	private GameObject bladeEndAudioObject;

	private Queue<Rigidbody2D> bladePool;

	private List<BladeTracker> currentBlades = new List<BladeTracker>();

	private float pressurePlateResetTimer;

	private AudioSource bladeEndAudioSource;

	private void OnValidate()
	{
		if (poolBlades < 1)
		{
			poolBlades = 1;
		}
	}

	private void Awake()
	{
		if ((bool)pressurePlate)
		{
			pressurePlate.OnPressed.AddListener(TriggerBladeMove);
		}
		bladeTemplate.gameObject.SetActive(value: false);
		bladePool = new Queue<Rigidbody2D>(poolBlades);
		for (int i = 0; i < poolBlades; i++)
		{
			bladePool.Enqueue(Object.Instantiate(bladeTemplate, bladeTemplate.transform.parent));
		}
		if ((bool)bladeEndAudioObject)
		{
			bladeEndAudioSource = bladeEndAudioObject.GetComponent<AudioSource>();
		}
	}

	private void Update()
	{
		if (pressurePlateResetTimer > 0f)
		{
			pressurePlateResetTimer -= Time.deltaTime;
			if (pressurePlateResetTimer <= 0f && (bool)pressurePlate)
			{
				pressurePlate.SetBlocked(value: false);
			}
		}
		for (int i = 0; i < currentBlades.Count; i++)
		{
			BladeTracker bladeTracker = currentBlades[i];
			if (!(bladeTracker.EmergeDelayLeft <= 0f))
			{
				bladeTracker.EmergeDelayLeft -= Time.deltaTime;
				if (bladeTracker.EmergeDelayLeft <= 0f)
				{
					bladeTracker.Blade.gameObject.SetActive(value: true);
					bladeTracker.CurrentDistance = 0f;
					UpdateBlade(bladeTracker);
					bladeTracker.Blade.transform.SetPosition2D(bladeTracker.Blade.position);
				}
				currentBlades[i] = bladeTracker;
			}
		}
	}

	private void FixedUpdate()
	{
		for (int num = currentBlades.Count - 1; num >= 0; num--)
		{
			BladeTracker bladeTracker = currentBlades[num];
			bladeTracker.CurrentDistance += bladeSpeed * Time.deltaTime;
			if (bladeTracker.CurrentDistance >= base.TotalDistance)
			{
				if ((bool)bladeEndAudioObject)
				{
					bladeEndAudioObject.transform.position = bladeTracker.Blade.gameObject.transform.position;
					bladeEndAudioSource.Play();
				}
				currentBlades.RemoveAt(num);
				bladeTracker.Blade.gameObject.SetActive(value: false);
				bladePool.Enqueue(bladeTracker.Blade);
			}
			else
			{
				UpdateBlade(bladeTracker);
				currentBlades[num] = bladeTracker;
			}
		}
	}

	public void TriggerBladeMove()
	{
		if (!(pressurePlateResetTimer > 0f))
		{
			pressurePlateResetTimer = pressurePlateResetTime;
			if ((bool)pressurePlate)
			{
				pressurePlate.SetBlocked(value: true);
			}
			emergeAnticParticles.Play();
			Rigidbody2D blade = ((bladePool.Count > 0) ? bladePool.Dequeue() : Object.Instantiate(bladeTemplate, bladeTemplate.transform.parent));
			currentBlades.Add(new BladeTracker
			{
				Blade = blade,
				EmergeDelayLeft = bladeEmergeDelay
			});
		}
	}

	private void UpdateBlade(BladeTracker bladeTracker)
	{
		Vector2 position = bladeTracker.Blade.position;
		Vector2 positionAlongSpline = GetPositionAlongSpline(bladeTracker.CurrentDistance);
		bladeTracker.Blade.position = positionAlongSpline;
		Vector3 to = positionAlongSpline - position;
		to.z = 0f;
		to.Normalize();
		float rotation = Vector3.SignedAngle(Vector3.right, to, Vector3.forward);
		bladeTracker.Blade.rotation = rotation;
	}
}
