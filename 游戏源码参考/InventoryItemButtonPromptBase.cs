using TeamCherry.SharedUtils;
using UnityEngine;

public abstract class InventoryItemButtonPromptBase<TData> : MonoBehaviour, InventoryItemUpdateable.IIsSeenProvider
{
	[SerializeField]
	protected InventoryItemButtonPromptDisplayList display;

	[SerializeField]
	private TData data;

	[SerializeField]
	private PlayerDataTest appearCondition;

	[SerializeField]
	private InventoryItemExtraDescription extraDescriptionOverride;

	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string hasSeenPdBool;

	[SerializeField]
	protected int order;

	public bool IsSeen
	{
		get
		{
			if (string.IsNullOrEmpty(hasSeenPdBool))
			{
				return true;
			}
			return PlayerData.instance.GetVariable<bool>(hasSeenPdBool);
		}
		set
		{
			if (!string.IsNullOrEmpty(hasSeenPdBool))
			{
				PlayerData.instance.SetVariable(hasSeenPdBool, value);
			}
		}
	}

	private void OnEnable()
	{
		if (!appearCondition.IsFulfilled || ((bool)extraDescriptionOverride && extraDescriptionOverride.WillDisplay))
		{
			return;
		}
		InventoryItemSelectable component = GetComponent<InventoryItemSelectable>();
		if ((bool)component)
		{
			component.OnSelected += Show;
			component.OnDeselected += Hide;
			InventoryItemUpdateable component2 = GetComponent<InventoryItemUpdateable>();
			if ((bool)component2)
			{
				component2.RegisterIsSeenProvider(this);
			}
		}
	}

	private void OnDisable()
	{
		InventoryItemUpdateable component = GetComponent<InventoryItemUpdateable>();
		if ((bool)component)
		{
			component.DeregisterIsSeenProvider(this);
		}
		InventoryItemSelectable component2 = GetComponent<InventoryItemSelectable>();
		if ((bool)component2)
		{
			component2.OnSelected -= Show;
			component2.OnDeselected -= Hide;
			Hide(component2);
		}
	}

	private void Hide(InventoryItemSelectable selectable)
	{
		if ((bool)display)
		{
			display.Clear();
		}
	}

	private void Show(InventoryItemSelectable selectable)
	{
		if (base.enabled && (bool)display)
		{
			OnShow(display, data);
		}
	}

	public void SetData(TData newData)
	{
		data = newData;
	}

	protected abstract void OnShow(InventoryItemButtonPromptDisplayList displayList, TData data);
}
