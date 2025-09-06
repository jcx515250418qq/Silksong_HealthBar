using System.Collections.Generic;
using UnityEngine;

public class ProgressBarSegmented : MonoBehaviour
{
	[SerializeField]
	private ImageSlider barTemplate;

	private List<ImageSlider> bars;

	public void SetSegmentCount(int count)
	{
		if (bars == null)
		{
			bars = new List<ImageSlider>(count);
		}
		barTemplate.gameObject.SetActive(value: false);
		while (bars.Count < count)
		{
			bars.Add(Object.Instantiate(barTemplate, barTemplate.transform.parent));
		}
		for (int i = 0; i < bars.Count; i++)
		{
			bars[i].gameObject.SetActive(i < count);
		}
	}

	public void SetSegmentProgress(int index, float progress)
	{
		bars[index].Value = progress;
	}
}
