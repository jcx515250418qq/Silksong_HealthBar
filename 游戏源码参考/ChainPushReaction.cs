using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChainPushReaction : MonoBehaviour
{
	[SerializeField]
	private float touchTime = 0.25f;

	[SerializeField]
	private float playerRangeEnable = 1f;

	[SerializeField]
	private float playerRangeEnableDelay = 0.1f;

	[Space]
	public UnityEvent OnTouchedLink;

	private List<Transform> rangeEnableTracked = new List<Transform>();

	private ChainLinkInteraction[] links;

	private Transform lowestLink;

	protected PlayChainSound Sound { get; private set; }

	public bool IsPushDisableStarted { get; private set; }

	public event Action<Vector3> OnTouched;

	private void OnDrawGizmosSelected()
	{
		if ((bool)lowestLink)
		{
			Vector3 position = lowestLink.position;
			Gizmos.color = Color.green;
			Gizmos.DrawLine(position + Vector3.right * playerRangeEnable, position + Vector3.right * playerRangeEnable + Vector3.down);
			Gizmos.DrawLine(position - Vector3.right * playerRangeEnable, position - Vector3.right * playerRangeEnable + Vector3.down);
		}
	}

	protected virtual void Awake()
	{
		AutoGenerateHangingRope componentInChildren = GetComponentInChildren<AutoGenerateHangingRope>();
		if ((bool)componentInChildren && !componentInChildren.HasGenerated)
		{
			componentInChildren.Generated += SetupLinks;
		}
		else
		{
			SetupLinks();
		}
	}

	private void SetupLinks()
	{
		ReplaceWithTemplate[] componentsInChildren = GetComponentsInChildren<ReplaceWithTemplate>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Awake();
		}
		links = GetComponentsInChildren<ChainLinkInteraction>(includeInactive: true);
		ChainLinkInteraction[] array = links;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Chain = this;
		}
		Sound = GetComponentInParent<PlayChainSound>();
		if (!base.transform.IsOnHeroPlane())
		{
			array = links;
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object.Destroy(array[i]);
			}
			Collider2D[] componentsInChildren2 = GetComponentsInChildren<Collider2D>(includeInactive: true);
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				componentsInChildren2[i].gameObject.layer = 2;
			}
			UnityEngine.Object.Destroy(this);
			HitRigidbody2D[] componentsInChildren3 = GetComponentsInChildren<HitRigidbody2D>(includeInactive: true);
			for (int i = 0; i < componentsInChildren3.Length; i++)
			{
				UnityEngine.Object.Destroy(componentsInChildren3[i]);
			}
			HitResponse[] componentsInChildren4 = GetComponentsInChildren<HitResponse>(includeInactive: true);
			for (int i = 0; i < componentsInChildren4.Length; i++)
			{
				UnityEngine.Object.Destroy(componentsInChildren4[i]);
			}
			Breakable[] componentsInChildren5 = GetComponentsInChildren<Breakable>(includeInactive: true);
			for (int i = 0; i < componentsInChildren5.Length; i++)
			{
				UnityEngine.Object.Destroy(componentsInChildren5[i]);
			}
			ChainAttackForce[] componentsInChildren6 = GetComponentsInChildren<ChainAttackForce>(includeInactive: true);
			for (int i = 0; i < componentsInChildren6.Length; i++)
			{
				UnityEngine.Object.Destroy(componentsInChildren6[i]);
			}
		}
		if (!lowestLink)
		{
			GetLowestLink();
		}
	}

	public void TouchedLink(Vector3 position)
	{
		if ((bool)Sound)
		{
			Sound.PlayTouchSound(position);
		}
		OnTouchedLink.Invoke();
		this.OnTouched?.Invoke(position);
	}

	public void DisableLinks(Transform trackObject)
	{
		AddDisableTracker(trackObject);
		if (!IsPushDisableStarted && (bool)lowestLink)
		{
			StartCoroutine(DisableLinksDelayed());
		}
	}

	public void AddDisableTracker(Transform trackObject)
	{
		rangeEnableTracked.Add(trackObject);
	}

	private IEnumerator DisableLinksDelayed()
	{
		IsPushDisableStarted = true;
		yield return new WaitForSeconds(touchTime);
		ChainLinkInteraction[] array = links;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		StartCoroutine(ReEnableLinks());
	}

	private IEnumerator ReEnableLinks()
	{
		bool stillInRange = true;
		while (stillInRange)
		{
			stillInRange = false;
			yield return new WaitForSeconds(playerRangeEnableDelay);
			foreach (Transform item in rangeEnableTracked)
			{
				if ((bool)item && Mathf.Abs(item.position.x - lowestLink.position.x) < playerRangeEnable)
				{
					stillInRange = true;
					break;
				}
			}
		}
		rangeEnableTracked.Clear();
		ChainLinkInteraction[] array = links;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
		IsPushDisableStarted = false;
	}

	private void GetLowestLink()
	{
		float num = float.MaxValue;
		ChainLinkInteraction[] array = links;
		foreach (ChainLinkInteraction chainLinkInteraction in array)
		{
			if (chainLinkInteraction.transform.position.y < num)
			{
				lowestLink = chainLinkInteraction.transform;
			}
		}
	}
}
