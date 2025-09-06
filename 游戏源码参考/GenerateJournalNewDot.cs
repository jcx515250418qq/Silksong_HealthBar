using UnityEngine;

public class GenerateJournalNewDot : MonoBehaviour
{
	public GameObject newDotObject;

	private void Start()
	{
		GameObject obj = Object.Instantiate(newDotObject);
		obj.transform.parent = base.transform;
		obj.transform.localPosition = new Vector3(-0.65f, 0f, -0.0001f);
		obj.SetActive(value: false);
	}
}
