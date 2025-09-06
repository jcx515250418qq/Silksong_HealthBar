using UnityEngine;

public class BlueHealth : MonoBehaviour
{
	[SerializeField]
	private PlayMakerFSM controlFsm;

	private HeroController setOnHc;

	private bool isActive;

	public static int ActiveCount { get; private set; }

	private void Reset()
	{
		controlFsm = GetComponent<PlayMakerFSM>();
	}

	private void OnDisable()
	{
		RemovePoison();
		MarkInactive();
	}

	public void MarkActive()
	{
		if (!isActive)
		{
			isActive = true;
			ActiveCount++;
		}
	}

	public void MarkInactive()
	{
		if (isActive)
		{
			isActive = false;
			ActiveCount--;
		}
	}

	public void CheckPoison()
	{
		if (!setOnHc && controlFsm.FsmVariables.FindFsmBool("Is Poison").Value)
		{
			setOnHc = HeroController.instance;
			setOnHc.ReportPoisonHealthAdded();
		}
	}

	public void RemovePoison()
	{
		if ((bool)setOnHc)
		{
			setOnHc.ReportPoisonHealthRemoved();
			setOnHc = null;
		}
	}
}
