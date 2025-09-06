using UnityEngine;

public class PlayAnimationInAir : MonoBehaviour
{
	[SerializeField]
	private tk2dSpriteAnimator animator;

	[SerializeField]
	private string airAnim;

	[SerializeField]
	private string groundAnim;

	private int currentCollisions;

	private void OnEnable()
	{
		if ((bool)animator)
		{
			animator.Play(airAnim);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		currentCollisions++;
		if (currentCollisions == 1 && (bool)animator)
		{
			animator.Play(groundAnim);
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		currentCollisions--;
		if (currentCollisions == 0 && (bool)animator)
		{
			animator.Play(airAnim);
		}
	}
}
