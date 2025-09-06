using UnityEngine;

public sealed class PinChallenge : MonoBehaviour
{
	[SerializeField]
	private InteractableBase restBenchInteractable;

	[SerializeField]
	private float pauseDuration = 2f;

	private bool isActive;

	private bool hasRestBench;

	private bool pausedInteractable;

	private float timer;

	private void Awake()
	{
		EventRegister.GetRegisterGuaranteed(base.gameObject, "PIN CHALLENGE START").ReceivedEvent += OnPinChallengeStart;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "PIN CHALLENGE END").ReceivedEvent += OnPinChallengeEnd;
		ToolItemManager.BoundAttackToolUsed += ToolItemManagerOnBoundAttackToolUsed;
		hasRestBench = restBenchInteractable;
		base.enabled = false;
	}

	private void OnValidate()
	{
		hasRestBench = restBenchInteractable;
	}

	private void OnDestroy()
	{
		ToolItemManager.BoundAttackToolUsed -= ToolItemManagerOnBoundAttackToolUsed;
	}

	private void Update()
	{
		timer -= Time.deltaTime;
		if (timer <= 0f)
		{
			ToggleBenchPause(active: false);
		}
	}

	private void OnPinChallengeStart()
	{
		isActive = true;
	}

	private void OnPinChallengeEnd()
	{
		isActive = false;
		ToggleBenchPause(active: false);
	}

	private void ToolItemManagerOnBoundAttackToolUsed(AttackToolBinding binding)
	{
		if (isActive && hasRestBench)
		{
			ToggleBenchPause(active: true);
		}
	}

	private void ToggleBenchPause(bool active)
	{
		if (pausedInteractable != active)
		{
			pausedInteractable = active;
			if (pausedInteractable)
			{
				restBenchInteractable.Deactivate(allowQueueing: false);
				timer = pauseDuration;
				base.enabled = true;
			}
			else
			{
				restBenchInteractable.Activate();
				base.enabled = false;
			}
		}
	}
}
