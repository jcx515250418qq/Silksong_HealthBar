using UnityEngine;

public class JitterSelfSimple : MonoBehaviour
{
	public float frameTime = 0.1f;

	public Vector2 jitterX;

	public Vector2 jitterY;

	public bool startActive;

	private float startX;

	private float startY;

	private bool up;

	private float timer;

	private bool jittering;

	private void Start()
	{
		Init();
	}

	private void OnEnable()
	{
		Init();
	}

	private void Init()
	{
		startX = base.transform.position.x;
		startY = base.transform.position.y;
		if (startActive)
		{
			jittering = true;
		}
		else
		{
			jittering = false;
		}
	}

	private void Update()
	{
		if (!jittering)
		{
			return;
		}
		if (timer < frameTime)
		{
			timer += Time.deltaTime;
			return;
		}
		if (up)
		{
			base.transform.position = new Vector3(startX + jitterX.x, startY + jitterY.x, base.transform.position.z);
			up = false;
		}
		else
		{
			base.transform.position = new Vector3(startX + jitterX.y, startY + jitterY.y, base.transform.position.z);
			up = true;
		}
		timer -= frameTime;
	}

	public void StartJitter()
	{
		jittering = true;
	}

	public void StopJitter()
	{
		base.transform.position = new Vector3(startX, startY, base.transform.position.z);
		jittering = false;
	}
}
