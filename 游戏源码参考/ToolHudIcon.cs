using System;
using GlobalSettings;
using UnityEngine;

public class ToolHudIcon : RadialHudIcon
{
	[Space]
	[SerializeField]
	private AttackToolBinding binding;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private RandomAudioClipTable failedAudioTable;

	[Space]
	[SerializeField]
	private SpriteRenderer skillZapIcon;

	[SerializeField]
	private bool updateOnHealthChange;

	private ToolItem previousTool;

	private bool isPoison;

	private bool isZap;

	private bool gotZapIconColor;

	private Color zapIconColor;

	private readonly int animFailedTrigger = Animator.StringToHash("Failed");

	private static readonly int _hueShiftPropId = Shader.PropertyToID("_HueShift");

	public ToolItem CurrentTool { get; private set; }

	public event Action Updated;

	private void Awake()
	{
		ToolItemManager.BoundAttackToolUpdated += OnBoundAttackToolUpdated;
		ToolItemManager.BoundAttackToolFailed += OnBoundAttackToolFailed;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "TOOL EQUIPS CHANGED").ReceivedEvent += base.UpdateDisplay;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "SILK REFRESHED").ReceivedEvent += OnSilkSpoolRefreshed;
		if (updateOnHealthChange)
		{
			EventRegister.GetRegisterGuaranteed(base.gameObject, "HEALTH UPDATE").ReceivedEvent += OnSilkSpoolRefreshed;
			EventRegister.GetRegisterGuaranteed(base.gameObject, "HERO HEALED").ReceivedEvent += OnSilkSpoolRefreshed;
			EventRegister.GetRegisterGuaranteed(base.gameObject, "HERO HEALED TO MAX").ReceivedEvent += OnSilkSpoolRefreshed;
		}
	}

	private void OnDestroy()
	{
		ToolItemManager.BoundAttackToolUpdated -= OnBoundAttackToolUpdated;
		ToolItemManager.BoundAttackToolFailed -= OnBoundAttackToolFailed;
	}

	protected override void OnPreUpdateDisplay()
	{
		previousTool = CurrentTool;
		CurrentTool = ToolItemManager.GetBoundAttackTool(binding, ToolEquippedReadSource.Hud);
		if ((bool)CurrentTool)
		{
			isPoison = CurrentTool.PoisonDamageTicks > 0 && Gameplay.PoisonPouchTool.IsEquippedHud;
			isZap = CurrentTool.ZapDamageTicks > 0 && Gameplay.ZapImbuementTool.IsEquippedHud;
		}
		else
		{
			isPoison = false;
			isZap = false;
		}
		if ((bool)skillZapIcon)
		{
			if (!gotZapIconColor)
			{
				zapIconColor = skillZapIcon.color;
				gotZapIconColor = true;
			}
			if (isZap)
			{
				skillZapIcon.gameObject.SetActive(value: true);
				bool isEmpty = GetIsEmpty();
				skillZapIcon.color = (isEmpty ? zapIconColor.MultiplyElements(inactiveColor) : zapIconColor);
			}
			else
			{
				skillZapIcon.gameObject.SetActive(value: false);
			}
		}
		this.Updated?.Invoke();
	}

	protected override void SetIconColour(SpriteRenderer icon, Color color)
	{
		bool num = CurrentTool.HudSpriteModified == CurrentTool.HudSpriteBase;
		if (num && isPoison && CurrentTool.UsePoisonTintRecolour)
		{
			base.SetIconColour(icon, color * Gameplay.PoisonPouchTintColour);
			if (!icon.sharedMaterial.IsKeywordEnabled("RECOLOUR"))
			{
				icon.material.EnableKeyword("RECOLOUR");
			}
		}
		else
		{
			base.SetIconColour(icon, color);
			if (icon.sharedMaterial.IsKeywordEnabled("RECOLOUR"))
			{
				icon.material.DisableKeyword("RECOLOUR");
			}
		}
		if (num && isPoison)
		{
			if (!icon.sharedMaterial.IsKeywordEnabled("CAN_HUESHIFT"))
			{
				icon.material.EnableKeyword("CAN_HUESHIFT");
			}
			icon.material.SetFloat(_hueShiftPropId, CurrentTool.PoisonHueShift);
		}
		else if (icon.sharedMaterial.IsKeywordEnabled("CAN_HUESHIFT"))
		{
			icon.material.DisableKeyword("CAN_HUESHIFT");
		}
	}

	protected override bool GetIsActive()
	{
		return CurrentTool;
	}

	protected override void GetAmounts(out int amountLeft, out int totalCount)
	{
		PlayerData instance = PlayerData.instance;
		if (CurrentTool.Type == ToolItemType.Skill)
		{
			amountLeft = 0;
			totalCount = 0;
		}
		else
		{
			amountLeft = instance.GetToolData(CurrentTool.name).AmountLeft;
			totalCount = ToolItemManager.GetToolStorageAmount(CurrentTool);
		}
	}

	protected override bool TryGetHudSprite(out Sprite sprite)
	{
		if (CurrentTool is ToolItemSkill toolItemSkill && !GetIsEmpty())
		{
			sprite = toolItemSkill.HudGlowSprite;
			if ((bool)sprite)
			{
				return true;
			}
		}
		sprite = CurrentTool.HudSpriteModified;
		if ((bool)sprite)
		{
			return true;
		}
		sprite = CurrentTool.InventorySpriteModified;
		return false;
	}

	public override bool GetIsEmpty()
	{
		PlayerData instance = PlayerData.instance;
		if (CurrentTool.Type != ToolItemType.Skill)
		{
			if (CurrentTool.IsEmpty)
			{
				return !CurrentTool.UsableWhenEmpty;
			}
			return false;
		}
		return instance.silk < instance.SilkSkillCost;
	}

	protected override bool HasTargetChanged()
	{
		if (CurrentTool is ToolItemSkill)
		{
			return true;
		}
		return CurrentTool != previousTool;
	}

	private void OnBoundAttackToolUpdated(AttackToolBinding otherBinding)
	{
		if (otherBinding == binding)
		{
			UpdateDisplay();
		}
	}

	private void OnBoundAttackToolFailed(AttackToolBinding otherBinding)
	{
		if (otherBinding == binding)
		{
			if ((bool)animator)
			{
				animator.SetTrigger(animFailedTrigger);
			}
			failedAudioTable.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
		}
	}

	private void OnSilkSpoolRefreshed()
	{
		if ((bool)CurrentTool && CurrentTool.Type == ToolItemType.Skill)
		{
			UpdateDisplay();
		}
	}

	public void UpdateDisplayInstant()
	{
		previousTool = null;
		UpdateDisplay();
	}

	protected override bool TryGetBarColour(out Color color)
	{
		if (!CurrentTool)
		{
			return base.TryGetBarColour(out color);
		}
		color = UI.GetToolTypeColor(CurrentTool.Type);
		return true;
	}
}
