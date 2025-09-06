using System.Collections;
using UnityEngine;

public class BossDoorLockUI : MonoBehaviour
{
	public GameObject iconParent;

	private BossDoorLockUIIcon[] bossIcons;

	public CanvasGroup buttonPrompts;

	public float buttonPromptFadeTime = 2f;

	private Coroutine fadeRoutine;

	private Coroutine fadeButtonRoutine;

	private CanvasGroup group;

	private Animator animator;

	private void Awake()
	{
		group = GetComponent<CanvasGroup>();
		animator = GetComponent<Animator>();
		bossIcons = (iconParent ? iconParent.GetComponentsInChildren<BossDoorLockUIIcon>() : new BossDoorLockUIIcon[0]);
	}

	public void Show(BossSequenceDoor door)
	{
		group.alpha = 0f;
		base.gameObject.SetActive(value: true);
		if ((bool)door && (bool)door.bossSequence)
		{
			BossSequenceDoor.Completion currentCompletion = door.CurrentCompletion;
			BossDoorLockUIIcon[] array = bossIcons;
			foreach (BossDoorLockUIIcon obj in array)
			{
				obj.bossIcon.enabled = false;
				obj.SetUnlocked(unlocked: false);
			}
			int count = door.bossSequence.Count;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (int j = 0; j < count; j++)
			{
				if (door.bossSequence.IsSceneHidden(j))
				{
					num--;
					continue;
				}
				BossScene bossScene = door.bossSequence.GetBossScene(j);
				string sceneObjectName = door.bossSequence.GetSceneObjectName(j);
				int num4 = j + num;
				num3 = num4;
				if (num4 >= bossIcons.Length || !bossIcons[num4])
				{
					continue;
				}
				bossIcons[num4].gameObject.SetActive(value: true);
				if ((bool)bossScene.DisplayIcon)
				{
					bossIcons[num4].bossIcon.enabled = true;
					bossIcons[num4].bossIcon.sprite = bossScene.DisplayIcon;
					bossIcons[num4].bossIcon.SetNativeSize();
				}
				if (bossScene.IsUnlocked(BossSceneCheckSource.Sequence))
				{
					if (currentCompletion.viewedBossSceneCompletions.Contains(sceneObjectName))
					{
						bossIcons[num4].SetUnlocked(unlocked: true);
						continue;
					}
					bossIcons[num4].SetUnlocked(unlocked: true, doUnlockAnim: true, num2);
					num2++;
					currentCompletion.viewedBossSceneCompletions.Add(sceneObjectName);
				}
			}
			for (int k = num3 + 1; k < bossIcons.Length; k++)
			{
				bossIcons[k].gameObject.SetActive(value: false);
			}
			door.CurrentCompletion = currentCompletion;
		}
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}
		fadeRoutine = StartCoroutine(ShowRoutine());
		GameCameras.instance.HUDOut();
	}

	public void Hide()
	{
		BossDoorLockUIIcon[] array = bossIcons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].StopAllCoroutines();
		}
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}
		fadeRoutine = StartCoroutine(HideRoutine());
	}

	private IEnumerator ShowRoutine()
	{
		if ((bool)buttonPrompts)
		{
			buttonPrompts.alpha = 0f;
		}
		if ((bool)animator)
		{
			animator.Play("Open");
			yield return null;
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
		}
		group.alpha = 1f;
		if (fadeButtonRoutine != null)
		{
			StopCoroutine(fadeButtonRoutine);
		}
		fadeButtonRoutine = StartCoroutine(FadeButtonPrompts(1f, buttonPromptFadeTime));
		yield return fadeButtonRoutine;
		fadeRoutine = null;
	}

	private IEnumerator HideRoutine()
	{
		if ((bool)animator)
		{
			animator.Play("Close");
			yield return null;
			float length = animator.GetCurrentAnimatorStateInfo(0).length;
			if (fadeButtonRoutine != null)
			{
				StopCoroutine(fadeButtonRoutine);
			}
			fadeButtonRoutine = StartCoroutine(FadeButtonPrompts(0f, length));
			yield return new WaitForSeconds(length);
		}
		GameCameras.instance.HUDIn();
		base.gameObject.SetActive(value: false);
		fadeRoutine = null;
	}

	private IEnumerator FadeButtonPrompts(float toAlpha, float time)
	{
		if ((bool)buttonPrompts)
		{
			float startAlpha = buttonPrompts.alpha;
			for (float elapsed = 0f; elapsed < time; elapsed += Time.deltaTime)
			{
				buttonPrompts.alpha = Mathf.Lerp(startAlpha, toAlpha, elapsed / time);
				yield return null;
			}
			buttonPrompts.alpha = toAlpha;
		}
		fadeButtonRoutine = null;
	}
}
