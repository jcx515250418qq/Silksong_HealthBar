using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossDoorCage : MonoBehaviour
{
	public BossSequenceDoor[] requiredComplete;

	public TriggerEnterEvent unlockTrigger;

	public string playerData = "bossDoorCageUnlocked";

	private Animator animator;

	private CameraControlAnimationEvents cameraShake;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		cameraShake = GetComponent<CameraControlAnimationEvents>();
	}

	private void Start()
	{
		if (GameManager.instance.GetPlayerDataBool(playerData))
		{
			base.gameObject.SetActive(value: false);
		}
		else if ((bool)unlockTrigger)
		{
			unlockTrigger.OnTriggerEntered += delegate
			{
				Unlock();
			};
		}
	}

	private void Unlock()
	{
		if (GameManager.instance.GetPlayerDataBool(playerData))
		{
			return;
		}
		BossSequenceDoor[] array = requiredComplete;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].CurrentCompletion.completed)
			{
				return;
			}
		}
		GameManager.instance.SetPlayerDataBool(playerData, value: true);
		StartCoroutine(UnlockRoutine());
	}

	private IEnumerator UnlockRoutine()
	{
		animator.Play("Unlock");
		yield return null;
		yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
	}

	public void StartShakeLock()
	{
		if ((bool)cameraShake)
		{
			cameraShake.SmallRumble();
		}
		PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(HeroController.instance.gameObject, "Roar and Wound States");
		if ((bool)playMakerFSM)
		{
			playMakerFSM.FsmVariables.FindFsmGameObject("Roar Object").Value = base.gameObject;
		}
		FSMUtility.SendEventToGameObject(HeroController.instance.gameObject, "ROAR ENTER");
	}

	public void StopShakeLock()
	{
		if ((bool)cameraShake)
		{
			cameraShake.StopRumble();
		}
		FSMUtility.SendEventToGameObject(HeroController.instance.gameObject, "ROAR EXIT");
	}
}
