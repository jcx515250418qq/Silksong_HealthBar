using System;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

public class CorpseItems : MonoBehaviour, BlackThreadState.IBlackThreadStateReceiver
{
	[Serializable]
	private class ItemPickupProbability : Probability.ProbabilityBase<SavedItem>
	{
		[SerializeField]
		private SavedItem item;

		public MinMaxInt Amount = new MinMaxInt(1, 1);

		public string PlayAnimOnPickup;

		public override SavedItem Item => item;
	}

	[Serializable]
	private class ItemPickupGroup
	{
		[Range(0f, 1f)]
		public float TotalProbability = 1f;

		public List<ItemPickupProbability> Drops;
	}

	private struct ItemPickupSingle
	{
		public SavedItem Item;

		public string PlayAnimOnPickup;
	}

	[SerializeField]
	private List<ItemPickupGroup> itemPickupGroups;

	[SerializeField]
	private Vector3 genericPickupOffset;

	[SerializeField]
	private GameObject extraPickupEffectPrefab;

	private List<ItemPickupSingle> pickupItems;

	private GenericPickup pickupObject;

	private GameObject pickupEffect;

	protected tk2dSpriteAnimator Animator;

	protected bool HasAnimator;

	private bool isBlackThreaded;

	protected bool IsBlackThreaded => isBlackThreaded;

	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(genericPickupOffset, 0.1f);
	}

	protected virtual void Awake()
	{
		Animator = GetComponent<tk2dSpriteAnimator>();
		HasAnimator = Animator;
	}

	protected virtual void Start()
	{
		pickupItems = GetPickupItems();
		List<ItemPickupSingle> list = pickupItems;
		if (list != null && list.Count > 0)
		{
			pickupObject = UnityEngine.Object.Instantiate(Gameplay.GenericPickupPrefab, base.transform);
			pickupObject.transform.localPosition = genericPickupOffset;
			pickupObject.SetActive(value: false, isInstant: true);
			pickupObject.PickupAction += DoPickupItems;
			if ((bool)extraPickupEffectPrefab)
			{
				pickupEffect = UnityEngine.Object.Instantiate(extraPickupEffectPrefab, base.transform);
				pickupEffect.transform.localPosition = genericPickupOffset;
				pickupEffect.SetActive(value: false);
			}
		}
	}

	public void DeactivatePickup()
	{
		if ((bool)pickupObject)
		{
			pickupObject.SetActive(value: false);
		}
	}

	public void ActivatePickup()
	{
		if ((bool)pickupObject)
		{
			pickupObject.SetActive(value: true);
		}
	}

	public void ClearPickupItems()
	{
		itemPickupGroups.Clear();
	}

	private List<ItemPickupSingle> GetPickupItems()
	{
		List<ItemPickupSingle> list = null;
		foreach (ItemPickupGroup itemPickupGroup in itemPickupGroups)
		{
			if (UnityEngine.Random.Range(0f, 1f) > itemPickupGroup.TotalProbability || itemPickupGroup.Drops.Count <= 0)
			{
				continue;
			}
			ItemPickupProbability itemPickupProbability = (ItemPickupProbability)Probability.GetRandomItemRootByProbability<ItemPickupProbability, SavedItem>(itemPickupGroup.Drops.ToArray());
			if (itemPickupProbability == null)
			{
				continue;
			}
			SavedItem item = itemPickupProbability.Item;
			if ((bool)item && item.CanGetMore())
			{
				int randomValue = itemPickupProbability.Amount.GetRandomValue();
				if (list == null)
				{
					list = new List<ItemPickupSingle>(randomValue);
				}
				for (int i = 0; i < randomValue; i++)
				{
					list.Add(new ItemPickupSingle
					{
						Item = item,
						PlayAnimOnPickup = itemPickupProbability.PlayAnimOnPickup
					});
				}
			}
		}
		return list;
	}

	private bool DoPickupItems()
	{
		int count = pickupItems.Count;
		for (int num = count - 1; num >= 0; num--)
		{
			ItemPickupSingle itemPickupSingle = pickupItems[num];
			if (itemPickupSingle.Item.TryGet(breakIfAtMax: false))
			{
				pickupItems.RemoveAt(num);
				if (!string.IsNullOrEmpty(itemPickupSingle.PlayAnimOnPickup) && HasAnimator)
				{
					Animator.Play(itemPickupSingle.PlayAnimOnPickup);
				}
			}
		}
		if (pickupItems.Count < count && (bool)pickupEffect)
		{
			pickupEffect.SetActive(value: false);
			pickupEffect.SetActive(value: true);
		}
		if (pickupItems.Count != 0)
		{
			return false;
		}
		pickupObject.transform.SetParent(null, worldPositionStays: true);
		pickupObject = null;
		base.transform.Translate(0f, 0f, 0.005f, Space.Self);
		return true;
	}

	public float GetBlackThreadAmount()
	{
		return isBlackThreaded ? 1 : 0;
	}

	public void SetIsBlackThreaded(bool isThreaded)
	{
		if (isThreaded)
		{
			isBlackThreaded = true;
			BlackThreadEffectRendererGroup component = GetComponent<BlackThreadEffectRendererGroup>();
			if (component != null)
			{
				component.SetBlackThreadAmount(1f);
			}
		}
		else
		{
			isBlackThreaded = false;
			BlackThreadEffectRendererGroup component2 = GetComponent<BlackThreadEffectRendererGroup>();
			if (component2 != null)
			{
				component2.OnRecycled();
			}
		}
	}
}
