using UnityEngine;

public class GameObjectMatchActiveState : MonoBehaviour
{
	[SerializeField]
	private GameObject setStateOn;

	[SerializeField]
	private GameObject readStateFrom;

	private void OnEnable()
	{
		if ((bool)readStateFrom && !readStateFrom.activeInHierarchy)
		{
			base.gameObject.SetActive(value: false);
		}
		if ((bool)setStateOn)
		{
			setStateOn.SetActive(value: true);
		}
	}

	private void OnDisable()
	{
		if ((bool)setStateOn)
		{
			setStateOn.SetActive(value: false);
		}
	}
}
