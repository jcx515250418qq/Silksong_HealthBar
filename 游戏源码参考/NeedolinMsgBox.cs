using System.Collections;
using System.Collections.Generic;
using TMProOld;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class NeedolinMsgBox : MonoBehaviour
{
	[SerializeField]
	private NestedFadeGroupBase boxFade;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private TMP_Text primaryText;

	[SerializeField]
	private SpriteRenderer primaryBackboard;

	[SerializeField]
	private TMP_Text secondaryText;

	[SerializeField]
	private SpriteRenderer secondaryBackboard;

	[SerializeField]
	private Sprite[] backboardSprites;

	[SerializeField]
	private float appearPause;

	[SerializeField]
	private float hidePause;

	[SerializeField]
	private float hideInstantSpeedModifier = 2.5f;

	[Space]
	[SerializeField]
	[AssetPickerDropdown]
	private NeedolinTextConfig defaultConfig;

	private readonly List<ILocalisedTextCollection> currentTexts = new List<ILocalisedTextCollection>();

	private readonly List<ILocalisedTextCollection> maxPriorityTexts = new List<ILocalisedTextCollection>();

	private readonly List<object> blockers = new List<object>();

	private readonly HashSet<ILocalisedTextCollection> uniqueTextsTempLow = new HashSet<ILocalisedTextCollection>();

	private readonly HashSet<ILocalisedTextCollection> uniqueTextsTempHigh = new HashSet<ILocalisedTextCollection>();

	private LocalisedString previousText;

	private ILocalisedTextCollection previousCollection;

	private ILocalisedTextCollection queuedTextCollection;

	private Dictionary<ILocalisedTextCollection, int> gapsLeft = new Dictionary<ILocalisedTextCollection, int>();

	private Coroutine cycleTextsRoutine;

	private Coroutine hideRoutine;

	private bool willSkipStartDelay;

	private int currentBackboardSpriteIndex;

	private bool hasAppeared;

	private GameManager gm;

	private static NeedolinMsgBox _instance;

	private static readonly int _speedId = Animator.StringToHash("Speed");

	private static readonly int _appearId = Animator.StringToHash("Appear");

	private static readonly int _swapTextId = Animator.StringToHash("Swap Text");

	private static readonly int _disappearId = Animator.StringToHash("Disappear");

	private bool isHidden = true;

	public static bool IsBlocked
	{
		get
		{
			if ((bool)_instance)
			{
				return _instance.blockers.Count > 0;
			}
			return false;
		}
	}

	private void Awake()
	{
		if (!_instance)
		{
			_instance = this;
		}
		if ((bool)boxFade)
		{
			boxFade.AlphaSelf = 0f;
		}
		if ((bool)animator)
		{
			animator.enabled = false;
		}
		gm = GameManager.instance;
		gm.NextSceneWillActivate += ClearAllText;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "DIALOGUE BOX APPEARING").ReceivedEvent += HideNeedolinMsgBox;
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
		if ((bool)gm)
		{
			gm.NextSceneWillActivate -= ClearAllText;
			gm = null;
		}
	}

	public static void AddText(ILocalisedTextCollection text, bool skipStartDelay = false, bool maxPriority = false)
	{
		if ((bool)_instance)
		{
			_instance.InternalAddText(text, skipStartDelay, maxPriority);
		}
	}

	public static void RemoveText(ILocalisedTextCollection text)
	{
		if ((bool)_instance)
		{
			_instance.InternalRemoveText(text);
		}
	}

	public static void AddBlocker(object blocker)
	{
		if ((bool)_instance)
		{
			_instance.blockers.Add(blocker);
			_instance.RemoveTextShared();
		}
	}

	public static void RemoveBlocker(object blocker)
	{
		if ((bool)_instance)
		{
			_instance.blockers.Remove(blocker);
			if (_instance.blockers.Count <= 0)
			{
				_instance.willSkipStartDelay = false;
				_instance.AddTextShared();
			}
		}
	}

	private void InternalAddText(ILocalisedTextCollection text, bool skipStartDelay, bool maxPriority)
	{
		if (!text.IsActive)
		{
			return;
		}
		if (maxPriority)
		{
			maxPriorityTexts.Add(text);
		}
		else
		{
			currentTexts.Add(text);
		}
		if (_instance.blockers.Count <= 0)
		{
			if (cycleTextsRoutine != null)
			{
				queuedTextCollection = text;
			}
			willSkipStartDelay = skipStartDelay;
			AddTextShared();
		}
	}

	private void AddTextShared()
	{
		if (cycleTextsRoutine == null && currentTexts.Count + maxPriorityTexts.Count > 0)
		{
			cycleTextsRoutine = StartCoroutine(CycleTexts());
		}
	}

	private void InternalRemoveText(ILocalisedTextCollection text)
	{
		currentTexts.Remove(text);
		maxPriorityTexts.Remove(text);
		RemoveTextShared();
	}

	private void RemoveTextShared()
	{
		if (currentTexts.Count + maxPriorityTexts.Count != 0 && blockers.Count <= 0)
		{
			return;
		}
		if (cycleTextsRoutine != null)
		{
			StopCoroutine(cycleTextsRoutine);
			cycleTextsRoutine = null;
		}
		if (hasAppeared)
		{
			hasAppeared = false;
			if (hideRoutine == null)
			{
				hideRoutine = StartCoroutine(Hide());
			}
		}
	}

	private void HideNeedolinMsgBox()
	{
		if (cycleTextsRoutine != null)
		{
			StopCoroutine(cycleTextsRoutine);
			cycleTextsRoutine = null;
		}
		if (!isHidden)
		{
			if (hideRoutine != null)
			{
				StopCoroutine(hideRoutine);
				hideRoutine = null;
			}
			currentTexts.Clear();
			maxPriorityTexts.Clear();
			animator.SetFloat(_speedId, hideInstantSpeedModifier);
			hideRoutine = StartCoroutine(Hide());
			hasAppeared = false;
			isHidden = true;
			queuedTextCollection = null;
			previousText = default(LocalisedString);
			previousCollection = null;
		}
	}

	private void ClearAllText()
	{
		currentTexts.Clear();
		maxPriorityTexts.Clear();
		blockers.Clear();
		if (cycleTextsRoutine != null)
		{
			StopCoroutine(cycleTextsRoutine);
			cycleTextsRoutine = null;
		}
		if (hideRoutine != null)
		{
			StopCoroutine(hideRoutine);
			hideRoutine = null;
		}
		if (hasAppeared)
		{
			animator.Play(_disappearId, 0, 1f);
			animator.Update(0f);
			hasAppeared = false;
			isHidden = true;
		}
		if ((bool)boxFade)
		{
			boxFade.AlphaSelf = 0f;
		}
		if ((bool)animator)
		{
			animator.enabled = false;
		}
		queuedTextCollection = null;
		previousText = default(LocalisedString);
		previousCollection = null;
	}

	private IEnumerator CycleTexts()
	{
		if (!willSkipStartDelay)
		{
			yield return new WaitForSecondsInterruptable(appearPause, () => willSkipStartDelay);
		}
		while (hideRoutine != null)
		{
			yield return null;
		}
		GetNewText(isFirst: true, out var _, out var _);
		GetNewText(isFirst: false, out var text2, out var config2);
		string text3 = (text2.IsEmpty ? string.Empty : ((string)text2));
		primaryText.text = text3;
		if (backboardSprites.Length != 0)
		{
			primaryBackboard.sprite = backboardSprites[currentBackboardSpriteIndex];
		}
		if ((bool)animator && !animator.enabled)
		{
			animator.enabled = true;
		}
		animator.SetFloat(_speedId, config2.SpeedMultiplier);
		isHidden = false;
		hasAppeared = true;
		bool didPlayAppear;
		if (!string.IsNullOrEmpty(text3))
		{
			didPlayAppear = true;
			animator.Play(_appearId, 0, 0f);
			yield return null;
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
		}
		else
		{
			didPlayAppear = false;
		}
		while (true)
		{
			yield return new WaitForSeconds(config2.HoldDuration);
			animator.SetFloat(_speedId, config2.SpeedMultiplier);
			if (didPlayAppear)
			{
				GetNewText(isFirst: false, out text2, out config2);
				text3 = (text2.IsEmpty ? string.Empty : ((string)text2));
			}
			if (!didPlayAppear || string.IsNullOrEmpty(text3))
			{
				float disappearLength;
				if (didPlayAppear)
				{
					animator.Play(_disappearId, 0, 0f);
					yield return null;
					disappearLength = animator.GetCurrentAnimatorStateInfo(0).length;
					yield return new WaitForSeconds(disappearLength + config2.HoldDuration);
				}
				else
				{
					disappearLength = 0f;
				}
				while (true)
				{
					GetNewText(isFirst: false, out text2, out config2);
					text3 = (text2.IsEmpty ? string.Empty : ((string)text2));
					if (!string.IsNullOrEmpty(text3))
					{
						break;
					}
					yield return new WaitForSeconds(disappearLength + config2.HoldDuration);
				}
				primaryText.text = text3;
				didPlayAppear = true;
				animator.Play(_appearId, 0, 0f);
				yield return null;
				yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
				continue;
			}
			secondaryText.text = primaryText.text;
			secondaryBackboard.sprite = primaryBackboard.sprite;
			primaryText.text = text3;
			if (backboardSprites.Length != 0)
			{
				currentBackboardSpriteIndex++;
				if (currentBackboardSpriteIndex >= backboardSprites.Length)
				{
					currentBackboardSpriteIndex = 0;
				}
			}
			primaryBackboard.sprite = backboardSprites[currentBackboardSpriteIndex];
			animator.Play(_swapTextId, 0, 0f);
			yield return null;
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
		}
	}

	private IEnumerator Hide(bool skipPause = false)
	{
		if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash != _disappearId)
		{
			while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
			{
				yield return null;
			}
		}
		if (!skipPause)
		{
			yield return new WaitForSeconds(hidePause);
		}
		isHidden = true;
		gapsLeft.Clear();
		if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash != _disappearId)
		{
			animator.Play(_disappearId, 0, 0f);
			yield return null;
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
		}
		hideRoutine = null;
	}

	private void GetNewText(bool isFirst, out LocalisedString text, out NeedolinTextConfig config)
	{
		ILocalisedTextCollection localisedTextCollection;
		if (maxPriorityTexts.Count > 0)
		{
			localisedTextCollection = maxPriorityTexts[Random.Range(0, maxPriorityTexts.Count)];
		}
		else if (queuedTextCollection != null && queuedTextCollection != previousCollection)
		{
			localisedTextCollection = queuedTextCollection;
			queuedTextCollection = null;
		}
		else
		{
			if (currentTexts.Count <= 0)
			{
				text = default(LocalisedString);
				config = defaultConfig;
				previousText = default(LocalisedString);
				previousCollection = null;
				return;
			}
			localisedTextCollection = null;
			currentTexts.Shuffle();
			int num;
			try
			{
				foreach (ILocalisedTextCollection currentText in currentTexts)
				{
					NeedolinTextConfig config2 = currentText.GetConfig();
					if ((bool)config2 && config2.EmptyStartGap > 0)
					{
						uniqueTextsTempLow.Add(currentText);
					}
					else
					{
						uniqueTextsTempHigh.Add(currentText);
					}
				}
			}
			finally
			{
				num = ((uniqueTextsTempHigh.Count > 0) ? uniqueTextsTempHigh.Count : uniqueTextsTempLow.Count);
				uniqueTextsTempLow.Clear();
				uniqueTextsTempHigh.Clear();
			}
			foreach (ILocalisedTextCollection currentText2 in currentTexts)
			{
				if (gapsLeft.TryGetValue(currentText2, out var value))
				{
					value--;
					if (value > 0)
					{
						gapsLeft[currentText2] = value;
					}
					else
					{
						gapsLeft.Remove(currentText2);
					}
				}
				else if (previousCollection == null || num <= 1 || currentText2 != previousCollection)
				{
					localisedTextCollection = currentText2;
				}
			}
			if (localisedTextCollection == null)
			{
				text = default(LocalisedString);
				config = defaultConfig;
				return;
			}
		}
		text = localisedTextCollection.GetRandom(previousText);
		config = localisedTextCollection.GetConfig();
		previousText = text;
		previousCollection = localisedTextCollection;
		if (!config)
		{
			config = defaultConfig;
		}
		int num2 = config.EmptyGap.GetRandomValue();
		if (isFirst)
		{
			num2 += config.EmptyStartGap;
		}
		if (num2 > 0)
		{
			gapsLeft[localisedTextCollection] = num2;
		}
	}
}
