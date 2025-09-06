using System.Collections;
using HutongGames.PlayMaker;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class CogCylinderPuzzle : MonoBehaviour
{
	[SerializeField]
	private PersistentBoolItem persistent;

	[SerializeField]
	private Animator stateAnimator;

	[SerializeField]
	private float completionDelay;

	[Space]
	[SerializeField]
	private PlayMakerFSM notifyCompleted;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmBool")]
	private string fsmBoolName;

	[Space]
	[SerializeField]
	private GameObject scaffoldActive;

	[SerializeField]
	private GameObject scaffoldBreak;

	[SerializeField]
	private AudioEventRandom scaffoldBreakSound;

	[SerializeField]
	private CameraShakeTarget scaffoldBreakShake;

	[Space]
	[SerializeField]
	private GameObject choirParent;

	public UnityEvent OnChoirStart;

	private bool isComplete;

	private CogCylinderPuzzleCog[] readCogs;

	[UsedImplicitly]
	private bool? ValidateFsmBool(string boolName)
	{
		if (!notifyCompleted || string.IsNullOrEmpty(boolName))
		{
			return null;
		}
		return notifyCompleted.FsmVariables.FindFsmBool(boolName) != null;
	}

	private void Awake()
	{
		readCogs = GetComponentsInChildren<CogCylinderPuzzleCog>();
		CogCylinderPuzzleCog[] array = readCogs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].RotateFinished += CheckComplete;
		}
		if ((bool)persistent)
		{
			persistent.OnGetSaveState += OnGetSaveState;
			persistent.OnSetSaveState += OnSetSaveState;
		}
		if ((bool)scaffoldActive)
		{
			scaffoldActive.SetActive(value: true);
		}
		if ((bool)scaffoldBreak)
		{
			scaffoldBreak.SetActive(value: false);
		}
	}

	private void OnGetSaveState(out bool value)
	{
		value = isComplete;
	}

	private void OnSetSaveState(bool value)
	{
		if (value)
		{
			InternalSetComplete();
		}
	}

	private void Start()
	{
		if (!isComplete)
		{
			stateAnimator.Play("Drop", 0, 0f);
			stateAnimator.enabled = false;
			stateAnimator.Update(0f);
		}
	}

	private void CheckComplete()
	{
		if (isComplete)
		{
			return;
		}
		bool flag = true;
		CogCylinderPuzzleCog[] array = readCogs;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].IsInTargetPos)
			{
				flag = false;
				break;
			}
		}
		isComplete = flag;
		if (isComplete)
		{
			StartCoroutine(Complete());
		}
	}

	private IEnumerator Complete()
	{
		CogCylinderPuzzleCog[] array = readCogs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetComplete();
		}
		yield return new WaitForSeconds(completionDelay);
		if ((bool)scaffoldActive)
		{
			scaffoldActive.SetActive(value: false);
		}
		if ((bool)scaffoldBreak)
		{
			scaffoldBreak.SetActive(value: true);
		}
		scaffoldBreakShake.DoShake(this);
		scaffoldBreakSound.SpawnAndPlayOneShot(base.transform.position);
		stateAnimator.enabled = true;
		stateAnimator.Play("Drop", 0, 0f);
		yield return null;
		yield return new WaitForSeconds(stateAnimator.GetCurrentAnimatorStateInfo(0).length);
		NotifyComplete(isInstant: false);
	}

	private void NotifyComplete(bool isInstant)
	{
		if (!string.IsNullOrEmpty(fsmBoolName))
		{
			FsmBool fsmBool = notifyCompleted.FsmVariables.FindFsmBool(fsmBoolName);
			if (fsmBool != null)
			{
				fsmBool.Value = true;
			}
		}
		notifyCompleted.SendEvent(isInstant ? "CYLINDER ALREADY COMPLETE" : "CYLINDER COMPLETED");
	}

	public void StartChoir()
	{
		if ((bool)choirParent)
		{
			choirParent.BroadcastMessage("StartJitter");
		}
		OnChoirStart.Invoke();
	}

	public void StopChoir()
	{
		if ((bool)choirParent)
		{
			choirParent.BroadcastMessage("StopJitter");
		}
	}

	public void SetComplete()
	{
		if ((bool)persistent)
		{
			persistent.OnGetSaveState -= OnGetSaveState;
			persistent.OnSetSaveState -= OnSetSaveState;
		}
		InternalSetComplete();
	}

	private void InternalSetComplete()
	{
		isComplete = true;
		CogCylinderPuzzleCog[] array = readCogs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetComplete();
		}
		stateAnimator.enabled = true;
		stateAnimator.Play("Drop", 0, 1f);
		NotifyComplete(isInstant: true);
		if ((bool)scaffoldActive)
		{
			scaffoldActive.SetActive(value: false);
		}
	}
}
