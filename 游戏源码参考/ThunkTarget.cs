using UnityEngine;

public class ThunkTarget : MonoBehaviour
{
	[SerializeField]
	private GameObject effectPrefab;

	private void Awake()
	{
		PersonalObjectPool.EnsurePooledInScene(base.gameObject, effectPrefab, 5);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.GetComponent<NailSlashTerrainThunk>())
		{
			return;
		}
		Vector2 vector = ((Vector2)base.transform.position + (Vector2)other.transform.position) * 0.5f;
		vector = other.bounds.ClosestPoint(vector);
		effectPrefab.Spawn(vector.ToVector3(effectPrefab.transform.localPosition.z));
		HeroController componentInParent = other.GetComponentInParent<HeroController>();
		if ((bool)componentInParent && !componentInParent.cState.upAttacking && !componentInParent.cState.downAttacking)
		{
			componentInParent.SetAllowRecoilWhileRelinquished(value: true);
			if (componentInParent.cState.facingRight)
			{
				componentInParent.RecoilLeft();
			}
			else
			{
				componentInParent.RecoilRight();
			}
			componentInParent.SetAllowRecoilWhileRelinquished(value: false);
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		TryThunk(other.collider);
	}

	private void TryThunk(Collider2D other)
	{
		if (!other.GetComponent<NailSlashTerrainThunk>())
		{
			return;
		}
		Vector2 vector = ((Vector2)base.transform.position + (Vector2)other.transform.position) * 0.5f;
		vector = other.bounds.ClosestPoint(vector);
		effectPrefab.Spawn(vector.ToVector3(effectPrefab.transform.localPosition.z));
		HeroController componentInParent = other.GetComponentInParent<HeroController>();
		if ((bool)componentInParent && !componentInParent.cState.upAttacking && !componentInParent.cState.downAttacking)
		{
			componentInParent.SetAllowRecoilWhileRelinquished(value: true);
			if (componentInParent.cState.facingRight)
			{
				componentInParent.RecoilLeft();
			}
			else
			{
				componentInParent.RecoilRight();
			}
			componentInParent.SetAllowRecoilWhileRelinquished(value: false);
		}
	}
}
