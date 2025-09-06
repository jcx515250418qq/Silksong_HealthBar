using TeamCherry.SharedUtils;
using UnityEngine;

public class DeactivateIfPlayerdataFalse : MonoBehaviour, GameMapPinLayout.IEvaluateHook
{
	[PlayerDataField(typeof(bool), true)]
	public string boolName;

	public GameObject objectToDeactivate;

	private bool hasStarted;

	private GameManager gm;

	private void Start()
	{
		hasStarted = true;
		ForceEvaluate();
	}

	private void OnEnable()
	{
		if (hasStarted)
		{
			ForceEvaluate();
		}
	}

	public void ForceEvaluate()
	{
		if (gm == null)
		{
			gm = GameManager.instance;
		}
		if (!gm.playerData.GetVariable<bool>(boolName))
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
