using UnityEngine;

public class DeactivateRandomly : MonoBehaviour
{
	public float deactivationChance = 50f;

	private void Start()
	{
		if ((float)Random.Range(1, 100) <= deactivationChance)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
