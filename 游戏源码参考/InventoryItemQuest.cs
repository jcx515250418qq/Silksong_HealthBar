using System;
using System.Collections;
using GlobalSettings;
using TMProOld;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class InventoryItemQuest : InventoryItemUpdateable
{
	private enum DescriptionTypes
	{
		Inventory = 0,
		QuestBoard = 1
	}

	[Space]
	[SerializeField]
	private DescriptionTypes descriptionType;

	[SerializeField]
	private BasicQuestBase quest;

	[SerializeField]
	private SpriteRenderer icon;

	[SerializeField]
	private SpriteRenderer canCompleteIcon;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private TextMeshPro typeText;

	[SerializeField]
	private TextMeshPro nameText;

	[SerializeField]
	private IconCounter iconCounter;

	[SerializeField]
	private ImageSlider progressBar;

	[SerializeField]
	private ProgressBarSegmented progressBarSegmented;

	[SerializeField]
	private NestedFadeGroupBase group;

	[SerializeField]
	private float completedAlpha = 0.7f;

	[SerializeField]
	private NestedFadeGroupBase iconGroup;

	[SerializeField]
	private float completedIconAlpha = 1f;

	[Space]
	[SerializeField]
	private float wishPromptPause = 0.5f;

	[SerializeField]
	private CaptureAnimationEvent consumeEffect;

	[SerializeField]
	private CameraShakeTarget consumeShake;

	[SerializeField]
	private AudioEvent consumeAudio;

	private bool consumeBlocked;

	private bool wasInCompletedSection;

	private InventoryItemExtraDescription extraDesc;

	protected QuestItemManager Manager;

	private bool hasBlockedActions;

	private static readonly int _canCompleteProp = Animator.StringToHash("Can Complete");

	private static readonly int _idleProp = Animator.StringToHash("Idle");

	private bool updateQueued;

	public override string DisplayName
	{
		get
		{
			if (!quest)
			{
				return string.Empty;
			}
			return quest.DisplayName;
		}
	}

	public override string Description
	{
		get
		{
			if (!quest)
			{
				return string.Empty;
			}
			return descriptionType switch
			{
				DescriptionTypes.Inventory => quest.GetDescription(BasicQuestBase.ReadSource.Inventory), 
				DescriptionTypes.QuestBoard => quest.GetDescription(BasicQuestBase.ReadSource.QuestBoard), 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}
	}

	public override Color? CursorColor
	{
		get
		{
			if ((bool)quest && (bool)quest.QuestType)
			{
				return quest.QuestType.TextColor;
			}
			return base.CursorColor;
		}
	}

	public BasicQuestBase Quest => quest;

	protected override bool IsSeen
	{
		get
		{
			return quest.HasBeenSeen;
		}
		set
		{
			quest.HasBeenSeen = value;
		}
	}

	public event Action<BasicQuestBase> Submitted;

	public event Action Canceled;

	protected override void Awake()
	{
		base.Awake();
		extraDesc = GetComponent<InventoryItemExtraDescription>();
		Manager = GetComponentInParent<QuestItemManager>();
	}

	protected override void Start()
	{
		base.Start();
		if ((bool)quest)
		{
			SetQuest(quest, wasInCompletedSection);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (updateQueued)
		{
			updateQueued = false;
			if (Application.isPlaying)
			{
				StartCoroutine(RefreshRoutine());
			}
		}
	}

	protected IEnumerator RefreshRoutine()
	{
		yield return null;
		if ((bool)typeText)
		{
			typeText.gameObject.SetActive(value: false);
			typeText.gameObject.SetActive(value: true);
		}
		if ((bool)nameText)
		{
			nameText.gameObject.SetActive(value: false);
			nameText.gameObject.SetActive(value: true);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (hasBlockedActions && (bool)Manager)
		{
			Manager.IsActionsBlocked = false;
			hasBlockedActions = false;
		}
	}

	public virtual void SetQuest(BasicQuestBase newQuest, bool isInCompletedSection)
	{
		quest = newQuest;
		wasInCompletedSection = isInCompletedSection;
		if (!quest)
		{
			if ((bool)group)
			{
				group.AlphaSelf = 0f;
			}
			if ((bool)extraDesc)
			{
				extraDesc.ExtraDescPrefab = null;
			}
			return;
		}
		if ((bool)quest.QuestType)
		{
			if ((bool)icon)
			{
				icon.sprite = quest.QuestType.Icon;
			}
			if ((bool)typeText)
			{
				typeText.text = quest.QuestType.DisplayName;
				typeText.color = quest.QuestType.TextColor;
			}
		}
		if ((bool)nameText)
		{
			nameText.text = quest.DisplayName;
		}
		if ((bool)consumeEffect)
		{
			consumeEffect.gameObject.SetActive(value: false);
		}
		if ((bool)iconCounter)
		{
			iconCounter.gameObject.SetActive(value: false);
		}
		if ((bool)progressBar)
		{
			progressBar.gameObject.SetActive(value: false);
		}
		if ((bool)progressBarSegmented)
		{
			progressBarSegmented.gameObject.SetActive(value: false);
		}
		if ((bool)group)
		{
			group.AlphaSelf = 1f;
		}
		if ((bool)iconGroup)
		{
			iconGroup.AlphaSelf = 1f;
		}
		IQuestWithCompletion questWithCompletion = quest as IQuestWithCompletion;
		FullQuestBase fullQuestBase = quest as FullQuestBase;
		bool flag = questWithCompletion?.CanComplete ?? false;
		if (fullQuestBase != null && !isInCompletedSection)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (var targetsAndCounter in fullQuestBase.TargetsAndCounters)
			{
				FullQuestBase.QuestTarget item = targetsAndCounter.target;
				int item2 = targetsAndCounter.count;
				num += Mathf.Clamp(item2, 0, item.Count);
				num2 += item.Count;
				num3++;
			}
			if (num3 > 0)
			{
				switch (fullQuestBase.ListCounterType)
				{
				case FullQuestBase.ListCounterTypes.Dots:
					if ((bool)iconCounter)
					{
						iconCounter.gameObject.SetActive(value: true);
						iconCounter.MaxValue = (fullQuestBase.HideMax ? num : num2);
						iconCounter.CurrentValue = num;
						iconCounter.SetColor(fullQuestBase.ProgressBarTint);
					}
					break;
				case FullQuestBase.ListCounterTypes.Bar:
					if (num3 > 1 && (bool)progressBarSegmented)
					{
						progressBarSegmented.gameObject.SetActive(value: true);
						progressBarSegmented.SetSegmentCount(num3);
						int num4 = 0;
						foreach (var targetsAndCounter2 in fullQuestBase.TargetsAndCounters)
						{
							FullQuestBase.QuestTarget item3 = targetsAndCounter2.target;
							float progress = (float)targetsAndCounter2.count / (float)item3.Count;
							progressBarSegmented.SetSegmentProgress(num4, progress);
							num4++;
						}
					}
					else if ((bool)progressBar)
					{
						progressBar.gameObject.SetActive(value: true);
						progressBar.Value = (float)num / (float)num2;
						progressBar.Color = fullQuestBase.ProgressBarTint;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
				case FullQuestBase.ListCounterTypes.None:
					break;
				}
			}
			if ((bool)extraDesc)
			{
				extraDesc.ExtraDescPrefab = (fullQuestBase.IsDescCounterTypeCustom ? fullQuestBase.CustomDescPrefab : null);
			}
		}
		else if ((bool)extraDesc)
		{
			extraDesc.ExtraDescPrefab = null;
		}
		bool flag2 = false;
		if (isInCompletedSection)
		{
			if ((bool)group)
			{
				group.AlphaSelf = completedAlpha;
			}
			if ((bool)iconGroup)
			{
				iconGroup.AlphaSelf = completedIconAlpha;
			}
		}
		else if (quest is SubQuest && flag)
		{
			if ((bool)canCompleteIcon)
			{
				canCompleteIcon.sprite = quest.QuestType.CanCompleteIcon;
			}
			flag2 = true;
		}
		if ((bool)animator)
		{
			animator.Play(flag2 ? _canCompleteProp : _idleProp);
		}
		UpdateDisplay();
	}

	public void RegisterTextForUpdate()
	{
		updateQueued = true;
	}

	public override bool Submit()
	{
		if (this.Submitted == null || consumeBlocked)
		{
			return false;
		}
		if (base.isActiveAndEnabled && !quest.QuestType.IsDonateType)
		{
			StartCoroutine(ConsumeRoutine());
		}
		else
		{
			this.Submitted(quest);
		}
		return true;
	}

	public override bool Cancel()
	{
		if (this.Canceled == null)
		{
			return base.Cancel();
		}
		this.Canceled();
		return true;
	}

	public override void Select(InventoryItemManager.SelectionDirection? direction)
	{
		base.Select(direction);
		quest.OnSelected();
	}

	private IEnumerator ConsumeRoutine()
	{
		consumeBlocked = true;
		hasBlockedActions = true;
		Manager.IsActionsBlocked = true;
		bool hitTrigger;
		if ((bool)consumeEffect)
		{
			hitTrigger = false;
			consumeEffect.EventFired += Temp;
			consumeEffect.gameObject.SetActive(value: false);
			consumeEffect.gameObject.SetActive(value: true);
			consumeShake.DoShake(this);
			consumeAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
			while (!hitTrigger)
			{
				yield return null;
			}
		}
		if ((bool)group)
		{
			group.AlphaSelf = 0f;
			yield return new WaitForSeconds(wishPromptPause);
		}
		else
		{
			Debug.LogError("Nested Fade Group \"group\" needs to be assigned!", this);
			yield return new WaitForSeconds(wishPromptPause);
		}
		consumeBlocked = false;
		Manager.IsActionsBlocked = false;
		hasBlockedActions = false;
		this.Submitted(quest);
		void Temp()
		{
			hitTrigger = true;
			consumeEffect.EventFired -= Temp;
		}
	}
}
