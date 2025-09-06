using System.Collections;
using UnityEngine;

public class WeaverLiftButton : MonoBehaviour
{
	private static readonly int IdleAnim = Animator.StringToHash("Idle");

	private static readonly int AppearAnim = Animator.StringToHash("Appear");

	private static readonly int HitAnim = Animator.StringToHash("Hit");

	[SerializeField]
	private HitResponse hitResponder;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private WeaverLift targetLift;

	[SerializeField]
	private TrackTriggerObjects firstAppearRange;

	[SerializeField]
	private float firstAppearDelay;

	[SerializeField]
	private PersistentBoolItem persistent;

	private bool isActive;

	private Coroutine activateWaitRoutine;

	private void Awake()
	{
		if ((bool)hitResponder)
		{
			hitResponder.OnHit.AddListener(OnHit);
		}
		Setup();
		if ((bool)persistent)
		{
			persistent.OnGetSaveState += delegate(out bool value)
			{
				value = isActive;
			};
			persistent.OnSetSaveState += delegate(bool value)
			{
				isActive = value;
				Setup();
			};
		}
	}

	private void Setup()
	{
		if (activateWaitRoutine != null)
		{
			StopCoroutine(activateWaitRoutine);
		}
		if (isActive)
		{
			animator.enabled = true;
			animator.Play(IdleAnim);
			return;
		}
		animator.Play(AppearAnim, 0, 0f);
		animator.Update(0f);
		animator.enabled = false;
		activateWaitRoutine = StartCoroutine(ActivateWait());
	}

	private void OnHit()
	{
		Debug.LogError("Direction button deprecated!", this);
	}

	private bool CanAppear()
	{
		if (targetLift.IsAvailable)
		{
			return targetLift.HasDirections;
		}
		return false;
	}

	private IEnumerator ActivateWait()
	{
		while (!firstAppearRange.IsInside || !CanAppear())
		{
			yield return null;
		}
		yield return new WaitForSeconds(firstAppearDelay);
		isActive = true;
		animator.enabled = true;
		animator.Play(AppearAnim, 0, 0f);
		CaptureAnimationEvent component = animator.GetComponent<CaptureAnimationEvent>();
		if ((bool)component)
		{
			bool isWaiting = true;
			component.EventFiredTemp += delegate
			{
				isWaiting = false;
			};
			while (isWaiting)
			{
				yield return null;
			}
		}
	}
}
