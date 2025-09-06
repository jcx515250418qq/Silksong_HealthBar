using UnityEngine;

public class ShellFossilMimicSpawn : MonoBehaviour
{
	public GameObject mimicPrefab;

	public PersistentIntItem intItemReference;

	private bool didCheck;

	private void LateUpdate()
	{
		if (!didCheck)
		{
			if (intItemReference.GetCurrentValue() == -10)
			{
				Object.Instantiate(mimicPrefab, base.transform.position, base.transform.rotation).transform.SetParent(base.transform);
			}
			didCheck = true;
		}
	}
}
