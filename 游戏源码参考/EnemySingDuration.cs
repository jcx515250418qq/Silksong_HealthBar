using UnityEngine;

public class EnemySingDuration : MonoBehaviour
{
	private const float COOLDOWN_TIME_MIN = 3f;

	private const float COOLDOWN_TIME_MAX = 5f;

	private float cooldownTimer;

	private void Update()
	{
		if (cooldownTimer > 0f)
		{
			cooldownTimer -= Time.deltaTime;
		}
	}

	public void StartSingCooldown()
	{
		cooldownTimer = Random.Range(3f, 5f);
	}

	public bool CheckCanSing()
	{
		return cooldownTimer <= 0f;
	}

	public void ResetSingCooldown()
	{
		cooldownTimer = 0f;
	}
}
