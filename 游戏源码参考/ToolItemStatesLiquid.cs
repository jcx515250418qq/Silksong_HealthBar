using TeamCherry.SharedUtils;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "Hornet/Tool Item (States - Liquid)")]
public class ToolItemStatesLiquid : ToolItemStates, IToolExtraNew
{
	[Header("Liquid")]
	[SerializeField]
	private Color liquidColor;

	[SerializeField]
	private int refillsMax;

	[Space]
	[SerializeField]
	private int bottleCost;

	[SerializeField]
	private bool delayBottleBreak;

	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string infiniteRefillsBool;

	[Space]
	[SerializeField]
	private GameObject refillEffectHero;

	private static ToolItemStatesLiquid _waitingForBottleBreak;

	public int RefillsMax => refillsMax;

	public bool HasInfiniteRefills
	{
		get
		{
			if (!string.IsNullOrEmpty(infiniteRefillsBool))
			{
				return PlayerData.instance.GetVariable<bool>(infiniteRefillsBool);
			}
			return false;
		}
	}

	public Color LiquidColor => liquidColor;

	public ToolItemLiquidsData.Data LiquidSavedData
	{
		get
		{
			return PlayerData.instance.ToolLiquids.GetData(base.name);
		}
		private set
		{
			PlayerData.instance.ToolLiquids.SetData(base.name, value);
		}
	}

	public override bool UsableWhenEmpty
	{
		get
		{
			if (base.UsableWhenEmpty)
			{
				return !LiquidSavedData.UsedExtra;
			}
			return false;
		}
	}

	public override bool UsableWhenEmptyPrevented => _waitingForBottleBreak == this;

	public override bool HideUsePrompt => IsEmptyNoRefills();

	public bool IsFull
	{
		get
		{
			int amountLeft = base.SavedData.AmountLeft;
			int toolStorageAmount = ToolItemManager.GetToolStorageAmount(this);
			if (amountLeft >= toolStorageAmount)
			{
				return IsRefillsFull;
			}
			return false;
		}
	}

	public bool IsRefillsFull => LiquidSavedData.RefillsLeft >= refillsMax;

	public bool HasExtraNew
	{
		get
		{
			ToolItemLiquidsData.Data liquidSavedData = LiquidSavedData;
			if (base.IsEmpty && liquidSavedData.RefillsLeft <= 0)
			{
				return !liquidSavedData.SeenEmptyState;
			}
			return false;
		}
	}

	public override void SetupExtraDescription(GameObject obj)
	{
		LiquidMeter component = obj.GetComponent<LiquidMeter>();
		if ((bool)component)
		{
			component.SetDisplay(this);
		}
	}

	private bool IsEmptyNoRefills()
	{
		ToolItemLiquidsData.Data liquidSavedData = LiquidSavedData;
		if (!base.IsEmpty || HasInfiniteRefills)
		{
			return false;
		}
		return liquidSavedData.RefillsLeft <= 0;
	}

	public override bool TryReplenishSingle(bool doReplenish, float inCost, out float outCost, out int reserveCost)
	{
		outCost = inCost;
		reserveCost = 1;
		if (HasInfiniteRefills)
		{
			reserveCost = 0;
			return true;
		}
		ToolItemLiquidsData.Data liquidSavedData = LiquidSavedData;
		bool flag = liquidSavedData.RefillsLeft <= 0;
		if (!liquidSavedData.UsedExtra)
		{
			return !flag;
		}
		outCost += bottleCost;
		if (flag)
		{
			reserveCost = 0;
		}
		if (doReplenish)
		{
			liquidSavedData.UsedExtra = false;
			LiquidSavedData = liquidSavedData;
			if (_waitingForBottleBreak == this)
			{
				_waitingForBottleBreak = null;
			}
			AttackToolBinding? attackToolBinding = ToolItemManager.GetAttackToolBinding(this);
			if (attackToolBinding.HasValue)
			{
				ToolItemManager.ReportBoundAttackToolUpdated(attackToolBinding.Value);
			}
		}
		return true;
	}

	public void TakeLiquid(int amount, bool showCounter)
	{
		if (showCounter)
		{
			LiquidReserveCounter.Take(this, amount);
		}
		ToolItemLiquidsData.Data liquidSavedData = LiquidSavedData;
		liquidSavedData.RefillsLeft -= amount;
		LiquidSavedData = liquidSavedData;
	}

	public void ShowLiquidInfiniteRefills()
	{
		LiquidReserveCounter.InfiniteRefillPopup(this);
	}

	public override void OnWasUsed(bool wasEmpty)
	{
		if (wasEmpty && !LiquidSavedData.UsedExtra)
		{
			if (delayBottleBreak)
			{
				_waitingForBottleBreak = this;
			}
			else
			{
				SetBottleBroken();
			}
		}
	}

	private void SetBottleBroken()
	{
		ToolItemLiquidsData.Data liquidSavedData = LiquidSavedData;
		liquidSavedData.UsedExtra = true;
		LiquidSavedData = liquidSavedData;
		AttackToolBinding? attackToolBinding = ToolItemManager.GetAttackToolBinding(this);
		if (attackToolBinding.HasValue)
		{
			ToolItemManager.ReportBoundAttackToolUpdated(attackToolBinding.Value);
		}
	}

	public static void ReportBottleBroken(ToolItemStatesLiquid tool)
	{
		if (!(_waitingForBottleBreak == null) && !(tool != _waitingForBottleBreak))
		{
			_waitingForBottleBreak = null;
			tool.SetBottleBroken();
		}
	}

	protected override void OnUnlocked()
	{
		RefillRefills(showPopup: false);
	}

	public void SetExtraSeen()
	{
		ToolItemLiquidsData.Data liquidSavedData = LiquidSavedData;
		liquidSavedData.SeenEmptyState = true;
		LiquidSavedData = liquidSavedData;
	}

	public void RefillRefills(bool showPopup)
	{
		ToolItemLiquidsData.Data liquidSavedData = LiquidSavedData;
		liquidSavedData.UsedExtra = false;
		liquidSavedData.RefillsLeft = refillsMax;
		LiquidSavedData = liquidSavedData;
		if (showPopup)
		{
			ShowRefillMsgFull();
			PlayHeroRefillEffect();
		}
	}

	public void PlayHeroRefillEffect()
	{
		if ((bool)refillEffectHero)
		{
			HeroController instance = HeroController.instance;
			if ((bool)instance)
			{
				Vector3 position = instance.transform.position;
				refillEffectHero.Spawn(position);
			}
		}
	}
}
