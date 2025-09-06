using UnityEngine;

public class CopyParentSprite : MonoBehaviour
{
	private void Start()
	{
		GetComponent<SpriteRenderer>().sprite = base.transform.parent.gameObject.GetComponent<SpriteRenderer>().sprite;
	}
}
