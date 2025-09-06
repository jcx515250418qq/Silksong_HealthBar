using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilkRationMachine : MonoBehaviour
{
	private static readonly int _dropAnimHash = Animator.StringToHash("Drop");

	[SerializeField]
	private PersistentIntItem purchaseTracker;

	[SerializeField]
	private List<Animator> rationObjects;

	[SerializeField]
	private int startCount = -1;

	[SerializeField]
	private GameObject rationPrefab;

	[Header("Jamming")]
	[SerializeField]
	private bool canJam;

	[SerializeField]
	[Range(0f, 1f)]
	private float chanceToJam;

	[SerializeField]
	private int minPurchaseToJam;

	[Space]
	[SerializeField]
	private GameObject jammedBreakable;

	private bool isJammed;

	private int purchasedRationsCount;

	public event Action RationDropped;

	private void OnValidate()
	{
		if (startCount > rationObjects.Count)
		{
			startCount = rationObjects.Count;
		}
		else if (startCount < -1)
		{
			startCount = -1;
		}
	}

	private void Awake()
	{
		OnValidate();
		for (int num = rationObjects.Count - 1; num >= 0; num--)
		{
			if (!rationObjects[num].gameObject.activeInHierarchy)
			{
				rationObjects.RemoveAt(num);
			}
		}
		if (startCount < 0)
		{
			startCount = rationObjects.Count;
		}
		for (int i = 0; i < rationObjects.Count; i++)
		{
			Animator animator = rationObjects[i];
			if (i - startCount >= 0)
			{
				animator.gameObject.SetActive(value: false);
			}
			else
			{
				animator.enabled = false;
			}
		}
		if ((bool)purchaseTracker)
		{
			purchaseTracker.OnGetSaveState += delegate(out int value)
			{
				value = purchasedRationsCount;
			};
			purchaseTracker.OnSetSaveState += delegate(int value)
			{
				purchasedRationsCount = value;
				int num2 = Mathf.Max(0, startCount - value);
				for (int num3 = startCount - 1; num3 >= num2; num3--)
				{
					rationObjects[num3].gameObject.SetActive(value: false);
				}
			};
		}
		if ((bool)jammedBreakable)
		{
			jammedBreakable.SetActive(value: false);
		}
	}

	private void Start()
	{
		if ((bool)rationPrefab)
		{
			int remainingRationCount = GetRemainingRationCount();
			if (remainingRationCount > 0)
			{
				PersonalObjectPool.EnsurePooledInScene(base.gameObject, rationPrefab, remainingRationCount);
			}
		}
	}

	public int GetRemainingRationCount()
	{
		return startCount - purchasedRationsCount;
	}

	public bool IsAnyRationsLeft()
	{
		if (!isJammed)
		{
			return GetRemainingRationCount() > 0;
		}
		return false;
	}

	public bool IsJammed()
	{
		return isJammed;
	}

	public bool TryDropRation()
	{
		if (!IsAnyRationsLeft())
		{
			Debug.LogError("No silk rations left!", this);
			return false;
		}
		int rationIndex = startCount - purchasedRationsCount - 1;
		StartCoroutine(DropRationRoutine(rationIndex));
		if (canJam && purchasedRationsCount >= minPurchaseToJam && UnityEngine.Random.Range(0f, 1f) <= chanceToJam)
		{
			isJammed = true;
			if ((bool)jammedBreakable)
			{
				jammedBreakable.SetActive(value: true);
			}
			return false;
		}
		purchasedRationsCount++;
		return true;
	}

	private IEnumerator DropRationRoutine(int rationIndex)
	{
		Animator ration = rationObjects[rationIndex];
		ration.enabled = true;
		ration.Play(_dropAnimHash);
		yield return null;
		yield return new WaitForSeconds(ration.GetCurrentAnimatorStateInfo(0).length);
		ration.gameObject.SetActive(value: false);
		if (this.RationDropped != null)
		{
			this.RationDropped();
		}
	}

	public void FlingRations()
	{
	}
}
