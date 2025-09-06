using System.Collections.Generic;
using UnityEngine;

public class GodfinderGateIconManager : MonoBehaviour
{
	public GodfinderGateIcon[] gateIcons;

	public float offsetX = 8f;

	private void OnValidate()
	{
		DoLayout();
	}

	private void OnEnable()
	{
		GodfinderGateIcon[] array = gateIcons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Evaluate();
		}
		DoLayout();
	}

	private void DoLayout()
	{
		Vector3 vector = base.transform.position + new Vector3((0f - offsetX) / 2f, 0f);
		Vector3 vector2 = base.transform.position + new Vector3(offsetX / 2f, 0f);
		List<GodfinderGateIcon> list = new List<GodfinderGateIcon>();
		GodfinderGateIcon[] array = gateIcons;
		foreach (GodfinderGateIcon godfinderGateIcon in array)
		{
			if (godfinderGateIcon.gameObject.activeSelf)
			{
				list.Add(godfinderGateIcon);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			if ((bool)list[j])
			{
				list[j].transform.position = Vector2.Lerp(vector, vector2, (float)j / (float)(list.Count - 1));
			}
		}
	}
}
