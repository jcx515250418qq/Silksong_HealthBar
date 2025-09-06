using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class QuestRewardHolder : MonoBehaviour
{
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private FullQuestBase quest;

	[SerializeField]
	[PlayerDataField(typeof(bool), true)]
	private string pickupPdBool;

	[Space]
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private GenericPickup pickup;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Animator animator;

	[SerializeField]
	private GameObject[] disableOnPickup;

	[Space]
	public UnityEvent OnPickedUp;

	private static readonly int _emptyAnim = Animator.StringToHash("Empty");

	private static readonly int _openAnim = Animator.StringToHash("Open");

	private static readonly int _closedAnim = Animator.StringToHash("Closed");

	private void OnEnable()
	{
		UpdateState(isInstant: true);
	}

	private void UpdateState(bool isInstant)
	{
		pickup.PickupAction -= OnItemPickup;
		if (PlayerData.instance.GetVariable<bool>(pickupPdBool))
		{
			animator.Play(_emptyAnim);
			pickup.SetActive(value: false, isInstant);
			disableOnPickup.SetAllActive(value: false);
			return;
		}
		if (quest.IsCompleted)
		{
			animator.Play(_openAnim);
			pickup.SetActive(value: true, isInstant);
		}
		else
		{
			animator.Play(_closedAnim);
			pickup.SetActive(value: false, isInstant);
		}
		pickup.PickupAction += OnItemPickup;
	}

	public void Refresh()
	{
		UpdateState(isInstant: false);
	}

	private bool OnItemPickup()
	{
		SavedItem rewardItem = quest.RewardItem;
		if ((bool)rewardItem && !rewardItem.TryGet(breakIfAtMax: false))
		{
			return false;
		}
		animator.Play(_emptyAnim);
		disableOnPickup.SetAllActive(value: false);
		PlayerData.instance.SetVariable(pickupPdBool, value: true);
		OnPickedUp.Invoke();
		return true;
	}
}
