using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ActivationRange : MonoBehaviour
{
	public bool reverseActivation;

	[FormerlySerializedAs("initChildren")]
	[SerializeField]
	private bool initialiseChildren;

	private List<IInitialisable> initialisers = new List<IInitialisable>();

	private void Awake()
	{
		if (!initialiseChildren || reverseActivation)
		{
			return;
		}
		ActivateChildren();
		initialisers.AddRange(GetComponentsInChildren<IInitialisable>(includeInactive: true));
		foreach (IInitialisable initialiser in initialisers)
		{
			initialiser.OnAwake();
		}
	}

	private void Start()
	{
		if (!reverseActivation)
		{
			if (initialiseChildren)
			{
				foreach (IInitialisable initialiser in initialisers)
				{
					initialiser.OnStart();
				}
			}
			DeactivateChildren();
		}
		else
		{
			ActivateChildren();
		}
		initialisers.Clear();
	}

	private void OnDestroy()
	{
		initialisers.Clear();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!reverseActivation)
		{
			ActivateChildren();
		}
		else
		{
			DeactivateChildren();
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (!reverseActivation)
		{
			DeactivateChildren();
		}
		else
		{
			ActivateChildren();
		}
	}

	private void ActivateChildren()
	{
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(value: true);
		}
	}

	private void DeactivateChildren()
	{
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(value: false);
		}
	}
}
