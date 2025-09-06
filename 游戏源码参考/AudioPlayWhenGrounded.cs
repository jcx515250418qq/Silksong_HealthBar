using UnityEngine;

public class AudioPlayWhenGrounded : MonoBehaviour
{
	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private Collider2D sweepCollider;

	[SerializeField]
	private float sweepDistance = 0.2f;

	public bool CanPlay = true;

	private bool isGrounded;

	private bool wasGrounded;

	private void OnEnable()
	{
		if (!audioSource || !sweepCollider)
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			audioSource.Stop();
		}
	}

	private void Update()
	{
		isGrounded = new Sweep(sweepCollider, 3, 3).Check(sweepDistance, 256);
		if (isGrounded && !CanPlay)
		{
			isGrounded = false;
		}
		if (isGrounded && !wasGrounded)
		{
			audioSource.Play();
		}
		else if (!isGrounded && wasGrounded)
		{
			audioSource.Stop();
		}
		wasGrounded = isGrounded;
	}
}
