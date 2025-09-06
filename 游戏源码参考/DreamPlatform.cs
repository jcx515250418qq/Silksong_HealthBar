using UnityEngine;

public class DreamPlatform : MonoBehaviour
{
	public TriggerEnterEvent outerCollider;

	public TriggerEnterEvent innerCollider;

	public Animator animator;

	public AudioClip activateSound;

	public AudioClip deactivateSound;

	private bool visible;

	public bool showOnEnable;

	private AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void Start()
	{
		if (showOnEnable)
		{
			return;
		}
		if ((bool)outerCollider)
		{
			outerCollider.OnTriggerExited += delegate
			{
				Hide();
			};
		}
		if ((bool)innerCollider)
		{
			innerCollider.OnTriggerEntered += delegate
			{
				Show();
			};
		}
	}

	private void OnEnable()
	{
		if (showOnEnable)
		{
			Show();
		}
	}

	public void Show()
	{
		if (!visible)
		{
			PlayAnimation("Show");
			activateSound.PlayOnSource(audioSource, 0.85f, 1.15f);
			visible = true;
		}
	}

	public void Hide()
	{
		if (visible)
		{
			PlayAnimation("Hide");
			deactivateSound.PlayOnSource(audioSource, 0.85f, 1.15f);
			visible = false;
		}
	}

	private void PlayAnimation(string animationName)
	{
		if ((bool)animator)
		{
			animator.Play(animationName);
		}
	}
}
