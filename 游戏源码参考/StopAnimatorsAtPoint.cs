using UnityEngine;

[RequireComponent(typeof(Animator))]
public class StopAnimatorsAtPoint : MonoBehaviour
{
	public EventRegister stopEvent;

	public EventRegister stopInstantEvent;

	private bool canStop;

	private bool shouldStop;

	public float stopInstantHeight = 1.75f;

	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		if ((bool)stopEvent)
		{
			stopEvent.ReceivedEvent += delegate
			{
				shouldStop = true;
				animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			};
		}
		if ((bool)stopInstantEvent)
		{
			stopInstantEvent.ReceivedEvent += delegate
			{
				animator.enabled = false;
				Vector3 localPosition = base.transform.localPosition;
				localPosition.y = stopInstantHeight;
				base.transform.localPosition = localPosition;
			};
		}
	}

	public void SetCanStop()
	{
		canStop = true;
	}

	public void SetCannotStop()
	{
		canStop = false;
	}

	private void Update()
	{
		if (shouldStop && canStop && animator.enabled)
		{
			animator.enabled = false;
			canStop = false;
			shouldStop = false;
		}
	}
}
