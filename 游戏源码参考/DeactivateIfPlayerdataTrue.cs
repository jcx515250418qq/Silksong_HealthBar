using UnityEngine;

public class DeactivateIfPlayerdataTrue : MonoBehaviour
{
	[PlayerDataField(typeof(bool), true)]
	public string boolName;

	public GameObject objectToDeactivate;

	[SerializeField]
	private bool waitForStart;

	private bool hasStarted;

	private void Start()
	{
		hasStarted = true;
		ForceEvaluate();
	}

	private void OnEnable()
	{
		if (!waitForStart || hasStarted)
		{
			ForceEvaluate();
		}
	}

	private void ForceEvaluate()
	{
		if (PlayerData.instance.GetBool(boolName))
		{
			if ((bool)objectToDeactivate)
			{
				objectToDeactivate.SetActive(value: false);
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
