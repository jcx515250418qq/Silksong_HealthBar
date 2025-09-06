using System;
using System.Collections;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;

public class BossChallengeUI : MonoBehaviour
{
	public delegate void HideEvent();

	public delegate void LevelSelectedEvent();

	[Serializable]
	public class ButtonDisplay
	{
		public Image completeImage;

		public Image incompleteImage;

		public MenuSelectable button;

		public float enabledAlpha = 1f;

		public float disabledAlpha = 0.5f;

		public void SetupNavigation(bool isActive, ButtonDisplay selectOnUp, ButtonDisplay selectOnDown)
		{
			button.interactable = isActive;
			Navigation navigation = button.navigation;
			navigation.selectOnUp = selectOnUp.button;
			navigation.selectOnDown = selectOnDown.button;
			button.navigation = navigation;
			CanvasGroup component = button.GetComponent<CanvasGroup>();
			if ((bool)component)
			{
				component.alpha = (isActive ? enabledAlpha : disabledAlpha);
			}
		}

		public void SetState(bool isComplete)
		{
			if ((bool)completeImage)
			{
				completeImage.gameObject.SetActive(isComplete);
			}
			if ((bool)incompleteImage)
			{
				incompleteImage.gameObject.SetActive(!isComplete);
			}
		}
	}

	private BossStatue bossStatue;

	public Text bossNameText;

	public Text descriptionText;

	[Space]
	public MenuSelectable firstSelected;

	public string closeStateName = "GG_Challenge_Close";

	public ButtonDisplay tier1Button;

	public ButtonDisplay tier2Button;

	public ButtonDisplay tier3Button;

	public GameObject tier3UnlockEffect;

	public float tier3UnlockEffectDelay = 0.5f;

	private static int lastSelectedButton = -1;

	private Canvas canvas;

	private Animator animator;

	private CanvasGroup group;

	public event HideEvent OnCancel;

	public event LevelSelectedEvent OnLevelSelected;

	private void Awake()
	{
		canvas = GetComponent<Canvas>();
		animator = GetComponent<Animator>();
		group = GetComponent<CanvasGroup>();
		if ((bool)group)
		{
			group.alpha = 0f;
		}
	}

	private void Start()
	{
		if ((bool)canvas && (bool)GameCameras.instance && (bool)GameCameras.instance.hudCamera)
		{
			canvas.worldCamera = GameCameras.instance.hudCamera;
		}
	}

	public void Setup(BossStatue bossStatue, string bossNameSheet, string bossNameKey, string descriptionSheet, string descriptionKey)
	{
		this.bossStatue = bossStatue;
		if ((bool)bossNameText)
		{
			bossNameText.text = Language.Get(bossNameKey, bossNameSheet);
		}
		if ((bool)descriptionText)
		{
			descriptionText.text = Language.Get(descriptionKey, descriptionSheet);
		}
		if (!bossStatue.hasNoTiers)
		{
			BossStatue.Completion completion = (bossStatue.UsingDreamVersion ? bossStatue.DreamStatueState : bossStatue.StatueState);
			tier1Button.SetState(completion.completedTier1);
			tier2Button.SetState(completion.completedTier2);
			tier3Button.SetState(completion.completedTier3);
			tier1Button.SetupNavigation(isActive: true, completion.completedTier2 ? tier3Button : tier2Button, tier2Button);
			tier2Button.SetupNavigation(isActive: true, tier1Button, completion.completedTier2 ? tier3Button : tier1Button);
			tier3Button.SetupNavigation(completion.completedTier2, tier2Button, tier1Button);
			if ((bool)tier3UnlockEffect && completion.completedTier2 && !completion.seenTier3Unlock)
			{
				StartCoroutine(ShowUnlockEffect());
			}
			StartCoroutine(SetFirstSelected());
		}
		else
		{
			LoadBoss(0, doHideAnim: false);
		}
	}

	private IEnumerator ShowUnlockEffect()
	{
		BossStatue.Completion state = (bossStatue.UsingDreamVersion ? bossStatue.DreamStatueState : bossStatue.StatueState);
		yield return new WaitForSeconds(tier3UnlockEffectDelay);
		tier3UnlockEffect.SetActive(value: true);
		state.seenTier3Unlock = true;
		if (bossStatue.UsingDreamVersion)
		{
			bossStatue.DreamStatueState = state;
		}
		else
		{
			bossStatue.StatueState = state;
		}
	}

	private IEnumerator SetFirstSelected()
	{
		MenuSelectable select = firstSelected;
		if (lastSelectedButton >= 0)
		{
			switch (lastSelectedButton)
			{
			case 0:
				if ((bool)tier1Button.button && tier1Button.button.interactable)
				{
					select = tier1Button.button;
				}
				break;
			case 1:
				if ((bool)tier2Button.button && tier2Button.button.interactable)
				{
					select = tier2Button.button;
				}
				break;
			case 2:
				if ((bool)tier3Button.button && tier3Button.button.interactable)
				{
					select = tier3Button.button;
				}
				break;
			}
		}
		if ((bool)select)
		{
			select.ForceDeselect();
			yield return null;
			UIManager.HighlightSelectableNoSound(select);
			ManagerSingleton<InputHandler>.Instance.StartUIInput();
		}
	}

	public void Hide()
	{
		Hide(doAnim: true);
	}

	public void Hide(bool doAnim)
	{
		if (doAnim)
		{
			StartCoroutine(HideAnim());
			return;
		}
		if (this.OnCancel != null)
		{
			this.OnCancel();
		}
		base.gameObject.SetActive(value: false);
	}

	private IEnumerator HideAnim()
	{
		if ((bool)animator)
		{
			animator.Play(closeStateName);
			AnimatorClipInfo[] currentAnimatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
			yield return new WaitForSeconds(currentAnimatorClipInfo[0].clip.length);
		}
		if ((bool)tier3UnlockEffect)
		{
			tier3UnlockEffect.SetActive(value: false);
		}
		if (this.OnCancel != null)
		{
			this.OnCancel();
		}
		base.gameObject.SetActive(value: false);
	}

	public void LoadBoss(int level)
	{
		LoadBoss(level, doHideAnim: true);
	}

	public void LoadBoss(int level, bool doHideAnim)
	{
		BossScene bossScene = (bossStatue.UsingDreamVersion ? bossStatue.dreamBossScene : bossStatue.bossScene);
		string text = bossScene.sceneName;
		switch (level)
		{
		case 0:
			text = bossScene.Tier1Scene;
			break;
		case 1:
			text = bossScene.Tier2Scene;
			break;
		case 2:
			text = bossScene.Tier3Scene;
			break;
		}
		if (!Application.CanStreamedLevelBeLoaded(text))
		{
			Hide(doHideAnim);
			Debug.LogError($"Could not start boss scene. Scene: \"{text}\" does not exist!");
			return;
		}
		StaticVariableList.SetValue("bossSceneToLoad", text);
		this.OnCancel = null;
		Hide(doHideAnim);
		GameManager.instance.playerData.bossReturnEntryGate = bossStatue.dreamReturnGate.name;
		BossSceneController.SetupEvent = delegate(BossSceneController self)
		{
			self.BossLevel = level;
			self.DreamReturnEvent = "DREAM RETURN";
			self.OnBossesDead += delegate
			{
				string fieldName = (bossStatue.UsingDreamVersion ? bossStatue.dreamStatueStatePD : bossStatue.statueStatePD);
				BossStatue.Completion playerDataVariable = GameManager.instance.GetPlayerDataVariable<BossStatue.Completion>(fieldName);
				switch (level)
				{
				case 0:
					playerDataVariable.completedTier1 = true;
					break;
				case 1:
					playerDataVariable.completedTier2 = true;
					break;
				case 2:
					playerDataVariable.completedTier3 = true;
					break;
				}
				GameManager.instance.SetPlayerDataVariable(fieldName, playerDataVariable);
				GameManager.instance.playerData.currentBossStatueCompletionKey = (bossStatue.UsingDreamVersion ? bossStatue.dreamStatueStatePD : bossStatue.statueStatePD);
				GameManager.instance.playerData.bossStatueTargetLevel = level;
			};
			self.OnBossSceneComplete += delegate
			{
				self.DoDreamReturn();
			};
		};
		if (this.OnLevelSelected != null)
		{
			this.OnLevelSelected();
		}
	}

	public void RecordLastSelected(int index)
	{
		lastSelectedButton = index;
	}
}
