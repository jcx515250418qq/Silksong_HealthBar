using UnityEngine;

public class HeroPlatformCollisionSplit : MonoBehaviour
{
	[SerializeField]
	[Tooltip("The platform the Hero should collide with. Others will ignore collision with it.")]
	private Collider2D heroPlatform;

	[SerializeField]
	private TriggerEnterEvent othersEnterDetector;

	[SerializeField]
	[Tooltip("The platform that Others should collide with. Hero will ignore collision with it.")]
	private Collider2D othersPlatform;

	private Collider2D heroCol;

	private void Start()
	{
		if ((bool)othersPlatform)
		{
			heroCol = HeroController.instance.GetComponent<Collider2D>();
			Physics2D.IgnoreCollision(heroCol, othersPlatform);
		}
		if ((bool)heroPlatform && (bool)othersEnterDetector)
		{
			othersEnterDetector.OnTriggerEntered += OnOthersEnterDetectorEntered;
		}
	}

	private void OnOthersEnterDetectorEntered(Collider2D collider, GameObject sender)
	{
		if (!(collider == heroCol))
		{
			Physics2D.IgnoreCollision(collider, heroPlatform);
		}
	}
}
