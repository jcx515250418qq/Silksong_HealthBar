using System.Collections;
using System.Collections.Generic;
using GlobalSettings;
using TMProOld;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class SimpleShopMenu : MonoBehaviour
{
	private enum State
	{
		Transitioning = -2,
		Inactive = -1,
		ItemList = 0,
		Confirm = 1
	}

	[SerializeField]
	private Vector2 screenPos;

	[Space]
	[SerializeField]
	private TMP_Text titleText;

	[SerializeField]
	[ArrayForEnum(typeof(State))]
	private NestedFadeGroupBase[] stateFaders;

	[SerializeField]
	private float stateFadeDuration;

	[SerializeField]
	private TMP_Text purchaseText;

	[Header("Item List")]
	[SerializeField]
	private Transform itemList;

	[SerializeField]
	private SimpleShopItemDisplay templateItem;

	[SerializeField]
	private Vector2 baseItemSpacing;

	[SerializeField]
	private Vector2 selectedItemSpacing;

	[Space]
	[SerializeField]
	private AnimationCurve moveToCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float moveToDuration;

	[SerializeField]
	private AnimationCurve failCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[SerializeField]
	private Vector2 failOffset;

	[SerializeField]
	private float failDuration;

	[Space]
	[SerializeField]
	private BaseAnimator upArrow;

	[SerializeField]
	private BaseAnimator downArrow;

	[Space]
	[SerializeField]
	private AudioEvent moveSound;

	[SerializeField]
	private AudioEvent notEnoughSubmitSound;

	[SerializeField]
	private JitterSelfForTime selectorJitter;

	[SerializeField]
	private AudioEvent submitSound;

	[SerializeField]
	private AudioEvent cancelSound;

	[Header("Confirm")]
	[SerializeField]
	private UISelectionList confirmList;

	[SerializeField]
	private TMP_Text confirmNameText;

	[SerializeField]
	private TMP_Text confirmCostText;

	private SimpleShopMenuOwner owner;

	private List<ISimpleShopItem> shopItems;

	private readonly List<SimpleShopItemDisplay> spawnedItemDisplays = new List<SimpleShopItemDisplay>();

	private int activeItemCount;

	private State state;

	private Coroutine transitionStateRoutine;

	private int selectedIndex = -1;

	private int purchasedIndex = -1;

	private bool didPurchase;

	private Coroutine moveRoutine;

	private float openTime;

	private InventoryPaneStandalone pane;

	private InventoryPaneInput paneInput;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref stateFaders, typeof(State));
	}

	private void Awake()
	{
		OnValidate();
		pane = GetComponent<InventoryPaneStandalone>();
		pane.OnPaneStart += OnPaneStart;
		pane.PaneOpenedAnimEnd += OnPaneOpenedAnimEnd;
		pane.OnPaneEnd += OnPaneEnd;
		pane.OnInputUp += OnInputUp;
		pane.OnInputDown += OnInputDown;
		paneInput = GetComponent<InventoryPaneInput>();
		if ((bool)templateItem)
		{
			templateItem.gameObject.SetActive(value: false);
		}
		if ((bool)confirmList)
		{
			confirmList.gameObject.SetActive(value: false);
		}
		base.transform.SetPosition2D(screenPos);
	}

	private void Update()
	{
		if (state != 0)
		{
			return;
		}
		HeroActions inputActions = ManagerSingleton<InputHandler>.Instance.inputActions;
		switch (Platform.Current.GetMenuAction(inputActions))
		{
		case Platform.MenuActions.Submit:
			OnSubmitPressed();
			return;
		case Platform.MenuActions.Cancel:
			OnCancelPressed();
			return;
		}
		if (InventoryPaneInput.IsInventoryButtonPressed(inputActions))
		{
			OnCancelPressed();
		}
	}

	public void SetStock(SimpleShopMenuOwner newOwner, List<ISimpleShopItem> newShopItems)
	{
		owner = newOwner;
		if ((bool)titleText)
		{
			titleText.text = owner.ShopTitle;
		}
		if ((bool)purchaseText)
		{
			purchaseText.text = owner.PurchaseText;
		}
		if (!templateItem)
		{
			Debug.LogError("No templateItem assigned!", this);
			return;
		}
		if (!itemList)
		{
			Debug.LogError("No itemList assigned!", this);
			return;
		}
		shopItems = newShopItems;
		activeItemCount = shopItems.Count;
		foreach (SimpleShopItemDisplay spawnedItemDisplay in spawnedItemDisplays)
		{
			spawnedItemDisplay.gameObject.SetActive(value: false);
		}
		for (int num = activeItemCount - spawnedItemDisplays.Count; num > 0; num--)
		{
			SimpleShopItemDisplay item = Object.Instantiate(templateItem, itemList);
			spawnedItemDisplays.Add(item);
		}
		for (int i = 0; i < activeItemCount; i++)
		{
			SimpleShopItemDisplay simpleShopItemDisplay = spawnedItemDisplays[i];
			simpleShopItemDisplay.SetItem(newShopItems[i]);
			simpleShopItemDisplay.gameObject.SetActive(value: true);
		}
	}

	public void Activate()
	{
		pane.PaneStart();
	}

	private void OnPaneStart()
	{
		state = State.ItemList;
		openTime = Time.time + 1f;
		if ((bool)confirmList)
		{
			confirmList.SetActive(value: false);
		}
		stateFaders[1].AlphaSelf = 0f;
		stateFaders[0].AlphaSelf = 1f;
		selectedIndex = -1;
		purchasedIndex = -1;
		didPurchase = false;
		ScrollTo(0, isInstant: true);
		CurrencyCounter.Show(CurrencyType.Money);
	}

	private void OnPaneOpenedAnimEnd()
	{
		openTime = 0f;
	}

	private void OnPaneEnd()
	{
		owner.ClosedMenu(didPurchase, purchasedIndex);
		state = State.Inactive;
		owner = null;
		CurrencyCounter.Hide(CurrencyType.Money);
	}

	private void OnInputUp()
	{
		if (!(openTime > Time.time) && state == State.ItemList)
		{
			ScrollTo(selectedIndex - 1);
			if ((bool)upArrow)
			{
				upArrow.StartAnimation();
			}
		}
	}

	private void OnInputDown()
	{
		if (!(openTime > Time.time) && state == State.ItemList)
		{
			ScrollTo(selectedIndex + 1);
			if ((bool)downArrow)
			{
				downArrow.StartAnimation();
			}
		}
	}

	private void OnSubmitPressed()
	{
		if (openTime > Time.time || state != 0)
		{
			return;
		}
		ISimpleShopItem currentSelectedItem = GetCurrentSelectedItem();
		if (currentSelectedItem == null)
		{
			return;
		}
		if (PlayerData.instance.geo < currentSelectedItem.GetCost())
		{
			notEnoughSubmitSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
			if ((bool)selectorJitter)
			{
				selectorJitter.StartTimedJitter();
			}
			return;
		}
		submitSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
		if (currentSelectedItem.DelayPurchase())
		{
			state = State.Transitioning;
			didPurchase = true;
			purchasedIndex = selectedIndex;
			pane.PaneEnd();
		}
		else if ((bool)confirmList)
		{
			confirmList.gameObject.SetActive(value: true);
			confirmList.SetActive(value: false);
			TransitionState(State.Confirm, waitFrame: false);
		}
		else
		{
			ConfirmYes();
		}
	}

	private void OnCancelPressed()
	{
		if (!(openTime > Time.time) && state == State.ItemList)
		{
			state = State.Inactive;
			cancelSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
			pane.PaneEnd();
		}
	}

	public void ConfirmYes()
	{
		didPurchase = true;
		purchasedIndex = selectedIndex;
		ISimpleShopItem currentSelectedItem = GetCurrentSelectedItem();
		if (currentSelectedItem != null)
		{
			CurrencyManager.TakeCurrency(currentSelectedItem.GetCost(), CurrencyType.Money);
		}
		if (owner.ClosePaneOnPurchase)
		{
			pane.PaneEnd();
			return;
		}
		owner.PurchaseNoClose(purchasedIndex);
		purchasedIndex = -1;
		if (owner.HasStockLeft())
		{
			owner.RefreshStock();
			ScrollTo(0, isInstant: true);
			ConfirmNo(waitFrame: true);
		}
		else
		{
			pane.PaneEnd();
		}
	}

	public void ConfirmNo()
	{
		confirmList.SetActive(value: false);
		TransitionState(State.ItemList, waitFrame: false);
	}

	public void ConfirmNo(bool waitFrame)
	{
		confirmList.SetActive(value: false);
		TransitionState(State.ItemList, waitFrame);
	}

	private void ScrollTo(int index, bool isInstant = false)
	{
		int num = 0;
		if (index >= activeItemCount)
		{
			index = activeItemCount - 1;
			num = 1;
		}
		else if (index < 0)
		{
			index = 0;
			num = -1;
		}
		int previousIndex = selectedIndex;
		selectedIndex = index;
		Vector2 targetPosition = baseItemSpacing * (index * -1);
		if (moveRoutine != null)
		{
			StopCoroutine(moveRoutine);
		}
		if (num == 0)
		{
			if (isInstant)
			{
				itemList.localPosition = targetPosition;
				UpdateItemPositions(-1, index, 1f);
				return;
			}
			moveSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
			Vector2 initialPos = itemList.localPosition;
			moveRoutine = this.StartTimerRoutine(0f, moveToDuration, delegate(float time)
			{
				time = moveToCurve.Evaluate(time);
				Vector2 position = Vector2.Lerp(initialPos, targetPosition, time);
				itemList.SetLocalPosition2D(position);
				UpdateItemPositions(previousIndex, selectedIndex, time);
			});
			return;
		}
		if (!isInstant)
		{
			UpdateItemPositions(-1, index, 1f);
			itemList.SetLocalPosition2D(targetPosition);
			Vector2 moveTargetPosition = targetPosition + failOffset * num;
			moveRoutine = this.StartTimerRoutine(0f, failDuration, delegate(float time)
			{
				Vector2 position2 = Vector2.Lerp(targetPosition, moveTargetPosition, failCurve.Evaluate(time));
				itemList.SetLocalPosition2D(position2);
			});
		}
		paneInput.CancelRepeat();
	}

	private void UpdateItemPositions(int previousItemIndex, int currentItemIndex, float blend)
	{
		for (int i = 0; i < activeItemCount; i++)
		{
			float b = 0f;
			float a = 0f;
			if (currentItemIndex >= 0)
			{
				if (i > currentItemIndex)
				{
					b = 1f;
				}
				else if (i < currentItemIndex)
				{
					b = -1f;
				}
			}
			if (previousItemIndex >= 0)
			{
				if (i > previousItemIndex)
				{
					a = 1f;
				}
				else if (i < previousItemIndex)
				{
					a = -1f;
				}
			}
			float num = Mathf.Lerp(a, b, blend);
			spawnedItemDisplays[i].transform.SetLocalPosition2D(baseItemSpacing * i + num * selectedItemSpacing);
		}
	}

	private void TransitionState(State newState, bool waitFrame)
	{
		if (transitionStateRoutine != null)
		{
			Debug.LogError("Already transitioning");
		}
		else if (state == State.Inactive || newState == State.Inactive)
		{
			Debug.LogError("Can't transition from or to inactive");
		}
		else
		{
			transitionStateRoutine = StartCoroutine(DoTransitionState(state, newState, waitFrame));
		}
	}

	private IEnumerator DoTransitionState(State previousState, State newState, bool waitFrame)
	{
		if (waitFrame)
		{
			yield return null;
		}
		Debug.LogFormat(this, "Transitioning from {0} to {1}", previousState.ToString(), newState.ToString());
		state = State.Transitioning;
		switch (newState)
		{
		case State.ItemList:
		{
			for (int i = 0; i < activeItemCount; i++)
			{
				spawnedItemDisplays[i].SetItem(shopItems[i]);
			}
			break;
		}
		case State.Confirm:
		{
			ISimpleShopItem currentSelectedItem = GetCurrentSelectedItem();
			if ((bool)confirmCostText)
			{
				confirmCostText.text = currentSelectedItem?.GetCost().ToString();
			}
			if ((bool)confirmNameText)
			{
				confirmNameText.text = currentSelectedItem?.GetDisplayName();
			}
			break;
		}
		}
		NestedFadeGroupBase nestedFadeGroupBase = stateFaders[(int)previousState];
		NestedFadeGroupBase newStateFader = stateFaders[(int)newState];
		float fadeTime = stateFadeDuration * 0.5f;
		if ((bool)nestedFadeGroupBase)
		{
			yield return new WaitForSeconds(nestedFadeGroupBase.FadeTo(0f, fadeTime));
		}
		if ((bool)newStateFader)
		{
			yield return new WaitForSeconds(newStateFader.FadeTo(1f, fadeTime));
		}
		switch (newState)
		{
		case State.Confirm:
			if ((bool)confirmList)
			{
				confirmList.SetActive(value: true);
			}
			break;
		case State.ItemList:
			if ((bool)confirmList)
			{
				confirmList.gameObject.SetActive(value: false);
				confirmList.SetActive(value: false);
			}
			break;
		}
		state = newState;
		transitionStateRoutine = null;
	}

	private ISimpleShopItem GetCurrentSelectedItem()
	{
		if (selectedIndex < 0)
		{
			return null;
		}
		return shopItems?[selectedIndex];
	}
}
