using System;
using GlobalSettings;
using UnityEngine;

public abstract class InventoryItemToolBase : InventoryItemUpdateable
{
	private const float HOLD_PRESS_TIME = 0.3f;

	private const float RELOAD_TIME = 0.025f;

	[Header("Tool Base")]
	[SerializeField]
	private JitterSelf reloadShakeParent;

	[SerializeField]
	private Animator reloadFailAnimator;

	[SerializeField]
	private RandomAudioClipTable failedAudioTable;

	[SerializeField]
	private GameObject changeEffectPrefab;

	[SerializeField]
	private AudioSource reloadAudioSource;

	[SerializeField]
	private GameObject poisonEquipEffect;

	[SerializeField]
	private GameObject zapEquipEffect;

	[SerializeField]
	private SpriteRenderer zapIcon;

	private float holdTimerLeft;

	private bool isReloading;

	private float reloadTimeElapsed;

	[NonSerialized]
	private InventoryItemToolManager manager;

	private InventoryItemExtraDescription extraDesc;

	private bool wasPoison;

	private bool wasZap;

	private int lastEffectFrame;

	private static readonly int _failedProp = Animator.StringToHash("Failed");

	private static readonly int _changeProp = Animator.StringToHash("Change");

	private static readonly int _hueShiftPropId = Shader.PropertyToID("_HueShift");

	public abstract Sprite Sprite { get; }

	public virtual Color SpriteTint => Color.white;

	public abstract ToolItem ItemData { get; }

	protected override void Awake()
	{
		base.Awake();
		manager = GetComponentInParent<InventoryItemToolManager>();
		extraDesc = GetComponent<InventoryItemExtraDescription>();
		if ((bool)extraDesc)
		{
			extraDesc.ActivatedDesc += delegate(GameObject obj)
			{
				ToolItem itemData = ItemData;
				if ((bool)itemData)
				{
					itemData.SetupExtraDescription(obj);
				}
			};
		}
		if ((bool)poisonEquipEffect)
		{
			poisonEquipEffect.SetActive(value: false);
		}
		if ((bool)zapEquipEffect)
		{
			zapEquipEffect.SetActive(value: false);
		}
		if ((bool)zapIcon)
		{
			zapIcon.sprite = null;
		}
	}

	protected virtual void Update()
	{
		if (holdTimerLeft > 0f)
		{
			holdTimerLeft -= Time.unscaledDeltaTime;
			if (holdTimerLeft <= 0f)
			{
				if (!manager.CanChangeEquips(ItemData.Type, InventoryItemToolManager.CanChangeEquipsTypes.Reload))
				{
					return;
				}
				if (CanReload())
				{
					SetReloading(value: true);
				}
				else
				{
					ReportFailure();
				}
			}
		}
		if (!isReloading)
		{
			return;
		}
		if (reloadTimeElapsed < 0.025f)
		{
			reloadTimeElapsed += Time.unscaledDeltaTime;
			return;
		}
		if (ReloadSingle())
		{
			manager.RefreshTools();
		}
		reloadTimeElapsed %= 0.025f;
	}

	protected void ItemDataUpdated()
	{
		ToolItem itemData = ItemData;
		if ((bool)extraDesc)
		{
			extraDesc.ExtraDescPrefab = (itemData ? itemData.ExtraDescriptionSection : null);
		}
		if ((bool)zapIcon)
		{
			zapIcon.sprite = null;
		}
		wasPoison = false;
		wasZap = false;
	}

	protected abstract bool IsToolEquipped(ToolItem toolItem);

	public Color UpdateGetIconColour(SpriteRenderer applyToSprite, Color sourceColor, bool spawnEffects)
	{
		ToolItem itemData = ItemData;
		if (!itemData)
		{
			wasPoison = false;
			wasZap = false;
			return sourceColor;
		}
		bool flag;
		bool flag2;
		if ((bool)itemData && IsToolEquipped(itemData))
		{
			flag = itemData.PoisonDamageTicks > 0 && IsToolEquipped(Gameplay.PoisonPouchTool);
			flag2 = itemData.ZapDamageTicks > 0 && IsToolEquipped(Gameplay.ZapImbuementTool);
		}
		else
		{
			flag = false;
			flag2 = false;
		}
		if (spawnEffects)
		{
			bool flag3 = Time.frameCount != lastEffectFrame;
			lastEffectFrame = Time.frameCount;
			if (flag && !wasPoison && (bool)poisonEquipEffect)
			{
				if (flag3)
				{
					poisonEquipEffect.SetActive(value: false);
				}
				poisonEquipEffect.SetActive(value: true);
			}
			if (flag2 && !wasZap && (bool)zapEquipEffect)
			{
				if (flag3)
				{
					zapEquipEffect.SetActive(value: false);
				}
				zapEquipEffect.SetActive(value: true);
			}
		}
		wasPoison = flag;
		wasZap = flag2;
		bool num = itemData.GetInventorySprite((itemData.PoisonDamageTicks > 0 && IsToolEquipped(Gameplay.PoisonPouchTool)) ? ToolItem.IconVariants.Poison : ToolItem.IconVariants.Default) == itemData.InventorySpriteBase;
		Color result;
		if (num && flag && itemData.UsePoisonTintRecolour)
		{
			result = sourceColor * Gameplay.PoisonPouchTintColour;
			if (!applyToSprite.sharedMaterial.IsKeywordEnabled("RECOLOUR"))
			{
				applyToSprite.material.EnableKeyword("RECOLOUR");
			}
		}
		else
		{
			result = sourceColor;
			if (applyToSprite.sharedMaterial.IsKeywordEnabled("RECOLOUR"))
			{
				applyToSprite.material.DisableKeyword("RECOLOUR");
			}
		}
		if (num && flag)
		{
			if (!applyToSprite.sharedMaterial.IsKeywordEnabled("CAN_HUESHIFT"))
			{
				applyToSprite.material.EnableKeyword("CAN_HUESHIFT");
			}
			applyToSprite.material.SetFloat(_hueShiftPropId, itemData.PoisonHueShift);
		}
		else if (applyToSprite.sharedMaterial.IsKeywordEnabled("CAN_HUESHIFT"))
		{
			applyToSprite.material.DisableKeyword("CAN_HUESHIFT");
		}
		if (flag2 && itemData is ToolItemSkill toolItemSkill)
		{
			if ((bool)zapIcon)
			{
				zapIcon.sprite = toolItemSkill.InvGlowSprite;
			}
		}
		else if ((bool)zapIcon)
		{
			zapIcon.sprite = null;
		}
		return result;
	}

	public override bool Submit()
	{
		return DoPress();
	}

	protected abstract bool DoPress();

	public override bool Extra()
	{
		ToolItem itemData = ItemData;
		if ((bool)itemData && itemData.ReplenishUsage == ToolItem.ReplenishUsages.OneForOne)
		{
			holdTimerLeft = 0.3f;
			return true;
		}
		if (!itemData || !itemData.DisplayTogglePrompt)
		{
			return base.Extra();
		}
		return DoExtraPress();
	}

	public override bool ExtraReleased()
	{
		if (holdTimerLeft > 0f)
		{
			if (!DoExtraPress())
			{
				Debug.LogError("Release press could not be performed", this);
			}
		}
		else if (isReloading)
		{
			SetReloading(value: false);
		}
		holdTimerLeft = 0f;
		return true;
	}

	private bool DoExtraPress()
	{
		ToolItem itemData = ItemData;
		if (!itemData.CanToggle || !manager.CanChangeEquips(itemData.Type, InventoryItemToolManager.CanChangeEquipsTypes.Transform) || !itemData.DoToggle(out var didChangeVisually))
		{
			return base.Extra();
		}
		UpdateDisplay();
		manager.RefreshTools();
		if (!didChangeVisually)
		{
			return true;
		}
		if ((bool)reloadFailAnimator)
		{
			reloadFailAnimator.SetTrigger(_changeProp);
		}
		if ((bool)changeEffectPrefab)
		{
			changeEffectPrefab.Spawn().transform.SetPosition2D(base.transform.position);
		}
		itemData.PlayToggleAudio(reloadAudioSource);
		return true;
	}

	private void SetReloading(bool value)
	{
		if (isReloading == value)
		{
			return;
		}
		isReloading = value;
		if (isReloading)
		{
			if ((bool)reloadShakeParent)
			{
				reloadShakeParent.StartJitter();
			}
			ItemData.StartedReloading(reloadAudioSource);
		}
		else
		{
			if ((bool)reloadShakeParent)
			{
				reloadShakeParent.StopJitter();
			}
			ItemData.StoppedReloading(reloadAudioSource, !CanReload());
		}
	}

	private bool CanReload()
	{
		ToolItem itemData = ItemData;
		if ((bool)itemData)
		{
			return itemData.CanReload();
		}
		return false;
	}

	private bool ReloadSingle()
	{
		ToolItem itemData = ItemData;
		if (!itemData)
		{
			return false;
		}
		itemData.ReloadSingle();
		if (!CanReload())
		{
			SetReloading(value: false);
		}
		return true;
	}

	public void ReportFailure()
	{
		if ((bool)reloadFailAnimator)
		{
			reloadFailAnimator.SetTrigger(_failedProp);
		}
		failedAudioTable.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
	}
}
