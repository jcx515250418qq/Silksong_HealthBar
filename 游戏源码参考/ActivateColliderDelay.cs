using TeamCherry.SharedUtils;
using UnityEngine;

public class ActivateColliderDelay : MonoBehaviour
{
	[SerializeField]
	private Collider2D targetCollider;

	[SerializeField]
	private float time;

	[SerializeField]
	private bool activate;

	[SerializeField]
	[Range(0f, 1f)]
	private float activateChance = 1f;

	[SerializeField]
	private bool activateonEnable;

	private float timer;

	private bool didActivation;

	private void OnEnable()
	{
		ComponentSingleton<ActivateColliderDelayCallbackHooks>.Instance.OnUpdate += OnUpdate;
		timer = time;
		targetCollider.enabled = activateonEnable;
		if (activateChance < 1f)
		{
			didActivation = Random.Range(0f, 1f) > activateChance;
		}
		else
		{
			didActivation = false;
		}
	}

	private void OnDisable()
	{
		ComponentSingleton<ActivateColliderDelayCallbackHooks>.Instance.OnUpdate -= OnUpdate;
	}

	private void OnUpdate()
	{
		if (!didActivation)
		{
			if (timer > 0f)
			{
				timer -= Time.deltaTime;
				return;
			}
			targetCollider.enabled = activate;
			didActivation = true;
		}
	}
}
