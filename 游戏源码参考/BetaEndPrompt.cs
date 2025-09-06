using System.Collections;
using UnityEngine;

public class BetaEndPrompt : MonoBehaviour
{
	private AudioSource audioSource;

	[SerializeField]
	private float delayDuration;

	[SerializeField]
	private SimpleSpriteFade blackFade;

	private bool canEnd;

	protected void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	protected IEnumerator Start()
	{
		yield return new WaitForSeconds(delayDuration);
		canEnd = true;
	}

	protected void Update()
	{
		if (canEnd && (GameManager.instance.inputHandler.inputActions.MenuSubmit.IsPressed || GameManager.instance.inputHandler.inputActions.MenuCancel.IsPressed))
		{
			canEnd = false;
			StartCoroutine(BeginEnd());
		}
	}

	protected IEnumerator BeginEnd()
	{
		if (audioSource != null)
		{
			audioSource.Play();
		}
		blackFade.FadeIn();
		yield return new WaitForSeconds(blackFade.fadeDuration);
		GameManager.instance.StartCoroutine(GameManager.instance.ReturnToMainMenu(willSave: true));
	}
}
