using System;
using TeamCherry.Localization;
using UnityEngine;

public class SteelSoulQuestSpot : PlayMakerNPC
{
	[Serializable]
	public class Spot
	{
		public string SceneName;

		public bool IsSeen;
	}

	public const int SPOT_COUNT = 3;

	[Space]
	[SerializeField]
	private FullQuestBase quest;

	[SerializeField]
	private GameObject presentChild;

	[SerializeField]
	private GameObject notPresentChild;

	[SerializeField]
	private GameObject inspectPlink;

	[Space]
	[SerializeField]
	private LocalisedString[] orderedDialogues;

	private Spot thisSpot;

	private Coroutine pickupRoutine;

	protected override void OnValidate()
	{
		base.OnValidate();
		LocalisedString[] array = orderedDialogues;
		if (array != null && array.Length == 3)
		{
			return;
		}
		LocalisedString[] array2 = orderedDialogues;
		orderedDialogues = new LocalisedString[3];
		if (array2 != null)
		{
			for (int i = 0; i < Mathf.Min(3, array2.Length); i++)
			{
				orderedDialogues[i] = array2[i];
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		presentChild.SetActive(value: false);
		notPresentChild.SetActive(value: false);
		inspectPlink.SetActive(value: false);
		Deactivate(allowQueueing: false);
	}

	protected override void Start()
	{
		base.Start();
		if (!quest.IsAccepted && !quest.IsCompleted)
		{
			SetPresent(value: false);
			return;
		}
		GameManager instance = GameManager.instance;
		PlayerData playerData = instance.playerData;
		string sceneNameString = instance.GetSceneNameString();
		Spot[] steelQuestSpots = playerData.SteelQuestSpots;
		foreach (Spot spot in steelQuestSpots)
		{
			if (!(spot.SceneName != sceneNameString))
			{
				thisSpot = spot;
				break;
			}
		}
		if (thisSpot == null)
		{
			SetPresent(value: false);
			return;
		}
		SetPresent(value: true);
		if (!thisSpot.IsSeen)
		{
			inspectPlink.SetActive(value: true);
			Activate();
		}
	}

	private void SetPresent(bool value)
	{
		presentChild.SetActive(value);
		notPresentChild.SetActive(!value);
	}

	public string GetCurrentDialogue()
	{
		PlayerData instance = PlayerData.instance;
		int num = 0;
		Spot[] steelQuestSpots = instance.SteelQuestSpots;
		for (int i = 0; i < steelQuestSpots.Length; i++)
		{
			if (steelQuestSpots[i].IsSeen)
			{
				num++;
			}
		}
		if (num >= orderedDialogues.Length)
		{
			num = orderedDialogues.Length - 1;
		}
		return orderedDialogues[num];
	}

	public void MarkSeen()
	{
		thisSpot.IsSeen = true;
		inspectPlink.SetActive(value: false);
		Deactivate(allowQueueing: false);
		QuestManager.ShowQuestUpdatedStandalone(quest);
	}
}
