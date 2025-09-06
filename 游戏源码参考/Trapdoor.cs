using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Trapdoor : MonoBehaviour
{
	[Serializable]
	private class DirectionalAnims
	{
		public string OpeningAnim;

		public string OpenedAnim;

		public string ClosingAnim;

		public string ClosedAnim;

		public int? OpeningAnimHash { get; private set; }

		public int? OpenedAnimHash { get; private set; }

		public int? ClosingAnimHash { get; private set; }

		public int? ClosedAnimHash { get; private set; }

		public void UpdateAnimHashes()
		{
			OpeningAnimHash = (string.IsNullOrEmpty(OpeningAnim) ? ((int?)null) : new int?(Animator.StringToHash(OpeningAnim)));
			OpenedAnimHash = (string.IsNullOrEmpty(OpenedAnim) ? ((int?)null) : new int?(Animator.StringToHash(OpenedAnim)));
			ClosingAnimHash = (string.IsNullOrEmpty(ClosingAnim) ? ((int?)null) : new int?(Animator.StringToHash(ClosingAnim)));
			ClosedAnimHash = (string.IsNullOrEmpty(ClosedAnim) ? ((int?)null) : new int?(Animator.StringToHash(ClosedAnim)));
		}
	}

	[Serializable]
	private class DirectionalLeverParts
	{
		public Lever LeverControl;

		public GameObject Lever;

		public Animator LeverRetract;

		public List<string> RetractPreventStates;
	}

	private static readonly int _retractAnim = Animator.StringToHash("Retract");

	private static readonly int _returnAnim = Animator.StringToHash("Return");

	private static readonly int _hiddenAnim = Animator.StringToHash("Hidden");

	[SerializeField]
	private Animator doorAnimator;

	[SerializeField]
	private Animator mechanismAnimator;

	[SerializeField]
	private bool isDirectional;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("isDirectional", true, false, false)]
	private Transform doorRoot;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("isDirectional", true, false, false)]
	private bool defaultDoorScalePositive;

	[SerializeField]
	private TrackTriggerObjects cannotCloseTrigger;

	[SerializeField]
	private float openWaitTime;

	[SerializeField]
	private float closeWaitTime;

	[SerializeField]
	private PersistentBoolItem persistent;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("persistent", false, false, false)]
	private bool stayOpen;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("persistent", false, false, false)]
	private TrackTriggerObjects enterSceneTrigger;

	[SerializeField]
	private float startOpenSign;

	[SerializeField]
	private float openForceDirection;

	[SerializeField]
	private AudioEvent openSound;

	[SerializeField]
	private AudioEvent closeSound;

	[Space]
	[SerializeField]
	private DirectionalAnims positiveAnims;

	[SerializeField]
	private DirectionalAnims negativeAnims;

	[Space]
	[SerializeField]
	private DirectionalLeverParts[] retractLevers;

	[SerializeField]
	private float retractStartDelay;

	[SerializeField]
	private float retractEndDelay;

	[Space]
	public UnityEvent OnOpen;

	public UnityEvent OnClose;

	public UnityEvent OnClosed;

	public UnityEvent OnStartOpen;

	private bool isOpen;

	private Coroutine openDoorRoutine;

	private bool resetCloseCounter;

	private bool isCustomOpened;

	public bool IsOpen => isOpen;

	private void Awake()
	{
		if ((bool)persistent)
		{
			persistent.OnGetSaveState += delegate(out bool value)
			{
				value = isOpen;
			};
			persistent.OnSetSaveState += delegate(bool value)
			{
				isOpen = value;
				if (value)
				{
					SetInitialStateOpen();
				}
			};
		}
		else if ((bool)enterSceneTrigger)
		{
			HeroController hc = HeroController.instance;
			HeroController.HeroInPosition enterSceneSetState = null;
			enterSceneSetState = delegate
			{
				if (enterSceneTrigger.IsInside)
				{
					SetInitialStateOpen();
				}
				hc.heroInPositionDelayed -= enterSceneSetState;
			};
			if (hc.isHeroInPosition)
			{
				enterSceneSetState(forceDirect: false);
			}
			else
			{
				hc.heroInPositionDelayed += enterSceneSetState;
			}
		}
		DirectionalLeverParts[] array = retractLevers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].LeverControl.OnHit.AddListener(ResetLeverState);
		}
		Action value2 = delegate
		{
			DirectionalLeverParts[] array2 = retractLevers;
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].LeverControl.HitBlocked = false;
			}
		};
		array = retractLevers;
		foreach (DirectionalLeverParts directionalLeverParts in array)
		{
			if ((bool)directionalLeverParts.LeverRetract)
			{
				CaptureAnimationEvent component = directionalLeverParts.LeverRetract.GetComponent<CaptureAnimationEvent>();
				if ((bool)component)
				{
					component.EventFired += value2;
				}
			}
		}
		positiveAnims.UpdateAnimHashes();
		negativeAnims.UpdateAnimHashes();
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		openDoorRoutine = null;
	}

	private void OnEnable()
	{
		ResetLeverState();
	}

	private void SetInitialStateOpen()
	{
		if (!(Math.Abs(startOpenSign) <= Mathf.Epsilon))
		{
			if (openDoorRoutine != null)
			{
				StopCoroutine(openDoorRoutine);
			}
			if ((bool)doorAnimator)
			{
				openDoorRoutine = StartCoroutine(DoOpenDoor(startOpenSign, skipOpen: true));
			}
		}
	}

	public void SetOpened()
	{
		if (openDoorRoutine != null)
		{
			StopCoroutine(openDoorRoutine);
		}
		if ((bool)doorAnimator)
		{
			openDoorRoutine = StartCoroutine(DoOpenDoor(startOpenSign, skipOpen: true));
		}
	}

	private void AttemptResetDoor()
	{
		if (!isOpen && openDoorRoutine != null)
		{
			StopCoroutine(openDoorRoutine);
			openDoorRoutine = null;
		}
	}

	[ContextMenu("Open Positive", true)]
	[ContextMenu("Open Negative", true)]
	private bool CanTestOpen()
	{
		return Application.isPlaying;
	}

	[ContextMenu("Open Positive")]
	public void OpenDoorPositive()
	{
		OpenDoor(1);
	}

	[ContextMenu("Open Negative")]
	public void OpenDoorNegative()
	{
		OpenDoor(-1);
	}

	public void OpenDoor(int direction)
	{
		InternalOpenDoor(direction, skip: false);
	}

	private void InternalOpenDoor(int direction, bool skip)
	{
		AttemptResetDoor();
		if (openDoorRoutine != null || !doorAnimator)
		{
			resetCloseCounter = true;
			return;
		}
		float doorSign = ((!(Math.Abs(openForceDirection) > Mathf.Epsilon)) ? ((float)direction) : openForceDirection);
		openDoorRoutine = StartCoroutine(DoOpenDoor(doorSign, skip));
	}

	public void OpenDoorCustom(int direction)
	{
		isCustomOpened = true;
		OpenDoor(direction);
	}

	public void OpenDoorCustomSilent(int direction)
	{
		isCustomOpened = true;
		InternalOpenDoor(direction, skip: true);
	}

	public void CloseDoorCustom()
	{
		isCustomOpened = false;
	}

	private IEnumerator DoOpenDoor(float doorSign, bool skipOpen = false)
	{
		isOpen = true;
		bool doAutoClose = !isCustomOpened;
		DirectionalLeverParts[] array = retractLevers;
		foreach (DirectionalLeverParts directionalLeverParts in array)
		{
			directionalLeverParts.LeverControl.HitBlocked = true;
			if (!directionalLeverParts.LeverControl || directionalLeverParts.LeverControl.gameObject != directionalLeverParts.Lever)
			{
				directionalLeverParts.Lever.SetActive(value: false);
			}
		}
		if (!skipOpen && openWaitTime > 0f)
		{
			yield return new WaitForSeconds(openWaitTime);
		}
		if ((bool)mechanismAnimator)
		{
			mechanismAnimator.Play("Open");
		}
		if (!skipOpen)
		{
			if (retractStartDelay > 0f)
			{
				yield return new WaitForSeconds(retractStartDelay);
			}
			array = retractLevers;
			foreach (DirectionalLeverParts directionalLeverParts2 in array)
			{
				if (!directionalLeverParts2.LeverRetract)
				{
					continue;
				}
				if (directionalLeverParts2.RetractPreventStates != null)
				{
					int currentState = directionalLeverParts2.LeverRetract.GetCurrentAnimatorStateInfo(0).shortNameHash;
					if (directionalLeverParts2.RetractPreventStates.Any((string stateName) => currentState == Animator.StringToHash(stateName)))
					{
						continue;
					}
				}
				if (directionalLeverParts2.LeverRetract.HasState(0, _retractAnim))
				{
					directionalLeverParts2.LeverRetract.Play(_retractAnim);
				}
			}
			if (retractEndDelay > 0f)
			{
				yield return new WaitForSeconds(retractEndDelay);
			}
		}
		else
		{
			array = retractLevers;
			foreach (DirectionalLeverParts directionalLeverParts3 in array)
			{
				if ((bool)directionalLeverParts3.LeverRetract && directionalLeverParts3.LeverRetract.HasState(0, _retractAnim))
				{
					directionalLeverParts3.LeverRetract.Play(_retractAnim, 0, 1f);
				}
			}
			OnStartOpen.Invoke();
		}
		DirectionalAnims anims;
		if (doorSign < 0f)
		{
			doorSign = -1f;
			anims = negativeAnims;
		}
		else
		{
			doorSign = 1f;
			anims = positiveAnims;
		}
		if (isDirectional)
		{
			Vector3 localScale = doorRoot.localScale;
			localScale.x = Mathf.Abs(localScale.x) * doorSign * (float)(defaultDoorScalePositive ? 1 : (-1));
			doorRoot.localScale = localScale;
		}
		if (OnOpen != null)
		{
			OnOpen.Invoke();
		}
		if (!skipOpen)
		{
			openSound.SpawnAndPlayOneShot(base.transform.position);
			if (anims.OpeningAnimHash.HasValue)
			{
				doorAnimator.Play(anims.OpeningAnimHash.Value, 0, 0f);
				yield return null;
				yield return new WaitForSeconds(doorAnimator.GetCurrentAnimatorStateInfo(0).length);
			}
		}
		if (anims.OpenedAnimHash.HasValue)
		{
			doorAnimator.Play(anims.OpenedAnimHash.Value);
		}
		if (stayOpen || (bool)persistent)
		{
			yield break;
		}
		if (doAutoClose)
		{
			float openTimeLeft = closeWaitTime;
			while (openTimeLeft > 0f)
			{
				yield return null;
				if (resetCloseCounter || ((bool)cannotCloseTrigger && cannotCloseTrigger.IsInside))
				{
					openTimeLeft = closeWaitTime;
					resetCloseCounter = false;
				}
				else
				{
					openTimeLeft -= Time.deltaTime;
				}
			}
		}
		else
		{
			while (isCustomOpened)
			{
				yield return null;
			}
		}
		if (OnClose != null)
		{
			OnClose.Invoke();
		}
		closeSound.SpawnAndPlayOneShot(base.transform.position);
		if ((bool)mechanismAnimator)
		{
			mechanismAnimator.Play("Close");
		}
		if (anims.ClosingAnimHash.HasValue)
		{
			doorAnimator.Play(anims.ClosingAnimHash.Value, 0, 0f);
			yield return null;
			yield return new WaitForSeconds(doorAnimator.GetCurrentAnimatorStateInfo(0).length);
		}
		if (anims.ClosedAnimHash.HasValue)
		{
			doorAnimator.Play(anims.ClosedAnimHash.Value);
		}
		if (OnClosed != null)
		{
			OnClosed.Invoke();
		}
		isOpen = false;
		yield return StartCoroutine(ReturnRetractedLevers());
		openDoorRoutine = null;
	}

	private IEnumerator ReturnRetractedLevers()
	{
		float returnTime = 0f;
		DirectionalLeverParts[] array = retractLevers;
		foreach (DirectionalLeverParts directionalLeverParts in array)
		{
			if ((bool)directionalLeverParts.LeverRetract && directionalLeverParts.LeverRetract.HasState(0, _returnAnim))
			{
				directionalLeverParts.LeverRetract.Play(_returnAnim);
			}
		}
		yield return null;
		array = retractLevers;
		foreach (DirectionalLeverParts directionalLeverParts2 in array)
		{
			if ((bool)directionalLeverParts2.LeverRetract && directionalLeverParts2.LeverRetract.isActiveAndEnabled)
			{
				AnimatorStateInfo currentAnimatorStateInfo = directionalLeverParts2.LeverRetract.GetCurrentAnimatorStateInfo(0);
				if (currentAnimatorStateInfo.shortNameHash == _returnAnim)
				{
					float length = currentAnimatorStateInfo.length;
					returnTime = Mathf.Max(returnTime, length);
				}
			}
		}
		if (returnTime > 0f)
		{
			yield return new WaitForSeconds(returnTime);
		}
		ResetLeverState();
	}

	private void ResetLeverState()
	{
		SetLeversRetracted(isRetracted: false, isInstant: true);
	}

	public void SetLeversRetracted(bool isRetracted, bool isInstant)
	{
		if (!isRetracted && !isInstant)
		{
			StartCoroutine(ReturnRetractedLevers());
			return;
		}
		DirectionalLeverParts[] array = retractLevers;
		foreach (DirectionalLeverParts directionalLeverParts in array)
		{
			directionalLeverParts.LeverControl.HitBlocked = isRetracted;
			if (!directionalLeverParts.LeverControl || directionalLeverParts.LeverControl.gameObject != directionalLeverParts.Lever)
			{
				directionalLeverParts.Lever.gameObject.SetActive(!isRetracted);
			}
			int num = (isRetracted ? _retractAnim : _hiddenAnim);
			if (directionalLeverParts.LeverRetract.HasState(0, num))
			{
				directionalLeverParts.LeverRetract.Play(num, 0, isInstant ? 1f : 0f);
			}
		}
	}
}
