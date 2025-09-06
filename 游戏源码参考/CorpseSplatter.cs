using UnityEngine;

public class CorpseSplatter : MonoBehaviour
{
	[SerializeField]
	private GameObject splatEffect;

	private void Awake()
	{
		PersonalObjectPool.EnsurePooledInScene(base.gameObject, splatEffect, 2);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer == 26)
		{
			ActiveCorpse component = other.GetComponent<ActiveCorpse>();
			if ((bool)component)
			{
				component.gameObject.SetActive(value: false);
				splatEffect.Spawn(component.transform.position);
			}
		}
	}
}
