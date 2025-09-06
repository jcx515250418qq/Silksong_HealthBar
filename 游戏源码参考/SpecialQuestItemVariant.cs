using UnityEngine;

public class SpecialQuestItemVariant : MonoBehaviour
{
	[SerializeField]
	private FullQuestBase activeQuest;

	[SerializeField]
	[Range(0f, 1f)]
	private float probability = 1f;

	[Space]
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private CollectableItemPickup itemPickupPrefab;

	[SerializeField]
	private SpriteFlash spriteFlash;

	[SerializeField]
	private BreakablePoleTopLand poleTopLand;

	private bool isInactive;

	private void OnDisable()
	{
		isInactive = false;
	}

	private void Start()
	{
		if (isInactive || Random.Range(0f, 1f) > probability || !activeQuest.IsAccepted || activeQuest.IsCompleted || activeQuest.CanComplete)
		{
			return;
		}
		if ((bool)spriteFlash)
		{
			spriteFlash.flashWhiteLong();
			spriteFlash.FlashingWhiteLong();
		}
		if (!itemPickupPrefab)
		{
			return;
		}
		CollectableItemPickup itemPick = Object.Instantiate(itemPickupPrefab, base.transform);
		Transform itemPickTrans = itemPick.transform;
		itemPickTrans.localPosition = new Vector3(0f, 0f, -0.0001f);
		itemPickTrans.SetRotation2D(0f);
		itemPick.SetItem(activeQuest);
		itemPick.OnPickup.AddListener(delegate
		{
			itemPickTrans.SetParent(null, worldPositionStays: true);
			if ((bool)spriteFlash)
			{
				spriteFlash.gameObject.SetActive(value: false);
			}
		});
		if ((bool)poleTopLand)
		{
			poleTopLand.OnStick.AddListener(delegate
			{
				itemPick.PickupAnim = CollectableItemPickup.PickupAnimations.Stand;
			});
		}
	}

	public void SetInactive()
	{
		isInactive = true;
	}
}
