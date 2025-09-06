using UnityEngine;

public class JoinedLevers : MonoBehaviour
{
	private static readonly int _hitAnimParam = Animator.StringToHash("Hit");

	private static readonly int _hitCAnim = Animator.StringToHash("Hit C");

	private static readonly int _hitCcAnim = Animator.StringToHash("Hit CC");

	private static readonly int _activateAnimParam = Animator.StringToHash("Activate");

	private static readonly int _resetAnimParam = Animator.StringToHash("Reset");

	[SerializeField]
	private Lever[] levers;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private PersistentBoolItem readPersistent;

	private bool isForced;

	private void Awake()
	{
		if ((bool)readPersistent)
		{
			readPersistent.OnSetSaveState += OnSetSaveState;
		}
		Lever[] array = levers;
		foreach (Lever lever in array)
		{
			if ((bool)lever)
			{
				lever.OnHit.AddListener(OnAnyLeverHit);
			}
		}
	}

	private void OnAnyLeverHit()
	{
		Lever[] array = levers;
		foreach (Lever lever in array)
		{
			if ((bool)lever)
			{
				lever.SetActivatedInert(value: true);
			}
		}
	}

	private void OnSetSaveState(bool value)
	{
		if (!value)
		{
			return;
		}
		Lever[] array = levers;
		foreach (Lever lever in array)
		{
			if ((bool)lever)
			{
				lever.SetActivatedInert(value: true);
			}
		}
		if ((bool)animator && animator.HasParameter(_activateAnimParam, null))
		{
			animator.SetTrigger(_activateAnimParam);
		}
	}

	public void ResetLevers()
	{
		if (isForced)
		{
			return;
		}
		Lever[] array = levers;
		foreach (Lever lever in array)
		{
			if ((bool)lever)
			{
				lever.SetActivatedInert(value: false);
			}
		}
		if ((bool)animator && animator.HasParameter(_resetAnimParam, null))
		{
			animator.SetTrigger(_resetAnimParam);
		}
	}

	public void SetLeversActivated()
	{
		OnSetSaveState(value: true);
	}

	public void ForceActivateLevers()
	{
		isForced = true;
		OnAnyLeverHit();
		PlayHitAnim();
	}

	public void PlayHitAnim()
	{
		if ((bool)animator)
		{
			int shortNameHash = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
			if (shortNameHash != _hitCAnim && shortNameHash != _hitCcAnim && animator.HasParameter(_hitAnimParam, null))
			{
				animator.SetTrigger(_hitAnimParam);
			}
		}
	}

	public void ForceResetLevers()
	{
		isForced = false;
		ResetLevers();
	}
}
