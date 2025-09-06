using UnityEngine;

public class FreezeMomentOnEnable : MonoBehaviour
{
	public int freezeMoment;

	private bool scheduledFreeze = true;

	private void OnEnable()
	{
		scheduledFreeze = true;
	}

	private void Update()
	{
		if (scheduledFreeze)
		{
			scheduledFreeze = false;
			DoFreeze();
		}
	}

	private void DoFreeze()
	{
		GameManager.instance.FreezeMoment(freezeMoment);
	}
}
