using System.Collections;
using UnityEngine;

public class TrapBridgeFlip : TrapBridge
{
	[Header("Flipper")]
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private GameObject closedColliders;

	[SerializeField]
	private GameObject openedColliders;

	[SerializeField]
	private float originXOffset;

	[Space]
	[SerializeField]
	private GameObject[] activateOnOpened;

	[SerializeField]
	private GameObject[] deActivateOnOpened;

	private bool isWaitingForOpen;

	private static readonly int ClosedAnim = Animator.StringToHash("Closed");

	private static readonly int OpenUpFirstAnim = Animator.StringToHash("Open Up First");

	private static readonly int OpenDownFirstAnim = Animator.StringToHash("Open Down First");

	private static readonly int CloseUpFirstAnim = Animator.StringToHash("Close Up First");

	private static readonly int CloseDownFirstAnim = Animator.StringToHash("Close Down First");

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(new Vector3(originXOffset, 0f, 0f), 0.1f);
	}

	private void OnEnable()
	{
		SetCollidersOpened(isOpened: false);
	}

	private void PlayAnim(int animID)
	{
		if ((bool)animator)
		{
			animator.Play(animID);
		}
	}

	public void ReportOpened()
	{
		isWaitingForOpen = false;
	}

	private void SetCollidersOpened(bool isOpened)
	{
		if ((bool)closedColliders)
		{
			closedColliders.SetActive(!isOpened);
		}
		if ((bool)openedColliders)
		{
			openedColliders.SetActive(isOpened);
		}
		activateOnOpened.SetAllActive(isOpened);
		deActivateOnOpened.SetAllActive(!isOpened);
	}

	private int GetOpenAnimID()
	{
		if (IsHeroOnLeft())
		{
			return OpenUpFirstAnim;
		}
		return OpenDownFirstAnim;
	}

	private int GetCloseAnimID()
	{
		if (IsHeroOnLeft())
		{
			return CloseDownFirstAnim;
		}
		return CloseUpFirstAnim;
	}

	private bool IsHeroOnLeft()
	{
		float x = HeroController.instance.transform.position.x;
		float x2 = base.transform.TransformPoint(new Vector3(originXOffset, 0f, 0f)).x;
		bool flag = x < x2;
		if (base.transform.lossyScale.x < 0f)
		{
			flag = !flag;
		}
		return flag;
	}

	protected override IEnumerator DoOpenAnim()
	{
		isWaitingForOpen = true;
		PlayAnim(GetOpenAnimID());
		while (isWaitingForOpen)
		{
			yield return null;
		}
		SetCollidersOpened(isOpened: true);
	}

	protected override IEnumerator DoCloseAnim()
	{
		SetCollidersOpened(isOpened: false);
		PlayAnim(GetCloseAnimID());
		yield return null;
	}
}
