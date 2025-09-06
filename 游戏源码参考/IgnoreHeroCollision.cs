using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class IgnoreHeroCollision : MonoBehaviour
{
	[SerializeField]
	private bool onlyWhileFloating;

	private HeroController hc;

	private List<(Collider2D, Collider2D)> ignoredColliders;

	private bool CanIgnore
	{
		get
		{
			if ((bool)hc)
			{
				if (onlyWhileFloating)
				{
					return hc.cState.floating;
				}
				return true;
			}
			return false;
		}
	}

	private void Start()
	{
		hc = HeroController.instance;
		if (!hc)
		{
			return;
		}
		if (onlyWhileFloating)
		{
			ignoredColliders = new List<(Collider2D, Collider2D)>();
			EventRegister.GetRegisterGuaranteed(base.gameObject, "BROLLY START").ReceivedEvent += Ignore;
			EventRegister.GetRegisterGuaranteed(base.gameObject, "BROLLY END").ReceivedEvent += Restore;
			return;
		}
		if (hc.isHeroInPosition)
		{
			Ignore();
			return;
		}
		HeroController.HeroInPosition temp = null;
		temp = delegate
		{
			Ignore();
			hc.heroInPosition -= temp;
			temp = null;
		};
		hc.heroInPosition += temp;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (CanIgnore && collision.gameObject.CompareTag("Player"))
		{
			IgnoreCollision(collision.collider, collision.otherCollider);
		}
	}

	private void Ignore()
	{
		if (CanIgnore)
		{
			Collider2D component = GetComponent<Collider2D>();
			Collider2D[] components = HeroController.instance.GetComponents<Collider2D>();
			foreach (Collider2D colB in components)
			{
				IgnoreCollision(component, colB);
			}
		}
	}

	private void Restore()
	{
		foreach (var (collider, collider2) in ignoredColliders)
		{
			Physics2D.IgnoreCollision(collider, collider2, ignore: false);
		}
		ignoredColliders.Clear();
	}

	private void IgnoreCollision(Collider2D colA, Collider2D colB)
	{
		Physics2D.IgnoreCollision(colA, colB);
		if (onlyWhileFloating)
		{
			ignoredColliders.Add((colA, colB));
		}
	}
}
