using System;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.Localization;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Quests/Main Quest")]
public class MainQuest : FullQuestBase
{
	[Serializable]
	public struct AltQuestTarget
	{
		public QuestTargetCounter Counter;

		public int Count;

		public PlayerDataTest AltTest;
	}

	[Header("- Main Quest")]
	[SerializeField]
	private LocalisedString typeDisplayName = new LocalisedString
	{
		Sheet = "Quests",
		Key = "TYPE_"
	};

	[SerializeField]
	private Color typeColor = new Color(0.5960785f, 0.5960785f, 0.5960785f, 1f);

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Sprite typeIcon;

	[SerializeField]
	private Sprite typeLargeIcon;

	[SerializeField]
	private Sprite typeLargeIconGlow;

	[Space]
	[SerializeField]
	private SubQuest[] subQuests;

	[SerializeField]
	private AltQuestTarget[] altTargets;

	[SerializeField]
	private int subQuestsRequired;

	private QuestType questType;

	public override QuestType QuestType => questType;

	public IReadOnlyList<SubQuest> SubQuests => subQuests;

	public IReadOnlyList<AltQuestTarget> AltTargets => altTargets;

	public override bool CanComplete
	{
		get
		{
			if (subQuests == null)
			{
				return base.CanComplete;
			}
			GetCompletionCount(out var subQuestsComplete, out var altTargetsComplete);
			subQuestsComplete += altTargetsComplete;
			if (subQuestsRequired > 0)
			{
				if (subQuestsComplete < subQuestsRequired)
				{
					return false;
				}
			}
			else if (subQuestsComplete < subQuests.Length)
			{
				return false;
			}
			return base.CanComplete;
		}
	}

	public bool IsAnyAltTargetsComplete
	{
		get
		{
			if (!CanComplete)
			{
				return false;
			}
			GetCompletionCount(out var _, out var altTargetsComplete);
			return altTargetsComplete > 0;
		}
	}

	public override bool IsMapMarkerVisible
	{
		get
		{
			if (subQuests == null)
			{
				return base.IsMapMarkerVisible;
			}
			SubQuest[] array = subQuests;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsMapMarkerVisible)
				{
					return false;
				}
			}
			return base.IsMapMarkerVisible;
		}
	}

	public bool WouldMapMarkerBeVisible
	{
		get
		{
			if (base.IsMapMarkerVisible)
			{
				return !IsAnyAltTargetsComplete;
			}
			return false;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		questType = QuestType.Create(typeDisplayName, typeIcon, typeColor, typeLargeIcon, typeLargeIconGlow);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		UnityEngine.Object.DestroyImmediate(questType);
		questType = null;
	}

	protected override void ShowQuestAccepted(Action afterPrompt, bool hasPreviousQuest)
	{
		UIMsgDisplay uIMsgDisplay = default(UIMsgDisplay);
		uIMsgDisplay.Name = (hasPreviousQuest ? UI.MainQuestProgressPopup : UI.MainQuestBeginPopup);
		uIMsgDisplay.Icon = typeIcon;
		uIMsgDisplay.IconScale = 1f;
		uIMsgDisplay.RepresentingObject = this;
		CollectableUIMsg.Spawn(uIMsgDisplay, null, forceReplacingEffect: true);
		UI.QuestContinuePopupSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, Vector3.zero);
		afterPrompt?.Invoke();
	}

	protected override void ShowQuestCompleted(Action afterPrompt)
	{
		UIMsgDisplay uIMsgDisplay = default(UIMsgDisplay);
		uIMsgDisplay.Name = UI.MainQuestCompletePopup;
		uIMsgDisplay.Icon = typeIcon;
		uIMsgDisplay.IconScale = 1f;
		uIMsgDisplay.RepresentingObject = this;
		CollectableUIMsg.Spawn(uIMsgDisplay, null, forceReplacingEffect: true);
		UI.QuestContinuePopupSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, Vector3.zero);
		afterPrompt?.Invoke();
	}

	protected override bool ConsumeTargets()
	{
		bool result = base.ConsumeTargets();
		if (subQuests == null)
		{
			return result;
		}
		SubQuest[] array = subQuests;
		foreach (SubQuest subQuest in array)
		{
			QuestTargetCounter questTargetCounter = subQuest.TargetCounter;
			if (!(questTargetCounter == null))
			{
				questTargetCounter.Consume(subQuest.TargetCount, showCounter: false);
				result = true;
			}
		}
		return result;
	}

	public void GetCompletionCount(out int subQuestsComplete, out int altTargetsComplete)
	{
		subQuestsComplete = 0;
		altTargetsComplete = 0;
		SubQuest[] array = subQuests;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].GetCurrent().CanGetMore())
			{
				subQuestsComplete++;
			}
		}
		AltQuestTarget[] array2 = altTargets;
		for (int i = 0; i < array2.Length; i++)
		{
			AltQuestTarget altQuestTarget = array2[i];
			int num = ((!altQuestTarget.AltTest.IsDefined || !altQuestTarget.AltTest.IsFulfilled) ? altQuestTarget.Counter.GetCompletionAmount(default(QuestCompletionData.Completion)) : altQuestTarget.Count);
			if (num >= altQuestTarget.Count)
			{
				altTargetsComplete++;
			}
		}
	}
}
