using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class BatchKillEvent : MonoBehaviour
{
	[SerializeField]
	private EventBase eventSource;

	[SerializeField]
	private List<HealthManager> healthManagers = new List<HealthManager>();

	[SerializeField]
	private bool removeNullOnEvent = true;

	private void Awake()
	{
		bool flag = eventSource;
		if (!flag)
		{
			eventSource = GetComponent<EventBase>();
			flag = eventSource;
		}
		if (flag)
		{
			eventSource.ReceivedEvent += OnReceivedEvent;
		}
		if (!removeNullOnEvent)
		{
			healthManagers.RemoveAll((HealthManager o) => o == null);
		}
	}

	private void OnValidate()
	{
		if (eventSource == null)
		{
			eventSource = GetComponent<EventBase>();
		}
	}

	[ContextMenu("Gather Health Managers")]
	private void GatherHealthManagers()
	{
		healthManagers.RemoveAll((HealthManager o) => o == null);
		healthManagers = healthManagers.Union(base.gameObject.GetComponentsInChildren<HealthManager>(includeInactive: true)).ToList();
	}

	private void OnReceivedEvent()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (removeNullOnEvent)
		{
			healthManagers.RemoveAll((HealthManager o) => o == null);
		}
		for (int num = healthManagers.Count - 1; num >= 0; num--)
		{
			HealthManager healthManager = healthManagers[num];
			if (!healthManager.isDead)
			{
				healthManager.Die(healthManager.GetAttackDirection(), AttackTypes.Generic, NailElements.None, null, ignoreEvasion: false, 1f, overrideSpecialDeath: true);
			}
		}
	}
}
