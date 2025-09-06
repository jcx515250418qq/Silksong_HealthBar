using TeamCherry.SharedUtils;
using UnityEngine;

public class MeshSortingOrder : MonoBehaviour
{
	[SerializeField]
	[HideInInspector]
	private string layerName;

	[SerializeField]
	[SortingLayer]
	private int layerID;

	[SerializeField]
	private int order;

	private MeshRenderer rend;

	private void OnValidate()
	{
		if (!string.IsNullOrEmpty(layerName))
		{
			layerID = SortingLayer.NameToID(layerName);
			layerName = null;
		}
		rend = GetComponent<MeshRenderer>();
		rend.sortingLayerID = layerID;
		rend.sortingOrder = order;
	}

	private void Awake()
	{
		OnValidate();
	}

	private void OnEnable()
	{
		ComponentSingleton<MeshSortingOrderCallbackHooks>.Instance.OnUpdate += OnUpdate;
	}

	private void OnDisable()
	{
		ComponentSingleton<MeshSortingOrderCallbackHooks>.Instance.OnUpdate -= OnUpdate;
	}

	private void OnUpdate()
	{
		if (rend.sortingLayerID != layerID)
		{
			rend.sortingLayerID = layerID;
		}
		if (rend.sortingOrder != order)
		{
			rend.sortingOrder = order;
		}
	}
}
