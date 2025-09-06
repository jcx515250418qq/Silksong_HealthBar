using UnityEngine;

public class BreakableInfectedVine : MonoBehaviour
{
	public GameObject[] blobs;

	[Space]
	public GameObject[] effects;

	[Space]
	public int spatterAmount = 5;

	public float spatterSpeedMin = 10f;

	public float spatterSpeedMax = 20f;

	public float spatterAngleMin = 40f;

	public float spatterAngleMax = 140f;

	[Space]
	public float audioPitchMin = 0.8f;

	public float audioPitchMax = 1.1f;

	private bool activated;

	private AudioSource source;

	private VibrationPlayer vibration;

	private void Awake()
	{
		source = GetComponent<AudioSource>();
		vibration = GetComponent<VibrationPlayer>();
	}

	private void Start()
	{
		if (Mathf.Abs(base.transform.position.z - 0.004f) > 1f)
		{
			if ((bool)source)
			{
				source.enabled = false;
			}
			Collider2D component = GetComponent<Collider2D>();
			if ((bool)component)
			{
				component.enabled = false;
			}
			base.enabled = false;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (activated)
		{
			return;
		}
		bool flag = false;
		if (collision.tag == "Nail Attack")
		{
			flag = true;
		}
		else if (collision.tag == "Hero Spell")
		{
			flag = true;
		}
		else if (collision.tag == "HeroBox" && HeroController.instance.cState.superDashing)
		{
			flag = true;
		}
		if (flag)
		{
			GameObject[] array = blobs;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(value: false);
				SpawnSpatters(gameObject.transform.position);
			}
			array = effects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
			if ((bool)source)
			{
				source.pitch = Random.Range(audioPitchMin, audioPitchMax);
				source.Play();
			}
			if ((bool)vibration)
			{
				vibration.Play();
			}
			activated = true;
		}
	}

	private void SpawnSpatters(Vector3 position)
	{
		BloodSpawner.SpawnBlood(position, (short)spatterAmount, (short)spatterAmount, spatterSpeedMin, spatterSpeedMax, spatterAngleMin, spatterAngleMax, null);
	}
}
