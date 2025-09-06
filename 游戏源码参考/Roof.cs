using UnityEngine;

public class Roof : NonSlider
{
	private const string PlayerTag = "Player";

	private const string CancelSuperDashMethod = "CancelSuperDash";

	private const string CancelHeroJumpMethod = "CancelHeroJump";

	protected void OnCollisionEnter2D(Collision2D collision)
	{
		GameObject gameObject = collision.gameObject;
		if (gameObject.CompareTag("Player"))
		{
			gameObject.SendMessage("CancelSuperDash", SendMessageOptions.DontRequireReceiver);
			gameObject.SendMessage("CancelHeroJump", SendMessageOptions.DontRequireReceiver);
		}
	}
}
