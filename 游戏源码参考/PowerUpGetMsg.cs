using System;
using GlobalEnums;
using TMProOld;
using TeamCherry.Localization;
using UnityEngine;

public class PowerUpGetMsg : UIMsgBase<PowerUpGetMsg.PowerUps>
{
	public enum PowerUps
	{
		Sprint = 0,
		WallJump = 1,
		HarpoonDash = 2,
		Needolin = 3,
		SuperJump = 4,
		EvaHeal = 5
	}

	[Serializable]
	private struct PowerUpInfo
	{
		public Sprite LineSprite;

		public Sprite SolidSprite;

		public Sprite GlowSprite;

		public Sprite PromptSprite;

		[Space]
		public LocalisedString Prefix;

		public LocalisedString Name;

		public LocalisedString Desc1;

		public LocalisedString Desc2;

		[Space]
		[LocalisedString.NotRequired]
		public LocalisedString PromptButtonText;

		public HeroActionButton PromptButton;

		public int ModifierDirection;
	}

	[Space]
	[SerializeField]
	[ArrayForEnum(typeof(PowerUps))]
	private PowerUpInfo[] powerUpInfos;

	[Space]
	[SerializeField]
	private TMP_Text prefixText;

	[SerializeField]
	private TMP_Text nameText;

	[SerializeField]
	private TMP_Text descTextTop;

	[SerializeField]
	private TMP_Text descTextBot;

	[SerializeField]
	private SpriteRenderer lineSprite;

	[SerializeField]
	private SpriteRenderer solidSprite;

	[SerializeField]
	private SpriteRenderer glowSprite;

	[SerializeField]
	private SpriteRenderer promptSprite;

	[SerializeField]
	private Transform promptGroup;

	[SerializeField]
	private GameObject singleGroup;

	[SerializeField]
	private ActionButtonIcon promptButtonSingle;

	[SerializeField]
	private TMP_Text promptButtonSingleText;

	[SerializeField]
	private GameObject modifierGroup;

	[SerializeField]
	private ActionButtonIcon promptButtonModifier;

	[SerializeField]
	private TMP_Text promptButtonModifierText;

	[SerializeField]
	private GameObject upModifier;

	[SerializeField]
	private GameObject downModifier;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref powerUpInfos, typeof(PowerUps));
	}

	private void Awake()
	{
		OnValidate();
	}

	protected override void Setup(PowerUps skill)
	{
		PowerUpInfo powerUpInfo = powerUpInfos[(int)skill];
		prefixText.text = powerUpInfo.Prefix;
		nameText.text = powerUpInfo.Name;
		descTextTop.text = powerUpInfo.Desc1;
		descTextBot.text = powerUpInfo.Desc2;
		lineSprite.sprite = powerUpInfo.LineSprite;
		solidSprite.sprite = powerUpInfo.SolidSprite;
		glowSprite.sprite = powerUpInfo.GlowSprite;
		promptSprite.sprite = powerUpInfo.PromptSprite;
		if (powerUpInfo.PromptButtonText.IsEmpty)
		{
			promptGroup.gameObject.SetActive(value: false);
			return;
		}
		promptGroup.gameObject.SetActive(value: true);
		if (powerUpInfo.ModifierDirection == 0)
		{
			singleGroup.SetActive(value: true);
			modifierGroup.SetActive(value: false);
			promptButtonSingle.SetAction(powerUpInfo.PromptButton);
			promptButtonSingleText.text = powerUpInfo.PromptButtonText;
		}
		else
		{
			singleGroup.SetActive(value: false);
			modifierGroup.SetActive(value: true);
			promptButtonModifier.SetAction(powerUpInfo.PromptButton);
			promptButtonModifierText.text = powerUpInfo.PromptButtonText;
			upModifier.SetActive(powerUpInfo.ModifierDirection > 0);
			downModifier.SetActive(powerUpInfo.ModifierDirection < 0);
		}
	}

	public static void Spawn(PowerUpGetMsg prefab, PowerUps skill, Action afterMsg)
	{
		PowerUpGetMsg msg = null;
		PlayerData.instance.InvPaneHasNew = true;
		msg = UIMsgBase<PowerUps>.Spawn(skill, prefab, AfterMsg) as PowerUpGetMsg;
		if ((bool)msg)
		{
			msg.transform.SetLocalPosition2D(Vector2.zero);
			GameCameras.instance.HUDOut();
			HeroController.instance.AddInputBlocker(msg);
		}
		void AfterMsg()
		{
			HeroController.instance.RemoveInputBlocker(msg);
			switch (skill)
			{
			case PowerUps.Sprint:
				InventoryCollectableItemSelectionHelper.LastSelectionUpdate = InventoryCollectableItemSelectionHelper.SelectionType.Sprint;
				break;
			case PowerUps.WallJump:
				InventoryCollectableItemSelectionHelper.LastSelectionUpdate = InventoryCollectableItemSelectionHelper.SelectionType.WallJump;
				break;
			case PowerUps.HarpoonDash:
				InventoryCollectableItemSelectionHelper.LastSelectionUpdate = InventoryCollectableItemSelectionHelper.SelectionType.HarpoonDash;
				break;
			case PowerUps.Needolin:
				InventoryCollectableItemSelectionHelper.LastSelectionUpdate = InventoryCollectableItemSelectionHelper.SelectionType.Needolin;
				break;
			case PowerUps.SuperJump:
				InventoryCollectableItemSelectionHelper.LastSelectionUpdate = InventoryCollectableItemSelectionHelper.SelectionType.SuperJump;
				break;
			case PowerUps.EvaHeal:
				InventoryCollectableItemSelectionHelper.LastSelectionUpdate = InventoryCollectableItemSelectionHelper.SelectionType.EvaHeal;
				break;
			}
			if (afterMsg != null)
			{
				afterMsg();
			}
			else
			{
				GameCameras.instance.HUDIn();
			}
		}
	}
}
