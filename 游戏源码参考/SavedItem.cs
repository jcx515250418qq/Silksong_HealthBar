using System;
using UnityEngine;

public abstract class SavedItem : ScriptableObject
{
	public virtual bool CanGetMultipleAtOnce => true;

	public virtual bool IsUnique => false;

	public void Get(int amount, bool showPopup = true)
	{
		if (CanGetMultipleAtOnce)
		{
			GetMultiple(amount, showPopup);
		}
		else
		{
			Get(showPopup);
		}
	}

	public abstract void Get(bool showPopup = true);

	public abstract bool CanGetMore();

	protected virtual void GetMultiple(int amount, bool showPopup)
	{
		for (int i = 0; i < amount; i++)
		{
			Get(showPopup);
		}
	}

	public bool TryGet(bool breakIfAtMax, bool showPopup = true)
	{
		CollectableItem collectableItem = this as CollectableItem;
		if (collectableItem != null && collectableItem.IsAtMax())
		{
			Get(showPopup);
			if (breakIfAtMax)
			{
				collectableItem.ConsumeItemResponse();
				collectableItem.InstantUseSounds.SpawnAndPlayOneShot(HeroController.instance.transform.position);
				return true;
			}
			return false;
		}
		Get(showPopup);
		return true;
	}

	public virtual Sprite GetPopupIcon()
	{
		if (Application.isPlaying)
		{
			Debug.LogException(new NotImplementedException());
		}
		return null;
	}

	public virtual string GetPopupName()
	{
		if (Application.isPlaying)
		{
			Debug.LogException(new NotImplementedException());
		}
		return null;
	}

	public virtual int GetSavedAmount()
	{
		if (Application.isPlaying)
		{
			Debug.LogException(new NotImplementedException());
		}
		return 0;
	}

	public virtual bool HasUpgradeIcon()
	{
		return false;
	}

	public virtual bool GetTakesHeroControl()
	{
		return false;
	}

	public virtual void SetupExtraDescription(GameObject obj)
	{
	}

	public virtual void SetHasNew(bool hasPopup)
	{
		Debug.LogException(new NotImplementedException());
	}
}
