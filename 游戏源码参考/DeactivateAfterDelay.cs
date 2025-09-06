using UnityEngine;

public class DeactivateAfterDelay : MonoBehaviour
{
	public float time;

	public bool stayInPlace;

	public bool deparent;

	public bool getPositionOnEnable;

	public bool deparentOnDeactivate;

	[SerializeField]
	private bool requireNotVisible;

	[SerializeField]
	private VisibilityEvent visibilityCheck;

	private float timer;

	private Vector3 worldPos;

	private Vector3 startPos;

	private bool hasVisibility;

	private void Awake()
	{
		if (stayInPlace)
		{
			startPos = base.transform.localPosition;
		}
		hasVisibility = visibilityCheck != null;
	}

	private void OnEnable()
	{
		timer = time;
		if (stayInPlace)
		{
			if (getPositionOnEnable)
			{
				startPos = base.transform.localPosition;
			}
			base.transform.localPosition = startPos;
			worldPos = base.transform.position;
		}
		if (deparent)
		{
			base.transform.parent = null;
		}
		if (time == 0f)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (timer > 0f)
		{
			timer -= Time.deltaTime;
			if (stayInPlace)
			{
				base.transform.position = worldPos;
			}
		}
		else if (!requireNotVisible || !hasVisibility || !visibilityCheck.IsVisible)
		{
			base.gameObject.SetActive(value: false);
			if (deparentOnDeactivate)
			{
				base.transform.parent = null;
			}
		}
	}

	public void SetTime(float newTime)
	{
		time = newTime;
		timer = time;
	}
}
