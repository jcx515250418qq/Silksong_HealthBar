using System;
using System.Collections;
using GlobalSettings;
using HutongGames.PlayMaker;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class InventoryPaneStandalone : InventoryPaneBase
{
	[SerializeField]
	private string openAnim = "Open";

	[SerializeField]
	private string closeAnim = "Close";

	[SerializeField]
	private Animator paneAnimator;

	[Space]
	[SerializeField]
	private AudioEvent openSound;

	[SerializeField]
	private AudioEvent closeSound;

	private Action onOpenAnimEnd;

	private Coroutine openAnimRoutine;

	private Action onCloseAnimEnd;

	private Coroutine closeAnimRoutine;

	private InventoryPaneInput paneInput;

	private bool hasStarted;

	public bool SkipInputEnable { get; set; }

	public event Action PaneClosedAnimEnd;

	public event Action PaneOpenedAnimEnd;

	private void Awake()
	{
		if (!paneAnimator)
		{
			paneAnimator = GetComponent<Animator>();
		}
		paneInput = GetComponent<InventoryPaneInput>();
	}

	private void OnEnable()
	{
		NestedFadeGroupBase component = GetComponent<NestedFadeGroupBase>();
		if ((bool)component)
		{
			component.AlphaSelf = 0f;
		}
	}

	private void Start()
	{
		if ((bool)paneAnimator)
		{
			paneAnimator.Update(0f);
		}
		base.gameObject.SetActive(value: false);
	}

	[ContextMenu("Pane Start", true)]
	[ContextMenu("Pane End", true)]
	private bool IsPlayMode()
	{
		return Application.isPlaying;
	}

	[ContextMenu("Pane Start")]
	public override void PaneStart()
	{
		StopCloseAnim();
		base.gameObject.SetActive(value: true);
		base.PaneStart();
		PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(base.gameObject, "Inventory Proxy");
		if ((bool)playMakerFSM)
		{
			FsmBool fsmBool = playMakerFSM.FsmVariables.FindFsmBool("Only Pane");
			if (fsmBool != null)
			{
				fsmBool.Value = true;
			}
		}
		FSMUtility.SendEventToGameObject(base.gameObject, "UI ACTIVE");
		FSMUtility.SendEventToGameObject(base.gameObject, "ACTIVATE");
		if (!SkipInputEnable && (bool)paneInput)
		{
			paneInput.enabled = true;
		}
		if ((bool)paneAnimator)
		{
			paneAnimator.Update(0f);
		}
		StopOpenAnim();
		onOpenAnimEnd = delegate
		{
			if (this.PaneOpenedAnimEnd != null)
			{
				this.PaneOpenedAnimEnd();
			}
		};
		openAnimRoutine = StartCoroutine(OpenAnimation());
	}

	[ContextMenu("Pane End")]
	public override void PaneEnd()
	{
		StopCloseAnim();
		if ((bool)paneInput)
		{
			paneInput.enabled = false;
		}
		onCloseAnimEnd = delegate
		{
			base.PaneEnd();
			FSMUtility.SendEventToGameObject(base.gameObject, "UI INACTIVE");
			base.gameObject.SetActive(value: false);
			if (this.PaneClosedAnimEnd != null)
			{
				this.PaneClosedAnimEnd();
			}
		};
		closeAnimRoutine = StartCoroutine(CloseAnimation());
	}

	private void StopOpenAnim()
	{
		if (openAnimRoutine != null)
		{
			StopCoroutine(openAnimRoutine);
			if (onOpenAnimEnd != null)
			{
				onOpenAnimEnd();
			}
		}
	}

	private IEnumerator OpenAnimation()
	{
		openSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
		if ((bool)paneAnimator)
		{
			paneAnimator.Play(openAnim);
			yield return null;
			yield return new WaitForSeconds(paneAnimator.GetCurrentAnimatorStateInfo(0).length);
		}
		closeAnimRoutine = null;
		onOpenAnimEnd();
		onOpenAnimEnd = null;
	}

	private void StopCloseAnim()
	{
		if (closeAnimRoutine != null)
		{
			StopCoroutine(closeAnimRoutine);
			closeAnimRoutine = null;
			onCloseAnimEnd?.Invoke();
		}
	}

	private IEnumerator CloseAnimation()
	{
		closeSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
		if ((bool)paneAnimator)
		{
			paneAnimator.Play(closeAnim);
			yield return null;
			yield return new WaitForSeconds(paneAnimator.GetCurrentAnimatorStateInfo(0).length);
		}
		closeAnimRoutine = null;
		onCloseAnimEnd();
		onCloseAnimEnd = null;
	}
}
