using UnityEngine;

public class GeneratePortraitFrame : MonoBehaviour
{
	public GameObject frameObject;

	private void Start()
	{
		GameObject gameObject = base.transform.parent.gameObject;
		GameObject obj = Object.Instantiate(frameObject);
		obj.transform.parent = gameObject.transform;
		obj.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, base.transform.localPosition.z - 0.0001f);
		obj.transform.localScale = new Vector3(1f, 1f, 1f);
		obj.SetActive(value: false);
	}
}
