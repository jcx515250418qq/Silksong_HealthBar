using System.Collections;
using UnityEngine;

public class BossSequenceBindingsDoor : MonoBehaviour
{
	public string playerData = "blueRoomDoorUnlocked";

	public GameObject[] bindingIcons;

	public GameObject transitionPointDoor;

	public float doorEnableAnimDelay = 1f;

	public string unlockAnimation = "Unlock";

	public string unlockedAnimation = "Unlocked";

	private bool isUnlocked;

	private bool shouldBeUnlocked;

	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		if (GameManager.instance.GetPlayerDataBool(playerData))
		{
			SetUnlocked(value: true);
			return;
		}
		SetUnlocked(value: false);
		int num = BossSequenceBindingsDisplay.CountCompletedBindings();
		for (int i = 0; i < bindingIcons.Length; i++)
		{
			bindingIcons[i].SetActive(i < num);
		}
		if (num >= bindingIcons.Length)
		{
			shouldBeUnlocked = true;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!isUnlocked && shouldBeUnlocked)
		{
			GameManager.instance.SetPlayerDataBool(playerData, value: true);
			SetUnlocked(value: true, doUnlockAnimation: true);
		}
	}

	private void SetUnlocked(bool value, bool doUnlockAnimation = false)
	{
		isUnlocked = value;
		if (value)
		{
			if (doUnlockAnimation && (bool)animator)
			{
				StartCoroutine(DoUnlockAnimation());
				return;
			}
			animator.Play(unlockedAnimation);
			if ((bool)transitionPointDoor)
			{
				transitionPointDoor.SetActive(value: true);
			}
		}
		else if ((bool)transitionPointDoor)
		{
			transitionPointDoor.SetActive(value: false);
		}
	}

	private IEnumerator DoUnlockAnimation()
	{
		PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(HeroController.instance.gameObject, "Roar and Wound States");
		if ((bool)playMakerFSM)
		{
			playMakerFSM.FsmVariables.FindFsmGameObject("Roar Object").Value = base.gameObject;
		}
		FSMUtility.SendEventToGameObject(HeroController.instance.gameObject, "ROAR ENTER");
		animator.Play(unlockAnimation);
		yield return null;
		yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
		FSMUtility.SendEventToGameObject(HeroController.instance.gameObject, "ROAR EXIT");
		if ((bool)transitionPointDoor)
		{
			transitionPointDoor.SetActive(value: true);
		}
	}
}
