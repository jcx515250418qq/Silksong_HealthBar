using System.Text;
using TeamCherry.Localization;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (Memento)")]
public class CollectableItemMemento : CollectableItemBasic, ICollectionViewerItem
{
	[Header("Memento")]
	[SerializeField]
	private FullQuestBase requireCompletedQuest;

	[SerializeField]
	private Object countKey;

	[SerializeField]
	private LocalisedString extraDesc;

	public bool CanDeposit
	{
		get
		{
			if ((bool)requireCompletedQuest)
			{
				return requireCompletedQuest.IsCompleted;
			}
			return true;
		}
	}

	public Object CountKey
	{
		get
		{
			if (!countKey)
			{
				return this;
			}
			return countKey;
		}
	}

	public bool IsSeenOverridden => true;

	public bool IsSeenOverrideValue
	{
		get
		{
			return PlayerData.instance.MementosDeposited.GetData(base.name).HasSeenInRelicBoard;
		}
		set
		{
			CollectableMementosData mementosDeposited = PlayerData.instance.MementosDeposited;
			CollectableMementosData.Data data = mementosDeposited.GetData(base.name);
			data.HasSeenInRelicBoard = value;
			mementosDeposited.SetData(base.name, data);
		}
	}

	string ICollectionViewerItem.name => base.name;

	public bool IsListedInCollection()
	{
		if (CollectedAmount <= 0)
		{
			return IsVisibleInCollection();
		}
		return true;
	}

	public override bool IsVisibleInCollection()
	{
		return PlayerData.instance.MementosDeposited.GetData(base.name).IsDeposited;
	}

	public override string GetDescription(ReadSource readSource)
	{
		string text = base.GetDescription(readSource);
		if (readSource == ReadSource.Shop)
		{
			return text;
		}
		if ((bool)requireCompletedQuest && !requireCompletedQuest.IsCompleted)
		{
			return text;
		}
		StringBuilder tempStringBuilder = Helper.GetTempStringBuilder(text);
		tempStringBuilder.AppendLine();
		tempStringBuilder.AppendLine();
		tempStringBuilder.Append(extraDesc);
		return tempStringBuilder.ToString();
	}
}
