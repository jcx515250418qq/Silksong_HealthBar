using System.Collections;
using UnityEngine;

public class TestGameObjectActivator : MonoBehaviour, ISceneLintUpgrader
{
	[SerializeField]
	private GameObject activateGameObject;

	[SerializeField]
	private GameObject deactivateGameObject;

	[Space]
	[SerializeField]
	private string activateEventRegister;

	[SerializeField]
	private string deactivateEventRegister;

	[Header("Tests")]
	[SerializeField]
	private PlayerDataTest playerDataTest;

	[Space]
	[SerializeField]
	private QuestTest[] questTests;

	[SerializeField]
	private ToolBase[] equipTests;

	[Space]
	[SerializeField]
	private string[] entryGateWhitelist;

	[SerializeField]
	private string[] entryGateBlacklist;

	[Space]
	[SerializeField]
	private GameObject checkActive;

	[SerializeField]
	private bool expectedActive;

	private bool hasStarted;

	private void Start()
	{
		DoEvaluate();
		hasStarted = true;
	}

	private void OnEnable()
	{
		if (hasStarted)
		{
			DoEvaluate();
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	public void DoEvaluate()
	{
		if ((bool)checkActive)
		{
			StartCoroutine(DelayEvaluate());
		}
		else
		{
			Evaluate();
		}
	}

	private IEnumerator DelayEvaluate()
	{
		yield return null;
		Evaluate();
	}

	private void Evaluate()
	{
		bool flag = playerDataTest.IsFulfilled;
		if (flag)
		{
			QuestTest[] array = questTests;
			foreach (QuestTest questTest in array)
			{
				if (!questTest.IsFulfilled)
				{
					flag = false;
					break;
				}
			}
		}
		if (flag)
		{
			ToolBase[] array2 = equipTests;
			for (int i = 0; i < array2.Length; i++)
			{
				if (!array2[i].IsEquipped)
				{
					flag = false;
					break;
				}
			}
		}
		string entryGateName = GameManager.instance.GetEntryGateName();
		if (flag && entryGateWhitelist.Length != 0 && !entryGateName.IsAny(entryGateWhitelist))
		{
			flag = false;
		}
		if (flag && entryGateName.IsAny(entryGateBlacklist))
		{
			flag = false;
		}
		if ((bool)checkActive && checkActive.activeInHierarchy != expectedActive)
		{
			flag = false;
		}
		if ((bool)activateGameObject)
		{
			activateGameObject.SetActive(flag);
		}
		if ((bool)deactivateGameObject)
		{
			deactivateGameObject.SetActive(!flag);
		}
		EventRegister.SendEvent(flag ? activateEventRegister : deactivateEventRegister);
	}

	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		bool flag = false;
		if ((bool)activateGameObject && activateGameObject.activeSelf && activateGameObject != base.gameObject)
		{
			activateGameObject.SetActive(value: false);
			flag = true;
		}
		if ((bool)deactivateGameObject && deactivateGameObject.activeSelf && deactivateGameObject != base.gameObject)
		{
			deactivateGameObject.SetActive(value: false);
			flag = true;
		}
		if (!flag)
		{
			return null;
		}
		return "TestGameObjectActivator disabled targets";
	}
}
