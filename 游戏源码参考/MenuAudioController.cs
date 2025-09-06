using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuAudioController : MonoBehaviour
{
	[SerializeField]
	private AudioSource audioSource;

	[Header("Sound Effects")]
	[SerializeField]
	private AudioClip select;

	[SerializeField]
	private AudioClip submit;

	[SerializeField]
	private AudioClip cancel;

	[SerializeField]
	private AudioClip slider;

	[SerializeField]
	private AudioClip startGame;

	[SerializeField]
	private AudioClip openProfileSelect;

	[Space]
	[SerializeField]
	private AudioClip pause;

	[SerializeField]
	private AudioClip unpause;

	private IEnumerator Start()
	{
		if (!(SceneManager.GetActiveScene().name == "Pre_Menu_Intro"))
		{
			float startVol = audioSource.volume;
			audioSource.volume = 0f;
			yield return GameManager.instance.timeTool.TimeScaleIndependentWaitForSeconds(1f);
			audioSource.volume = startVol;
		}
	}

	public void PlaySelect()
	{
		if ((bool)select)
		{
			audioSource.PlayOneShot(select);
		}
	}

	public void PlaySubmit()
	{
		if ((bool)submit)
		{
			audioSource.PlayOneShot(submit);
		}
	}

	public void PlayCancel()
	{
		if ((bool)cancel)
		{
			audioSource.PlayOneShot(cancel);
		}
	}

	public void PlaySlider()
	{
		if ((bool)slider)
		{
			audioSource.PlayOneShot(slider);
		}
	}

	public void PlayStartGame()
	{
		if ((bool)startGame)
		{
			audioSource.PlayOneShot(startGame);
		}
	}

	public void PlayOpenProfileSelect()
	{
		if ((bool)openProfileSelect)
		{
			audioSource.PlayOneShot(openProfileSelect);
		}
	}

	public void PlayPause()
	{
		if ((bool)pause)
		{
			audioSource.PlayOneShot(pause);
		}
	}

	public void PlayUnpause()
	{
		if ((bool)unpause)
		{
			audioSource.PlayOneShot(unpause);
		}
	}
}
