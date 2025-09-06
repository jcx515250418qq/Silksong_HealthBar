using System.Collections;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BossDoorChallengeUI : MonoBehaviour
{
	public delegate void HideEvent();

	public delegate void BeginEvent();

	public Text titleTextSuper;

	public Text titleTextMain;

	public Text descriptionText;

	public BossDoorChallengeUIBindingButton boundNailButton;

	public BossDoorChallengeUIBindingButton boundHeartButton;

	public BossDoorChallengeUIBindingButton boundCharmsButton;

	public BossDoorChallengeUIBindingButton boundSoulButton;

	private BossDoorChallengeUIBindingButton[] buttons;

	private bool allPreviouslySelected;

	public AudioSource audioPlayerPrefab;

	public AudioEvent allSelectedSound;

	public GameObject allSelectedEffect;

	private BossSequenceDoor door;

	private Animator animator;

	private Canvas canvas;

	private CanvasGroup group;

	public event HideEvent OnHidden;

	public event BeginEvent OnBegin;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		canvas = GetComponent<Canvas>();
		group = GetComponent<CanvasGroup>();
	}

	private void Start()
	{
		canvas.worldCamera = GameCameras.instance.hudCamera;
		buttons = new BossDoorChallengeUIBindingButton[4];
		buttons[0] = boundNailButton;
		buttons[1] = boundHeartButton;
		buttons[2] = boundCharmsButton;
		buttons[3] = boundSoulButton;
		BossDoorChallengeUIBindingButton[] array = buttons;
		foreach (BossDoorChallengeUIBindingButton obj in array)
		{
			obj.OnButtonSelected += UpdateAllButtons;
			obj.OnButtonCancelled += Hide;
			obj.Reset();
		}
		group.alpha = 0f;
	}

	private void OnEnable()
	{
		if (buttons != null)
		{
			BossDoorChallengeUIBindingButton[] array = buttons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Reset();
			}
			allPreviouslySelected = false;
		}
	}

	public void Setup(BossSequenceDoor door)
	{
		this.door = door;
		if ((bool)titleTextSuper)
		{
			titleTextSuper.text = Language.Get(door.titleSuperKey, door.titleSuperSheet);
		}
		if ((bool)titleTextMain)
		{
			titleTextMain.text = Language.Get(door.titleMainKey, door.titleMainSheet);
		}
		if ((bool)descriptionText)
		{
			descriptionText.text = Language.Get(door.descriptionKey, door.descriptionSheet);
		}
	}

	private void UpdateAllButtons()
	{
		bool flag = true;
		BossDoorChallengeUIBindingButton[] array = buttons;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].Selected)
			{
				flag = false;
				break;
			}
		}
		if (flag || allPreviouslySelected)
		{
			array = buttons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetAllSelected(flag);
			}
		}
		if (flag)
		{
			GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
			allSelectedSound.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
			if ((bool)allSelectedEffect)
			{
				allSelectedEffect.SetActive(value: false);
				allSelectedEffect.SetActive(value: true);
				allSelectedEffect.SetActiveChildren(value: true);
			}
		}
		allPreviouslySelected = flag;
	}

	public void Show()
	{
		base.gameObject.SetActive(value: true);
		StartCoroutine(ShowSequence());
		GameCameras.instance.HUDOut();
	}

	private IEnumerator ShowSequence()
	{
		group.interactable = false;
		EventSystem.current.SetSelectedGameObject(null);
		yield return null;
		if ((bool)animator)
		{
			animator.Play("Open");
			yield return null;
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
		}
		group.interactable = true;
		if (buttons.Length != 0)
		{
			EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
		}
		ManagerSingleton<InputHandler>.Instance.StartUIInput();
	}

	public void Hide()
	{
		StartCoroutine(HideSequence(sendEvent: true));
	}

	private IEnumerator HideSequence(bool sendEvent)
	{
		GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
		if ((bool)currentSelectedGameObject)
		{
			MenuButton component = currentSelectedGameObject.GetComponent<MenuButton>();
			if ((bool)component)
			{
				component.ForceDeselect();
			}
		}
		if ((bool)animator)
		{
			animator.Play("Close");
			yield return null;
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
		}
		if (sendEvent && this.OnHidden != null)
		{
			this.OnHidden();
		}
		GameCameras.instance.HUDIn();
		if ((bool)allSelectedEffect)
		{
			allSelectedEffect.SetActive(value: false);
		}
		base.gameObject.SetActive(value: false);
	}

	public void Begin()
	{
		StartCoroutine(HideSequence(sendEvent: false));
		GameManager.instance.playerData.bossReturnEntryGate = door.dreamReturnGate.name;
		BossSequenceController.ChallengeBindings challengeBindings = BossSequenceController.ChallengeBindings.None;
		if (boundNailButton.Selected)
		{
			challengeBindings |= BossSequenceController.ChallengeBindings.Nail;
		}
		if (boundHeartButton.Selected)
		{
			challengeBindings |= BossSequenceController.ChallengeBindings.Shell;
		}
		if (boundCharmsButton.Selected)
		{
			challengeBindings |= BossSequenceController.ChallengeBindings.Charms;
		}
		if (boundSoulButton.Selected)
		{
			challengeBindings |= BossSequenceController.ChallengeBindings.Soul;
		}
		BossSequenceController.SetupNewSequence(door.bossSequence, challengeBindings, door.playerDataString);
		if (this.OnBegin != null)
		{
			this.OnBegin();
		}
	}
}
