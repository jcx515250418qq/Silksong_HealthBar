using UnityEngine;

public sealed class ActivateOnDoubleJump : MonoBehaviour
{
	[SerializeField]
	private GameObject target;

	[SerializeField]
	private bool deactivateOnStart;

	private HeroController hc;

	private bool hasTarget;

	private void Start()
	{
		if (target == null)
		{
			target = base.gameObject;
		}
		hasTarget = true;
		hc = HeroController.instance;
		if (hc != null)
		{
			hc.OnDoubleJumped += HcOnDoubleJumped;
		}
		if (deactivateOnStart)
		{
			target.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		if (hc != null)
		{
			hc.OnDoubleJumped -= HcOnDoubleJumped;
		}
	}

	private void HcOnDoubleJumped()
	{
		target.SetActive(value: true);
	}
}
