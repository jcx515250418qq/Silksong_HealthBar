using System.Collections.Generic;
using UnityEngine;

public class BuildEquippedCharms : MonoBehaviour
{
	public Color notchFullColor;

	public Color notchOverColor;

	public List<int> equippedCharms;

	public List<GameObject> gameObjectList;

	public List<GameObject> instanceList;

	private PlayerData pd;

	private GameObject textNotches;

	public GameObject nextDot;

	public GameObject charmsFolder;

	private GameManager gm;

	public int charmSlots;

	public int charmSlotsFilled;

	public int equippedAmount;

	public int uiItems;

	private float START_X = -7.28f;

	private float START_Y = -3.86f;

	private float CHARM_SCALE = 1.15f;

	private float CHARM_DISTANCE_X = 1.76f;

	private void Start()
	{
	}

	private void BuildCharmList()
	{
		if (gm == null)
		{
			gm = GameManager.instance;
		}
		if (pd == null)
		{
			pd = PlayerData.instance;
		}
		uiItems = 0;
		equippedAmount = 0;
		int num = 0;
		float num2 = ((num < 9) ? CHARM_DISTANCE_X : (num switch
		{
			9 => 1.7f, 
			10 => 1.5f, 
			_ => 1.4f, 
		}));
		instanceList = new List<GameObject>();
		float num3 = START_X;
		for (int i = 0; i < equippedCharms.Count; i++)
		{
			GameObject gameObject = Object.Instantiate(gameObjectList[equippedCharms[i] - 1]);
			gameObject.transform.position = new Vector3(num3, START_Y, -10f);
			gameObject.transform.SetParent(charmsFolder.transform, worldPositionStays: false);
			gameObject.transform.localScale = new Vector3(CHARM_SCALE, CHARM_SCALE, CHARM_SCALE);
			gameObject.name = equippedCharms[i].ToString();
			gameObject.GetComponent<CharmItem>().listNumber = i + 1;
			instanceList.Add(gameObject);
			num3 += num2;
		}
		uiItems = instanceList.Count;
		uiItems++;
		instanceList.Add(nextDot);
		nextDot.transform.localPosition = new Vector3(num3, START_Y, -6f);
		nextDot.GetComponent<CharmItem>().listNumber = instanceList.Count + 1;
		UpdateNotches();
	}

	public void UpdateNotches()
	{
	}

	public GameObject GetObjectAt(int listNumber)
	{
		return instanceList[listNumber - 1];
	}

	public int GetUICount()
	{
		return uiItems;
	}

	public string GetItemName(int itemNum)
	{
		return instanceList[itemNum - 1].name;
	}
}
