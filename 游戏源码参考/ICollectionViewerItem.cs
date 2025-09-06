using UnityEngine;

public interface ICollectionViewerItem
{
	string name { get; }

	bool IsSeen { get; set; }

	bool IsSeenOverridden => false;

	bool IsSeenOverrideValue
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	bool CanDeposit => true;

	string GetCollectionName();

	string GetCollectionDesc();

	Sprite GetCollectionIcon();

	bool IsListedInCollection()
	{
		return IsVisibleInCollection();
	}

	bool IsVisibleInCollection();

	bool IsRequiredInCollection();
}
