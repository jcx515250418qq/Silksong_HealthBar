using UnityEngine;

public sealed class RestBenchHelper : MonoBehaviour
{
	private bool heroOnBench;

	private void OnDisable()
	{
		if (!heroOnBench)
		{
			return;
		}
		heroOnBench = false;
		HeroController instance = HeroController.instance;
		if (instance != null)
		{
			PlayerData.instance.atBench = false;
			instance.AffectedByGravity(gravityApplies: true);
			InteractEvents component = base.gameObject.GetComponent<InteractEvents>();
			if ((bool)component)
			{
				component.EndInteraction();
			}
			instance.RegainControl();
			instance.StartAnimationControlToIdle();
		}
	}

	public void SetOnBench(bool onBench)
	{
		heroOnBench = onBench;
	}
}
