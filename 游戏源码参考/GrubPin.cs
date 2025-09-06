using UnityEngine;

public class GrubPin : MonoBehaviour
{
	private PlayerData pd;

	private void Start()
	{
		pd = PlayerData.instance;
	}

	private void OnEnable()
	{
		if (pd == null)
		{
			pd = PlayerData.instance;
		}
	}
}
