using UnityEngine;

public class InventoryItemMaterium : InventoryItemUpdateable
{
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private Sprite emptySprite;

	private MateriumItem itemData;

	public MateriumItem ItemData
	{
		get
		{
			return itemData;
		}
		set
		{
			itemData = value;
			Refresh();
		}
	}

	public override string DisplayName => ItemData.DisplayName;

	public override string Description => ItemData.Description;

	public Sprite Sprite => ItemData.Icon;

	protected override bool IsSeen
	{
		get
		{
			return true;
		}
		set
		{
		}
	}

	protected override void Awake()
	{
		base.Awake();
		spriteRenderer = GetComponent<SpriteRenderer>();
		InventoryPaneBase componentInParent = GetComponentInParent<InventoryPaneBase>();
		if ((bool)componentInParent)
		{
			componentInParent.OnPaneStart += Refresh;
		}
		Refresh();
	}

	private void Refresh()
	{
		bool flag = (bool)itemData && itemData.IsCollected;
		if ((bool)spriteRenderer)
		{
			spriteRenderer.sprite = (flag ? Sprite : emptySprite);
		}
	}
}
