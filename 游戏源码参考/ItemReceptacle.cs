using System;
using System.Collections;
using TMProOld;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class ItemReceptacle : NPCControlBase
{
	[Space]
	[SerializeField]
	private LocalisedString inspectDialogue;

	[SerializeField]
	private LocalisedString promptFormatText;

	[SerializeField]
	private CollectableItem requiredItem;

	[SerializeField]
	private int requiredItemCount;

	[SerializeField]
	private bool skipInspectOnUse;

	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string playerDataBool;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("playerDataBool", false, false, false)]
	private PersistentBoolItem persistent;

	[Space]
	[SerializeField]
	private AudioEvent unlockSound;

	[SerializeField]
	private Transform unlockEffectPoint;

	[SerializeField]
	private GameObject unlockEffectPrefab;

	[SerializeField]
	private float unlockDelay;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private bool animatorStartActive;

	[SerializeField]
	private float unlockEventDelay;

	[Space]
	[SerializeField]
	private UnlockablePropBase unlock;

	[Space]
	[SerializeField]
	private UnityEvent onUnlockEffect;

	[SerializeField]
	private UnityEvent onPreUnlock;

	[SerializeField]
	private UnityEvent onUnlock;

	[SerializeField]
	private UnityEvent onStartUnlocked;

	[Space]
	[SerializeField]
	private PlayMakerFSM inspectEventTarget;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsEventValid")]
	private string inspectEndEvent;

	private static readonly int _unlockAnim = Animator.StringToHash("Unlock");

	private bool isActivated;

	private bool yesNoOpened;

	public override bool AutoEnd
	{
		get
		{
			if ((bool)inspectEventTarget)
			{
				return string.IsNullOrEmpty(inspectEndEvent);
			}
			return true;
		}
	}

	public event Action Unlocked;

	public event Action StartedUnlocked;

	private bool? IsEventValid(string eventName)
	{
		if ((bool)inspectEventTarget)
		{
			return inspectEventTarget.IsEventValid(eventName, isRequired: true);
		}
		return null;
	}

	protected override void Awake()
	{
		base.Awake();
		if (!persistent || !string.IsNullOrEmpty(playerDataBool))
		{
			return;
		}
		persistent.OnGetSaveState += delegate(out bool value)
		{
			value = isActivated;
		};
		persistent.OnSetSaveState += delegate(bool value)
		{
			isActivated = value;
			if (isActivated)
			{
				StartedActivated();
			}
		};
	}

	protected override void Start()
	{
		if ((bool)animator && !animatorStartActive)
		{
			animator.enabled = true;
			animator.Play(_unlockAnim, 0, 0f);
			animator.Update(0f);
			animator.enabled = false;
		}
		if (!string.IsNullOrEmpty(playerDataBool))
		{
			isActivated = PlayerData.instance.GetVariable<bool>(playerDataBool);
			if (isActivated)
			{
				StartedActivated();
			}
		}
	}

	private void StartedActivated()
	{
		if ((bool)animator)
		{
			animator.enabled = true;
			animator.Play(_unlockAnim, 0, 1f);
			animator.Update(0f);
			animator.enabled = false;
		}
		if ((bool)unlock)
		{
			unlock.Opened();
		}
		onStartUnlocked.Invoke();
		this.StartedUnlocked?.Invoke();
		Deactivate(allowQueueing: false);
	}

	protected override void OnStartDialogue()
	{
		DisableInteraction();
		yesNoOpened = false;
		bool hasItem = (bool)requiredItem && requiredItem.CollectedAmount >= requiredItemCount;
		if (hasItem && skipInspectOnUse)
		{
			OpenYesNo();
			return;
		}
		DialogueBox.StartConversation(inspectDialogue, this, overrideContinue: false, new DialogueBox.DisplayOptions
		{
			ShowDecorators = true,
			Alignment = TextAlignmentOptions.Top,
			TextColor = Color.white
		}, delegate
		{
			if (hasItem)
			{
				OpenYesNo();
			}
		});
		void OpenYesNo()
		{
			yesNoOpened = true;
			DialogueYesNoBox.Open(AcceptedPrompt, CanceledPrompt, returnHud: true, promptFormatText, requiredItem, requiredItemCount, displayHudPopup: false);
		}
	}

	public override void OnDialogueBoxEnded()
	{
		if (!AutoEnd)
		{
			DialogueBox.EndConversation();
		}
	}

	protected override void OnEndDialogue()
	{
		if (!yesNoOpened)
		{
			EnableInteraction();
			if ((bool)inspectEventTarget)
			{
				inspectEventTarget.SendEvent(inspectEndEvent);
			}
		}
	}

	private void AcceptedPrompt()
	{
		StartCoroutine(UnlockSequence());
	}

	private void CanceledPrompt()
	{
		EnableInteraction();
	}

	private IEnumerator UnlockSequence()
	{
		HeroController hc = HeroController.instance;
		tk2dSpriteAnimator heroAnimator = hc.GetComponent<tk2dSpriteAnimator>();
		HeroAnimationController heroAnim = hc.GetComponent<HeroAnimationController>();
		hc.StopAnimationControl();
		heroAnimator.Play(heroAnim.GetClip("Collect Stand 1"));
		yield return new WaitForSeconds(0.75f);
		heroAnimator.Play(heroAnim.GetClip("Collect Stand 2"));
		requiredItem.Take(requiredItemCount, showCounter: false);
		isActivated = true;
		if (!string.IsNullOrEmpty(playerDataBool))
		{
			PlayerData.instance.SetVariable(playerDataBool, value: true);
		}
		if ((bool)unlockEffectPrefab)
		{
			Transform transform = (unlockEffectPoint ? unlockEffectPoint : base.transform);
			unlockEffectPrefab.Spawn(transform.position);
		}
		unlockSound.SpawnAndPlayOneShot(base.transform.position);
		onUnlockEffect.Invoke();
		yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(heroAnimator.PlayAnimWait(heroAnim.GetClip("Collect Stand 3")));
		hc.StartAnimationControl();
		EnableInteraction();
		Deactivate(allowQueueing: false);
		yield return new WaitForSeconds(unlockDelay);
		onPreUnlock.Invoke();
		if ((bool)animator)
		{
			animator.enabled = true;
			animator.Play(_unlockAnim, 0, 0f);
			yield return null;
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
		}
		yield return new WaitForSeconds(unlockEventDelay);
		onUnlock.Invoke();
		if ((bool)unlock)
		{
			unlock.Open();
		}
		this.Unlocked?.Invoke();
	}
}
