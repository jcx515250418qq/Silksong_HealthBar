using System;
using System.Collections;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.Localization;
using UnityEngine;

public class CollectionViewerDesk : NPCControlBase
{
	[Serializable]
	public class Section
	{
		public NamedScriptableObjectListDummy List;

		[ModifiableProperty]
		[Conditional("List", true, false, false)]
		public LocalisedString Heading;

		public GameObject[] DisplayObjects;

		[Space]
		public PlayerDataTest UnlockTest;

		public CollectableItem UnlockItem;

		[PlayerDataField(typeof(bool), false)]
		public string UnlockSaveBool;

		[LocalisedString.NotRequired]
		public LocalisedString ConstructPrompt;

		public AudioEvent ConstructAudio = AudioEvent.Default;

		public string ConstructEventRegister;

		private Dictionary<object, object> tempDict;

		public bool IsActive
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(UnlockSaveBool))
				{
					return PlayerData.instance.GetBool(UnlockSaveBool);
				}
				return true;
			}
		}

		public bool IsUnlockable
		{
			get
			{
				if (IsActive)
				{
					return false;
				}
				if (!UnlockTest.IsFulfilled)
				{
					return false;
				}
				if ((bool)UnlockItem && UnlockItem.CollectedAmount <= 0)
				{
					return false;
				}
				if ((bool)List && !CheckIsListActive(checkVisible: true, null))
				{
					return false;
				}
				return true;
			}
		}

		public void Unlock()
		{
			if ((bool)UnlockItem)
			{
				UnlockItem.Take(1, showCounter: false);
			}
			PlayerData.instance.SetBool(UnlockSaveBool, value: true);
		}

		public bool CheckIsListActive(bool checkVisible, Action<ICollectionViewerItem> addFunc)
		{
			if (!(List is IEnumerable enumerable))
			{
				return false;
			}
			if (tempDict == null)
			{
				tempDict = new Dictionary<object, object>();
			}
			foreach (object item in enumerable)
			{
				if (item is CollectableItemMemento { CountKey: var countKey })
				{
					if (!tempDict.TryGetValue(countKey, out var value) || !(value is CollectableItemMemento collectableItemMemento2) || !collectableItemMemento2.IsListedInCollection())
					{
						if ((bool)countKey)
						{
							tempDict[countKey] = item;
						}
						else
						{
							tempDict[item] = item;
						}
					}
				}
				else
				{
					tempDict[item] = item;
				}
			}
			bool result = false;
			foreach (var (_, obj3) in tempDict)
			{
				if (!(obj3 is ICollectionViewerItemList collectionViewerItemList))
				{
					if (obj3 is ICollectionViewerItem collectionViewerItem && (collectionViewerItem.IsListedInCollection() || (!checkVisible && collectionViewerItem.IsRequiredInCollection())))
					{
						result = true;
						if (addFunc == null)
						{
							return true;
						}
						addFunc(collectionViewerItem);
					}
					continue;
				}
				foreach (ICollectionViewerItem collectionViewerItem2 in collectionViewerItemList.GetCollectionViewerItems())
				{
					if (!checkVisible || collectionViewerItem2.IsListedInCollection())
					{
						result = true;
						if (addFunc == null)
						{
							return true;
						}
						addFunc(collectionViewerItem2);
					}
				}
			}
			return result;
		}
	}

	[Space]
	[SerializeField]
	private CollectionViewBoard board;

	[Space]
	[SerializeField]
	private List<Section> sections;

	[Space]
	[SerializeField]
	private GameObject activeWhileInteractable;

	[SerializeField]
	private GameObject activeWhileNew;

	[SerializeField]
	private CollectionGramaphone gramaphone;

	[Space]
	[SerializeField]
	private tk2dSpriteAnimator sitDownHornet;

	[SerializeField]
	private RandomAudioClipTable hornetMoveSfx;

	[SerializeField]
	private GameObject inertSeat;

	[SerializeField]
	private float screenFadeTime;

	[SerializeField]
	private float fadeHoldTime;

	[SerializeField]
	private Transform mementosParent;

	[SerializeField]
	private GameObject heartMementosGroup;

	[SerializeField]
	private Transform heartMementosParent;

	[SerializeField]
	private float mementoAppearDelay;

	[SerializeField]
	private GameObject mementoAppearEffect;

	[SerializeField]
	private float mementoAppearEndDelay;

	[Space]
	[SerializeField]
	private NPCControlBase onlyMementosInspect;

	private bool didStartLoad;

	public IReadOnlyList<Section> Sections => sections;

	public bool HasGramaphone
	{
		get
		{
			if ((bool)gramaphone)
			{
				return gramaphone.gameObject.activeInHierarchy;
			}
			return false;
		}
	}

	public IEnumerable<Transform> MementosChildren
	{
		get
		{
			if ((bool)mementosParent)
			{
				foreach (Transform item in mementosParent)
				{
					if (!(item.gameObject == heartMementosGroup))
					{
						yield return item;
					}
				}
			}
			if (!heartMementosParent)
			{
				yield break;
			}
			foreach (Transform item2 in heartMementosParent)
			{
				yield return item2;
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		EventRegister.GetRegisterGuaranteed(base.gameObject, "ACTIVATE DISPLAYS").ReceivedEvent += ActivateDisplays;
		if ((bool)sitDownHornet)
		{
			sitDownHornet.gameObject.SetActive(value: false);
		}
		if ((bool)inertSeat)
		{
			inertSeat.SetActive(value: true);
		}
		if ((bool)mementoAppearEffect)
		{
			mementoAppearEffect.SetActive(value: false);
		}
		didStartLoad = true;
		DoForEachRelic(delegate(CollectableRelic relic)
		{
			relic.LoadClips();
		});
	}

	protected override void Start()
	{
		base.Start();
		ActivateDisplays();
		PlayerData instance = PlayerData.instance;
		foreach (Transform mementosChild in MementosChildren)
		{
			mementosChild.gameObject.SetActive(instance.MementosDeposited.GetData(mementosChild.name).IsDeposited);
		}
		ActivateHeartsGroup();
	}

	private void OnDestroy()
	{
		if (didStartLoad)
		{
			didStartLoad = false;
			DoForEachRelic(delegate(CollectableRelic relic)
			{
				relic.FreeClips();
			});
		}
	}

	private void DoForEachRelic(Action<CollectableRelic> action)
	{
		foreach (Section section in Sections)
		{
			if (!(section.List is IEnumerable enumerable))
			{
				continue;
			}
			foreach (object item in enumerable)
			{
				if (!(item is CollectableItemRelicType collectableItemRelicType))
				{
					if (item is CollectableRelic obj)
					{
						action(obj);
					}
					continue;
				}
				foreach (CollectableRelic relic in collectableItemRelicType.Relics)
				{
					action(relic);
				}
			}
		}
	}

	protected override void OnStartDialogue()
	{
		DisableInteraction();
		LocalisedString constructPrompt = default(LocalisedString);
		int constructIndex = -1;
		SavedItem takeItem = null;
		FindNext();
		void FindNext()
		{
			int num = constructIndex + 1;
			constructIndex = -1;
			for (int i = num; i < Sections.Count; i++)
			{
				Section section = Sections[i];
				if (!section.ConstructPrompt.IsEmpty && section.IsUnlockable)
				{
					constructPrompt = section.ConstructPrompt;
					constructIndex = i;
					takeItem = section.UnlockItem;
					break;
				}
			}
			if (constructIndex < 0)
			{
				GetIsActive(canConstruct: false, out var isAnyActive, out var isAnyUnlockable, out var _);
				StartCoroutine((isAnyActive || isAnyUnlockable) ? SitDownSequence(-1) : GetUpSequence());
			}
			else
			{
				GameCameras.instance.HUDOut();
				DialogueYesNoBox.Open(delegate
				{
					Section section2 = sections[constructIndex];
					string text = ((constructIndex >= 0) ? section2.ConstructEventRegister : null);
					if (!string.IsNullOrWhiteSpace(text))
					{
						section2.Unlock();
						EndDialogue();
						EventRegister.SendEvent(text);
					}
					else
					{
						StartCoroutine(SitDownSequence(constructIndex));
					}
				}, FindNext, returnHud: false, constructPrompt, takeItem, 1, displayHudPopup: false);
			}
		}
	}

	protected override void OnEndDialogue()
	{
		EnableInteraction();
	}

	private void GetIsActive(bool canConstruct, out bool isAnyActive, out bool isAnyUnlockable, out bool isJustMementos)
	{
		isAnyActive = false;
		isAnyUnlockable = false;
		isJustMementos = false;
		foreach (Section section in Sections)
		{
			if (!isAnyActive && section.IsActive && section.CheckIsListActive(checkVisible: true, null))
			{
				if (!isAnyActive && section.List == Gameplay.MementoList)
				{
					isJustMementos = true;
				}
				else if (isJustMementos)
				{
					isJustMementos = false;
				}
				isAnyActive = true;
			}
			if ((canConstruct || section.ConstructPrompt.IsEmpty) && !isAnyUnlockable && section.IsUnlockable)
			{
				isAnyUnlockable = true;
			}
		}
		PlayerData instance = PlayerData.instance;
		foreach (Transform mementosChild in MementosChildren)
		{
			string itemName = mementosChild.name;
			if (instance.Collectables.GetData(itemName).Amount > 0)
			{
				CollectableItem itemByName = CollectableItemManager.GetItemByName(itemName);
				if ((object)itemByName == null || ((ICollectionViewerItem)itemByName).CanDeposit)
				{
					isAnyUnlockable = true;
					isJustMementos = true;
				}
			}
		}
	}

	private void OnBoardClosed()
	{
		board.BoardClosed -= OnBoardClosed;
		StartCoroutine(GetUpSequence());
	}

	public void PlayOnGramaphone(CollectableRelic playingRelic)
	{
		StopPlayingRelic();
		gramaphone.Play(playingRelic, alreadyPlaying: false, null);
	}

	public void StopPlayingRelic()
	{
		gramaphone.Stop();
	}

	public CollectableRelic GetPlayingRelic()
	{
		if (!HasGramaphone)
		{
			return null;
		}
		return gramaphone.PlayingRelic;
	}

	private IEnumerator SitDownSequence(int constructIndex)
	{
		if ((bool)sitDownHornet)
		{
			HeroController instance = HeroController.instance;
			instance.GetComponent<MeshRenderer>().enabled = false;
			if ((bool)inertSeat)
			{
				inertSeat.SetActive(value: false);
			}
			hornetMoveSfx.SpawnAndPlayOneShot(instance.transform.position);
			sitDownHornet.gameObject.SetActive(value: true);
			yield return new WaitForSeconds(sitDownHornet.PlayAnimGetTime("Hornet Desk Sit"));
		}
		bool flag = false;
		Section constructingSection = null;
		for (int i = 0; i < sections.Count; i++)
		{
			Section section = sections[i];
			if (!section.IsUnlockable)
			{
				continue;
			}
			if (!section.ConstructPrompt.IsEmpty)
			{
				if (constructIndex != i)
				{
					continue;
				}
				constructingSection = section;
			}
			section.Unlock();
			flag = true;
		}
		if (flag)
		{
			ScreenFaderUtils.Fade(ScreenFaderUtils.GetColour(), Color.black, screenFadeTime);
			yield return new WaitForSeconds(screenFadeTime);
			if (constructingSection != null && (bool)constructingSection.ConstructAudio.Clip)
			{
				constructingSection.ConstructAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, Vector3.zero);
				yield return new WaitForSeconds(constructingSection.ConstructAudio.Clip.length);
			}
			ActivateDisplays();
			yield return new WaitForSeconds(fadeHoldTime);
			Color colour;
			Color startColour = (colour = ScreenFaderUtils.GetColour());
			colour.a = 0f;
			ScreenFaderUtils.Fade(startColour, colour, screenFadeTime);
			yield return new WaitForSeconds(screenFadeTime);
		}
		board.BoardClosed += OnBoardClosed;
		board.OpenBoard(this);
	}

	private void ActivateDisplays()
	{
		foreach (Section section in Sections)
		{
			section.DisplayObjects.SetAllActive(section.IsActive);
		}
		GetIsActive(canConstruct: true, out var isAnyActive, out var isAnyUnlockable, out var isJustMementos);
		bool flag = isAnyActive || isAnyUnlockable;
		if (!flag)
		{
			Deactivate(allowQueueing: false);
		}
		if ((bool)activeWhileInteractable)
		{
			activeWhileInteractable.SetActive(flag);
		}
		if ((bool)activeWhileNew)
		{
			activeWhileNew.SetActive(isAnyUnlockable && !isJustMementos);
		}
		if (isJustMementos && (bool)onlyMementosInspect)
		{
			base.PromptMarker = onlyMementosInspect.PromptMarker;
			base.HeroAnimation = onlyMementosInspect.HeroAnimation;
			base.TalkPosition = onlyMementosInspect.TalkPosition;
			base.CentreOffset = onlyMementosInspect.CentreOffset;
			base.TargetDistance = onlyMementosInspect.TargetDistance;
		}
	}

	private void ActivateHeartsGroup()
	{
		if (!heartMementosParent)
		{
			return;
		}
		bool active = false;
		foreach (Transform item in heartMementosParent)
		{
			if (item.gameObject.activeSelf)
			{
				active = true;
			}
		}
		if ((bool)heartMementosGroup)
		{
			heartMementosGroup.SetActive(active);
		}
	}

	private IEnumerator GetUpSequence()
	{
		HeroController hc = HeroController.instance;
		HeroAnimationController heroAnim = hc.GetComponent<HeroAnimationController>();
		if ((bool)sitDownHornet)
		{
			hornetMoveSfx.SpawnAndPlayOneShot(hc.transform.position);
			yield return new WaitForSeconds(sitDownHornet.PlayAnimGetTime("Hornet Desk Stand"));
			sitDownHornet.gameObject.SetActive(value: false);
			if ((bool)inertSeat)
			{
				inertSeat.SetActive(value: true);
			}
			hc.GetComponent<MeshRenderer>().enabled = true;
			heroAnim.SetPlayRunToIdle();
		}
		else if (heroAnim.animator.IsPlaying("Abyss Kneel"))
		{
			hornetMoveSfx.SpawnAndPlayOneShot(hc.transform.position);
			yield return new WaitForSeconds(heroAnim.animator.PlayAnimGetTime(heroAnim.GetClip("Abyss Kneel to Stand")));
		}
		GameCameras.instance.HUDIn();
		EndDialogue();
	}

	public void DoMementoDeposit(string mementoName)
	{
		foreach (Transform mementosChild in MementosChildren)
		{
			if (!(mementosChild.name != mementoName))
			{
				if ((bool)mementoAppearEffect)
				{
					mementoAppearEffect.SetActive(value: false);
					mementoAppearEffect.transform.SetPosition2D(mementosChild.transform.position);
					mementoAppearEffect.SetActive(value: true);
				}
				mementosChild.gameObject.SetActive(value: true);
				ActivateHeartsGroup();
			}
		}
	}
}
