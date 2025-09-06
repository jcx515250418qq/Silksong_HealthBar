using UnityEngine;

public class ChainLinkInteraction : MonoBehaviour
{
	private Collider2D collider;

	public ChainPushReaction Chain { get; set; }

	private void Awake()
	{
		collider = GetComponent<Collider2D>();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if ((bool)Chain)
		{
			Chain.TouchedLink(collision.GetSafeContact().Point);
			if (!Chain.IsPushDisableStarted)
			{
				Chain.DisableLinks(collision.transform);
			}
			else
			{
				Chain.AddDisableTracker(collision.transform);
			}
		}
	}

	public void SetActive(bool value)
	{
		if ((bool)collider)
		{
			collider.enabled = value;
		}
	}
}
