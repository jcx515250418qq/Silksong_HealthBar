using UnityEngine;

public class EnemyKillEventListener : MonoBehaviour
{
	public EventRegister killEvent;

	private HealthManager healthManager;

	private void Awake()
	{
		healthManager = GetComponent<HealthManager>();
	}

	private void OnEnable()
	{
		if ((bool)killEvent)
		{
			killEvent.ReceivedEvent += Die;
		}
	}

	private void OnDisable()
	{
		if ((bool)killEvent)
		{
			killEvent.ReceivedEvent -= Die;
		}
	}

	private void Die()
	{
		if ((bool)healthManager)
		{
			healthManager.Hit(new HitInstance
			{
				AttackType = AttackTypes.Generic,
				CircleDirection = false,
				DamageDealt = 9999,
				Direction = 0f,
				IgnoreInvulnerable = true,
				Multiplier = 1f
			});
		}
	}
}
