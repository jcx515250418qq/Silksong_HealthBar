using UnityEngine;

public sealed class DisableIfNotVisible : MonoBehaviour
{
	[SerializeField]
	private VisibilityEvent visibilityEvent;

	[SerializeField]
	private float disableDelay = 2f;

	private float timer;

	private void Awake()
	{
		if (visibilityEvent == null)
		{
			visibilityEvent = GetComponent<VisibilityEvent>();
			if (visibilityEvent == null)
			{
				visibilityEvent = base.gameObject.AddComponent<VisibilityGroup>();
			}
		}
		visibilityEvent.OnVisibilityChanged += OnVisibilityChanged;
	}

	private void Start()
	{
		UpdateVisibility(visibilityEvent.IsVisible);
	}

	private void OnValidate()
	{
		if (visibilityEvent == null)
		{
			visibilityEvent = GetComponent<VisibilityEvent>();
		}
	}

	private void LateUpdate()
	{
		timer += Time.deltaTime;
		if (timer >= disableDelay)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void OnVisibilityChanged(bool visible)
	{
		UpdateVisibility(visible);
	}

	private void UpdateVisibility(bool visible)
	{
		base.enabled = !visible;
		timer = 0f;
	}
}
