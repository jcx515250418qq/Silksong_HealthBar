using UnityEngine;

public class InstantiateOnAwake : MonoBehaviour
{
	[SerializeField]
	private GameObject template;

	[SerializeField]
	private int count;

	[SerializeField]
	private Vector2 itemOffset;

	[SerializeField]
	private string fsmCounterName;

	[SerializeField]
	private int fsmCounterOffset;

	private void Awake()
	{
		template.SetActive(value: false);
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = Object.Instantiate(template, template.transform.parent);
			gameObject.transform.localPosition = template.transform.localPosition + (Vector3)(itemOffset * i);
			if (!string.IsNullOrEmpty(fsmCounterName))
			{
				gameObject.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmInt(fsmCounterName).Value = fsmCounterOffset + i;
			}
			gameObject.SetActive(value: true);
		}
	}
}
