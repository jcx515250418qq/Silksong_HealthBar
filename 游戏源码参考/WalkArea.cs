using UnityEngine;

public class WalkArea : MonoBehaviour
{
	private Collider2D myCollider;

	private bool activated;

	private GameManager gm;

	protected void Awake()
	{
		myCollider = GetComponent<Collider2D>();
	}

	private void Start()
	{
		gm = GameManager.instance;
		gm.UnloadingLevel += Deactivate;
	}

	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		if (otherCollider.gameObject.layer == 9)
		{
			activated = true;
			HeroController.instance.SetWalkZone(inWalkZone: true);
		}
	}

	private void OnTriggerStay2D(Collider2D otherCollider)
	{
		if (!activated && myCollider.enabled && otherCollider.gameObject.layer == 9)
		{
			activated = true;
			HeroController.instance.SetWalkZone(inWalkZone: true);
		}
	}

	private void OnTriggerExit2D(Collider2D otherCollider)
	{
		if (otherCollider.gameObject.layer == 9)
		{
			activated = false;
			HeroController.instance.SetWalkZone(inWalkZone: false);
		}
	}

	private void Deactivate()
	{
		activated = false;
		HeroController.instance.SetWalkZone(inWalkZone: false);
	}

	private void OnDisable()
	{
		if (gm != null)
		{
			gm.UnloadingLevel -= Deactivate;
		}
		if (activated)
		{
			activated = false;
			HeroController silentInstance = HeroController.SilentInstance;
			if ((bool)silentInstance)
			{
				silentInstance.SetWalkZone(inWalkZone: false);
			}
		}
	}
}
