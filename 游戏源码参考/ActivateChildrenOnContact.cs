using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActivateChildrenOnContact : MonoBehaviour
{
	public CircleCollider2D circleCollider;

	public BoxCollider2D boxCollider;

	public bool setActive = true;

	[SerializeField]
	private bool initChildren;

	[SerializeField]
	private bool onlyReactToPlayer;

	[SerializeField]
	private UnityEvent onContact;

	private Dictionary<Transform, bool> children = new Dictionary<Transform, bool>();

	private bool activated;

	private void Awake()
	{
		if (!initChildren)
		{
			return;
		}
		foreach (Transform item in base.transform)
		{
			bool activeSelf = item.gameObject.activeSelf;
			children[item] = activeSelf;
			if (!activeSelf)
			{
				item.gameObject.SetActive(value: true);
			}
		}
	}

	private IEnumerator Start()
	{
		if (!initChildren)
		{
			yield break;
		}
		yield return null;
		if (!activated)
		{
			foreach (Transform item in base.transform)
			{
				if (children.TryGetValue(item, out var value) && !value)
				{
					item.gameObject.SetActive(value: false);
				}
			}
		}
		children.Clear();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (onlyReactToPlayer && !collision.gameObject.CompareTag("Player"))
		{
			return;
		}
		activated = true;
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(setActive);
		}
		if (circleCollider != null)
		{
			circleCollider.enabled = false;
		}
		if (boxCollider != null)
		{
			boxCollider.enabled = false;
		}
		if (onContact != null)
		{
			onContact.Invoke();
		}
	}
}
