using UnityEngine;

public class InventoryParentPane : InventoryPane
{
	[Space]
	[SerializeField]
	private InventoryPane[] subPanes;

	public override bool IsAvailable
	{
		get
		{
			if (!base.IsAvailable)
			{
				return false;
			}
			InventoryPane[] array = subPanes;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsAvailable)
				{
					return true;
				}
			}
			return false;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		InventoryPane[] array = subPanes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].RootPane = this;
		}
	}

	public override InventoryPane Get()
	{
		int num = 0;
		InventoryPane result = null;
		InventoryPane[] array = subPanes;
		foreach (InventoryPane inventoryPane in array)
		{
			if (inventoryPane.IsAvailable)
			{
				num++;
				result = inventoryPane;
			}
		}
		if (num == 0)
		{
			return base.Get();
		}
		if (num <= 1)
		{
			return result;
		}
		return base.Get();
	}
}
