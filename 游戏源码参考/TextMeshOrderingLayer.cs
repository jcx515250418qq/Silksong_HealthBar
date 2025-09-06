using UnityEngine;

public class TextMeshOrderingLayer : MonoBehaviour
{
	private void Start()
	{
		GetComponent<MeshRenderer>().sortingLayerID = base.transform.parent.GetComponent<SpriteRenderer>().sortingLayerID;
		GetComponent<MeshRenderer>().sortingOrder = base.transform.parent.GetComponent<SpriteRenderer>().sortingOrder;
	}
}
