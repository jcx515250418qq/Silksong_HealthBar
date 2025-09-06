using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRecoilBlocker : MonoBehaviour
{
	private List<Recoil> touchingRecoilers;

	private readonly Dictionary<Recoil, Coroutine> touchingTrackRoutines = new Dictionary<Recoil, Coroutine>();

	private Collider2D[] results;

	private Collider2D selfCollider;

	private void Awake()
	{
		selfCollider = GetComponent<Collider2D>();
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		Recoil component = other.gameObject.GetComponent<Recoil>();
		if ((bool)component && !touchingTrackRoutines.ContainsKey(component))
		{
			touchingTrackRoutines[component] = StartCoroutine(TouchingTrack(component));
		}
	}

	private void OnCollisionExit2D(Collision2D other)
	{
		Recoil component = other.gameObject.GetComponent<Recoil>();
		if ((bool)component && !Physics2D.GetIgnoreCollision(selfCollider, other.collider) && touchingTrackRoutines.Remove(component, out var value))
		{
			StopCoroutine(value);
		}
	}

	private IEnumerator TouchingTrack(Recoil recoil)
	{
		Collider2D recoilCollider = recoil.GetComponent<Collider2D>();
		WaitForFixedUpdate loopWait = new WaitForFixedUpdate();
		WaitForSeconds recoilWait = new WaitForSeconds(0.1f);
		while (true)
		{
			if (recoilCollider == null)
			{
				yield break;
			}
			if (recoil.IsRecoiling)
			{
				if (!Physics2D.GetIgnoreCollision(selfCollider, recoilCollider))
				{
					Physics2D.IgnoreCollision(selfCollider, recoilCollider);
					yield return recoilWait;
				}
			}
			else if (Physics2D.GetIgnoreCollision(selfCollider, recoilCollider))
			{
				if (results == null)
				{
					results = new Collider2D[10];
				}
				int b = Physics2D.OverlapCollider(selfCollider, new ContactFilter2D
				{
					useLayerMask = true,
					layerMask = 1 << recoilCollider.gameObject.layer
				}, results);
				bool flag = false;
				for (int i = 0; i < Mathf.Min(results.Length, b); i++)
				{
					if (!(results[i] != recoilCollider))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					break;
				}
			}
			yield return loopWait;
		}
		touchingTrackRoutines.Remove(recoil);
		Physics2D.IgnoreCollision(selfCollider, recoilCollider, ignore: false);
	}
}
