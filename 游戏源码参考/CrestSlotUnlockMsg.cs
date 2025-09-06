using System;
using System.Collections;
using GlobalSettings;
using UnityEngine;

public class CrestSlotUnlockMsg : UIMsgPopupBase<CrestSlotUnlockMsg.SlotUnlockPopupInfo, CrestSlotUnlockMsg>
{
	public struct SlotUnlockPopupInfo : IUIMsgPopupItem
	{
		public ToolItemType SlotType { get; set; }

		public AttackToolBinding AttackBinding { get; set; }

		public UnityEngine.Object GetRepresentingObject()
		{
			return null;
		}
	}

	[Serializable]
	private struct SlotIcons
	{
		[ArrayForEnum(typeof(AttackToolBinding))]
		public RuntimeAnimatorController[] Icons;
	}

	[Space]
	[SerializeField]
	private SpriteRenderer icon;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private float iconAnimateDelay;

	[SerializeField]
	[ArrayForEnum(typeof(ToolItemType))]
	private SlotIcons[] slotTypeIcons;

	private Coroutine animateRoutine;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref slotTypeIcons, typeof(ToolItemType));
		for (int i = 0; i < slotTypeIcons.Length; i++)
		{
			SlotIcons slotIcons = slotTypeIcons[i];
			ArrayForEnumAttribute.EnsureArraySize(ref slotIcons.Icons, typeof(AttackToolBinding));
			slotTypeIcons[i] = slotIcons;
		}
	}

	private void Awake()
	{
		OnValidate();
	}

	public static void Spawn(ToolItemType toolType, AttackToolBinding attackBinding)
	{
		CrestSlotUnlockMsg crestSlotUnlockMsgPrefab = UI.CrestSlotUnlockMsgPrefab;
		if ((bool)crestSlotUnlockMsgPrefab)
		{
			UIMsgPopupBase<SlotUnlockPopupInfo, CrestSlotUnlockMsg>.SpawnInternal(crestSlotUnlockMsgPrefab, new SlotUnlockPopupInfo
			{
				SlotType = toolType,
				AttackBinding = (toolType.IsAttackType() ? attackBinding : AttackToolBinding.Neutral)
			});
		}
	}

	protected override void UpdateDisplay(SlotUnlockPopupInfo item)
	{
		if ((bool)icon)
		{
			icon.color = UI.GetToolTypeColor(item.SlotType);
		}
		if ((bool)animator)
		{
			RuntimeAnimatorController runtimeAnimatorController = slotTypeIcons[(int)item.SlotType].Icons[(int)item.AttackBinding];
			animator.runtimeAnimatorController = runtimeAnimatorController;
			if (animateRoutine != null)
			{
				StopCoroutine(animateRoutine);
				animateRoutine = null;
			}
			animator.enabled = true;
			animator.Play("Unequip", 0, 0f);
			if (iconAnimateDelay > 0f)
			{
				animateRoutine = StartCoroutine(AnimateDelayed());
			}
		}
	}

	private IEnumerator AnimateDelayed()
	{
		yield return null;
		animator.enabled = false;
		yield return new WaitForSeconds(iconAnimateDelay);
		animator.enabled = true;
		animator.Play("Unequip", 0, 0f);
		animateRoutine = null;
	}
}
