using TeamCherry.SharedUtils;
using UnityEngine;

public class AmbientSway : MonoBehaviour
{
	[SerializeField]
	[AssetPickerDropdown]
	private AmbientSwayProfile profile;

	[SerializeField]
	private bool active = true;

	private float time;

	private float updateOffset;

	private float timeOffset;

	private float nextUpdateTime;

	private Vector3 initialPosition;

	private bool started;

	private void Start()
	{
		initialPosition = base.transform.localPosition;
		if (active)
		{
			timeOffset = Random.Range(-10f, 10f);
		}
		updateOffset = Random.Range(0f, 1f / profile.Fps);
		started = true;
		ComponentSingleton<AmbientSwayCallbackHooks>.Instance.OnUpdate += OnUpdate;
	}

	private void OnEnable()
	{
		if (started)
		{
			ComponentSingleton<AmbientSwayCallbackHooks>.Instance.OnUpdate += OnUpdate;
		}
		if (!profile)
		{
			base.enabled = false;
		}
	}

	private void OnDisable()
	{
		ComponentSingleton<AmbientSwayCallbackHooks>.Instance.OnUpdate -= OnUpdate;
	}

	private void OnUpdate()
	{
		if (!active)
		{
			return;
		}
		time += Time.deltaTime;
		float num = time + updateOffset;
		if (profile.Fps > 0f)
		{
			if (num < nextUpdateTime)
			{
				return;
			}
			nextUpdateTime = num + 1f / profile.Fps;
		}
		base.transform.localPosition = initialPosition + profile.GetOffset(time, timeOffset);
	}

	public void RecordInitialPosition()
	{
		initialPosition = base.transform.localPosition;
		time = 0f;
		nextUpdateTime = 0f;
	}

	public void StartSway()
	{
		initialPosition = base.transform.localPosition;
		ContinueSway();
	}

	public void ContinueSway()
	{
		time = 0f;
		timeOffset = 0f;
		active = true;
		initialPosition = base.transform.localPosition;
		nextUpdateTime = 0f;
	}

	public void ResumeSway()
	{
		active = true;
	}

	public void StopSway()
	{
		active = false;
	}
}
