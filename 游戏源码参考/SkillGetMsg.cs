using System;
using System.Collections.Generic;
using TMProOld;
using UnityEngine;

public class SkillGetMsg : UIMsgBase<ToolItemSkill>
{
	[Space]
	[SerializeField]
	private Transform crestGroup;

	[SerializeField]
	private Vector2 baseCrestPos;

	[SerializeField]
	private SpriteRenderer crestSprite;

	[SerializeField]
	private SpriteRenderer crestGlowSprite;

	[Space]
	[SerializeField]
	private SpriteRenderer skillSprite;

	[SerializeField]
	private SpriteRenderer skillGlowSprite;

	[SerializeField]
	private SpriteRenderer skillSilhouetteSprite;

	[SerializeField]
	private SpriteRenderer skillIconSprite;

	[Space]
	[SerializeField]
	private TMP_Text prefixText;

	[SerializeField]
	private TMP_Text nameText;

	[SerializeField]
	private TMP_Text descText;

	private bool wasEquipped;

	private static readonly int _hideNoEquipProp = Animator.StringToHash("Hide NoEquip");

	protected override int GetHideAnimId()
	{
		if (!wasEquipped)
		{
			return _hideNoEquipProp;
		}
		return base.GetHideAnimId();
	}

	protected override void Setup(ToolItemSkill skill)
	{
		skillSprite.sprite = skill.PromptSprite;
		skillGlowSprite.sprite = skill.PromptGlowSprite;
		skillSilhouetteSprite.sprite = skill.PromptSilhouetteSprite;
		skillIconSprite.sprite = skill.InventorySpriteBase;
		prefixText.text = skill.MsgGetPrefix;
		nameText.text = skill.DisplayName;
		descText.text = skill.PromptDescription;
		ToolCrest crestByName = ToolItemManager.GetCrestByName(PlayerData.instance.CurrentCrestID);
		if (!crestByName)
		{
			return;
		}
		crestSprite.sprite = crestByName.CrestSprite;
		crestGlowSprite.sprite = crestByName.CrestGlow;
		int num = -1;
		List<ToolCrestsData.SlotData> slots = crestByName.SaveData.Slots;
		for (int i = 0; i < slots.Count; i++)
		{
			if (!(slots[i].EquippedTool != skill.name))
			{
				num = i;
				break;
			}
		}
		wasEquipped = num >= 0 && num < crestByName.Slots.Length;
		if (wasEquipped)
		{
			ToolCrest.SlotInfo slotInfo = crestByName.Slots[num];
			crestGroup.SetLocalPosition2D(baseCrestPos - slotInfo.Position);
		}
	}

	public static void Spawn(SkillGetMsg prefab, ToolItemSkill skill, Action afterMsg)
	{
		SkillGetMsg msg = null;
		PlayerData.instance.ToolPaneHasNew = true;
		msg = UIMsgBase<ToolItemSkill>.Spawn(skill, prefab, AfterMsg) as SkillGetMsg;
		if ((bool)msg)
		{
			msg.transform.SetLocalPosition2D(Vector2.zero);
			GameCameras.instance.HUDOut();
			HeroController.instance.AddInputBlocker(msg);
		}
		void AfterMsg()
		{
			HeroController.instance.RemoveInputBlocker(msg);
			if (afterMsg != null)
			{
				afterMsg();
			}
			else
			{
				GameCameras.instance.HUDIn();
			}
			EventRegister.SendEvent("SKILL GET MSG ENDED");
		}
	}

	public void ReportBackgroundFadedOut()
	{
		EventRegister.SendEvent("SKILL GET MSG FADED OUT");
	}
}
