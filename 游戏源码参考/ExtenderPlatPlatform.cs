using UnityEngine;

public class ExtenderPlatPlatform : MonoBehaviour
{
	private void Awake()
	{
		if (!GetComponentInParent<ExtenderPlatsController>())
		{
			TiltPlat component = GetComponent<TiltPlat>();
			if ((bool)component)
			{
				component.ActivateTiltPlat(isInstant: true);
			}
		}
	}
}
