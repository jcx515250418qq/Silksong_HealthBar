using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DeliveryHudIcon : RadialHudIcon
{
	[Space]
	[SerializeField]
	private GameObject hitEffectPrefab;

	[SerializeField]
	private GameObject burstAppearChild;

	[Space]
	public UnityEvent OnHit;

	public UnityEvent OnBreak;

	private DeliveryQuestItem previousItem;

	private DeliveryQuestItem currentItem;

	private int maxItemCount;

	private FullQuestBase currentQuest;

	private bool isHudOut;

	private bool queuedAppear;

	private GameManager gm;

	private bool started;

	private bool hasLoopEffect;

	private GameObject loopEffectObject;

	private bool hasCustomHitEffect;

	private GameObject customHitEffectPrefab;

	private void Awake()
	{
		gm = GameManager.instance;
		gm.SceneInit += base.UpdateDisplay;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "DELIVERY HUD REFRESH").ReceivedEvent += base.UpdateDisplay;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "DELIVERY HUD HIT").ReceivedEvent += SpawnHitEffect;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "DELIVERY HUD BREAK").ReceivedEvent += SpawnBreakEffect;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "TOOLS APPEAR").ReceivedEvent += StartUp;
	}

	private void OnEnable()
	{
		if ((bool)burstAppearChild)
		{
			burstAppearChild.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		if ((bool)gm)
		{
			gm.SceneInit -= base.UpdateDisplay;
		}
	}

	private void SpawnHitEffect()
	{
		OnHit.Invoke();
		Vector3 position = base.transform.position;
		if ((bool)hitEffectPrefab)
		{
			hitEffectPrefab.Spawn(position);
		}
		if (hasCustomHitEffect)
		{
			customHitEffectPrefab.Spawn(position);
		}
	}

	private void SpawnBreakEffect()
	{
		OnBreak.Invoke();
		StopLoopEffect();
		CleanLoopEffect();
		if ((bool)currentItem)
		{
			GameObject breakUIEffect = currentItem.BreakUIEffect;
			if ((bool)breakUIEffect)
			{
				breakUIEffect.Spawn(base.transform.position);
			}
		}
	}

	private void StartUp()
	{
		started = true;
		isHudOut = false;
		UpdateDisplay();
		if (base.gameObject.activeSelf && (bool)burstAppearChild)
		{
			burstAppearChild.SetActive(value: true);
		}
	}

	public void HudOut()
	{
		isHudOut = true;
		StopLoopEffect();
	}

	public void HudIn()
	{
		isHudOut = false;
		if (queuedAppear && started)
		{
			UpdateDisplay();
			StartLoopEffect();
			if ((bool)burstAppearChild)
			{
				burstAppearChild.SetActive(value: true);
			}
		}
	}

	protected override void OnPreUpdateDisplay()
	{
		previousItem = currentItem;
		currentItem = null;
		currentQuest = null;
		using (IEnumerator<DeliveryQuestItem.ActiveItem> enumerator = DeliveryQuestItem.GetActiveItems().GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				DeliveryQuestItem.ActiveItem current = enumerator.Current;
				currentItem = current.Item;
				currentQuest = current.Quest;
				maxItemCount = current.MaxCount;
			}
		}
		bool flag = currentItem != null;
		if (currentItem != previousItem)
		{
			hasCustomHitEffect = false;
			customHitEffectPrefab = null;
			if (previousItem != null)
			{
				CleanLoopEffect();
			}
			if (flag)
			{
				customHitEffectPrefab = currentItem.HitUIEffect;
				hasCustomHitEffect = customHitEffectPrefab != null;
				SpawnLoopEffect();
			}
		}
		if (isHudOut && flag)
		{
			queuedAppear = true;
			return;
		}
		queuedAppear = false;
		if (flag)
		{
			StartLoopEffect();
		}
	}

	protected override bool GetIsActive()
	{
		if (!started)
		{
			return false;
		}
		if (queuedAppear)
		{
			return false;
		}
		if ((bool)currentItem)
		{
			return DeliveryQuestItem.CanTakeHit();
		}
		return false;
	}

	protected override void GetAmounts(out int amountLeft, out int totalCount)
	{
		amountLeft = (currentQuest ? currentQuest.Counters.FirstOrDefault() : currentItem.CollectedAmount);
		totalCount = maxItemCount;
	}

	protected override bool TryGetHudSprite(out Sprite sprite)
	{
		sprite = currentItem.GetIcon(CollectableItem.ReadSource.Tiny);
		if ((bool)sprite)
		{
			return true;
		}
		sprite = currentItem.GetIcon(CollectableItem.ReadSource.Inventory);
		return false;
	}

	public override bool GetIsEmpty()
	{
		return false;
	}

	protected override bool HasTargetChanged()
	{
		return currentItem != previousItem;
	}

	protected override bool TryGetBarColour(out Color color)
	{
		if (!currentItem)
		{
			color = Color.white;
			return false;
		}
		color = currentItem.BarColour;
		return true;
	}

	protected override float GetMidProgress()
	{
		foreach (HeroController.DeliveryTimer deliveryTimer in HeroController.instance.GetDeliveryTimers())
		{
			if (!(deliveryTimer.Item.Item != currentItem))
			{
				float timeLeft = deliveryTimer.TimeLeft;
				float chunkDuration = deliveryTimer.Item.Item.GetChunkDuration(deliveryTimer.Item.MaxCount);
				return (chunkDuration - timeLeft) / chunkDuration;
			}
		}
		return 0f;
	}

	private void SpawnLoopEffect()
	{
		if (hasLoopEffect)
		{
			if (loopEffectObject != null)
			{
				Object.Destroy(loopEffectObject);
			}
			hasLoopEffect = false;
		}
		if (currentItem != previousItem && currentItem != null && currentItem.UILoopEffect != null)
		{
			loopEffectObject = currentItem.UILoopEffect.Spawn(base.transform, Vector3.zero);
			hasLoopEffect = loopEffectObject != null;
			if (hasLoopEffect)
			{
				loopEffectObject.SetActive(value: false);
			}
		}
	}

	private void StartLoopEffect()
	{
		if (hasLoopEffect)
		{
			loopEffectObject.gameObject.SetActive(value: true);
		}
	}

	private void StopLoopEffect()
	{
		if (hasLoopEffect)
		{
			loopEffectObject.gameObject.SetActive(value: false);
		}
	}

	private void CleanLoopEffect()
	{
		if (hasLoopEffect)
		{
			Object.Destroy(loopEffectObject);
			hasLoopEffect = false;
		}
	}
}
