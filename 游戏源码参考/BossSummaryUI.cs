using System.Collections;
using System.Collections.Generic;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;

public class BossSummaryUI : MonoBehaviour
{
	public GameObject listItemTemplate;

	public Sprite[] stateSprites;

	public Sprite[] noTierStateSprites;

	public string defaultName = ".....";

	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		CanvasGroup component = GetComponent<CanvasGroup>();
		if ((bool)component)
		{
			component.alpha = 0f;
		}
	}

	public void SetupUI(List<BossStatue> bossStatues)
	{
		listItemTemplate.SetActive(value: true);
		foreach (BossStatue bossStatue in bossStatues)
		{
			if (bossStatue.gameObject.activeInHierarchy)
			{
				CreateListItem(bossStatue);
				if ((bool)bossStatue && (bool)bossStatue.dreamBossScene)
				{
					CreateListItem(bossStatue, isAlt: true);
				}
			}
		}
		listItemTemplate.SetActive(value: false);
	}

	private void CreateListItem(BossStatue bossStatue, bool isAlt = false)
	{
		BossStatue.Completion completion = ((!(bossStatue != null)) ? BossStatue.Completion.None : (isAlt ? bossStatue.DreamStatueState : bossStatue.StatueState));
		if ((bool)listItemTemplate)
		{
			if (bossStatue.isHidden && !completion.completedTier1 && !completion.completedTier2 && !completion.completedTier3)
			{
				return;
			}
			GameObject obj = Object.Instantiate(listItemTemplate, listItemTemplate.transform.parent);
			obj.name = string.Format("{0} ({1})", listItemTemplate.name, bossStatue ? bossStatue.gameObject.name : "null");
			int num = 0;
			if (completion.completedTier3)
			{
				num = 4;
			}
			else if (completion.completedTier2)
			{
				num = 3;
			}
			else if (completion.completedTier1)
			{
				num = 2;
			}
			else if (completion.isUnlocked)
			{
				num = 1;
			}
			Image componentInChildren = obj.GetComponentInChildren<Image>();
			if ((bool)componentInChildren)
			{
				if (bossStatue.hasNoTiers)
				{
					num = Mathf.Clamp(num, 0, 3);
					componentInChildren.sprite = noTierStateSprites[num];
					componentInChildren.SetNativeSize();
				}
				else if (num < stateSprites.Length && num >= 0)
				{
					componentInChildren.sprite = stateSprites[num];
					componentInChildren.SetNativeSize();
				}
			}
			Text componentInChildren2 = obj.GetComponentInChildren<Text>();
			if ((bool)componentInChildren2)
			{
				if (num > 0 && (bool)bossStatue)
				{
					componentInChildren2.text = Language.Get(isAlt ? bossStatue.dreamBossDetails.nameKey : bossStatue.bossDetails.nameKey, isAlt ? bossStatue.dreamBossDetails.nameSheet : bossStatue.bossDetails.nameSheet).GetProcessed(LocalisationHelper.FontSource.Trajan).ToUpper();
				}
				else
				{
					componentInChildren2.text = defaultName;
				}
			}
		}
		else
		{
			Debug.LogError("No List Item template assigned!", this);
		}
	}

	public void Show()
	{
		base.gameObject.SetActive(value: true);
		GameCameras.instance.HUDOut();
	}

	public void Hide()
	{
		StartCoroutine(Close());
	}

	private IEnumerator Close()
	{
		if ((bool)animator)
		{
			animator.Play("Close");
			yield return null;
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
		}
		GameCameras.instance.HUDIn();
		base.gameObject.SetActive(value: false);
	}
}
