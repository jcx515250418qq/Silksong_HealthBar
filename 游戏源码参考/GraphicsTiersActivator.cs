using UnityEngine;

public class GraphicsTiersActivator : MonoBehaviour
{
	[SerializeField]
	private GameObject disableObj;

	[SerializeField]
	private GameObject altObj;

	[Space]
	[SerializeField]
	private Platform.GraphicsTiers disableThreshold = Platform.GraphicsTiers.Low;

	private void Reset()
	{
		disableObj = base.gameObject;
	}

	private void Awake()
	{
		Platform.GraphicsTierChanged += OnGraphicsTierChanged;
		OnGraphicsTierChanged(Platform.Current.GraphicsTier);
	}

	private void OnDestroy()
	{
		Platform.GraphicsTierChanged -= OnGraphicsTierChanged;
	}

	private void OnGraphicsTierChanged(Platform.GraphicsTiers graphicsTier)
	{
		bool flag = graphicsTier > disableThreshold;
		if ((bool)disableObj)
		{
			disableObj.SetActive(flag);
		}
		if ((bool)altObj)
		{
			altObj.SetActive(!flag);
		}
	}
}
