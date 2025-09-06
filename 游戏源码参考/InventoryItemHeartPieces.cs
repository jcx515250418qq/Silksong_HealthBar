using System;
using TeamCherry.Localization;
using UnityEngine;

public class InventoryItemHeartPieces : InventoryItemSelectableDirectional
{
	[Serializable]
	private struct DisplayState
	{
		public GameObject DisplayObject;

		public LocalisedString DisplayName;

		public LocalisedString Description;
	}

	[Space]
	[SerializeField]
	private DisplayState[] displayStates;

	private DisplayState currentState;

	public override string DisplayName => currentState.DisplayName;

	public override string Description => currentState.Description;

	protected override void Awake()
	{
		base.Awake();
		InventoryPane componentInParent = GetComponentInParent<InventoryPane>();
		if ((bool)componentInParent)
		{
			componentInParent.OnPaneStart += UpdateState;
		}
	}

	protected override void Start()
	{
		base.Start();
		UpdateState();
	}

	private void UpdateState()
	{
		PlayerData instance = PlayerData.instance;
		int num = instance.heartPieces;
		if (num <= 0 && instance.maxHealthBase <= 5)
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: false);
			}
			return;
		}
		bool flag = false;
		if (instance.maxHealthBase >= 10)
		{
			num = 4;
			flag = true;
		}
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		DisplayState[] array = displayStates;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].DisplayObject.SetActive(value: false);
		}
		int num2 = displayStates.Length - 1;
		for (int j = 0; j < displayStates.Length; j++)
		{
			DisplayState displayState = displayStates[j];
			if (!flag)
			{
				if (j <= num)
				{
					displayState.DisplayObject.SetActive(value: true);
				}
			}
			else if (j == num2)
			{
				displayState.DisplayObject.SetActive(value: true);
				currentState = displayState;
			}
			if (j == num)
			{
				currentState = displayState;
			}
			displayState.DisplayObject.transform.localScale = ((num == 1) ? new Vector3(-1f, 1f, 1f) : Vector3.one);
		}
	}
}
