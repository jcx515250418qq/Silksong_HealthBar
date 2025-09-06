using System.Collections;
using System.Collections.Generic;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.Audio;

public class CreditsHelper : MonoBehaviour
{
	[SerializeField]
	private CutsceneHelper cutSceneHelper;

	[SerializeField]
	private NestedFadeGroupBase screenFader;

	[SerializeField]
	private float startPause;

	[SerializeField]
	private float timeBetweenScreens;

	[SerializeField]
	private float endPause;

	[SerializeField]
	private GameObject activateOnEnd;

	[Space]
	[SerializeField]
	private AudioSource musicSource;

	[SerializeField]
	private AudioMixerSnapshot silentSnapshot;

	[SerializeField]
	private AudioMixerSnapshot musicSnapshot;

	private List<CreditsSectionBase> creditsSections;

	private void Awake()
	{
		creditsSections = new List<CreditsSectionBase>(base.transform.childCount);
		for (int i = 0; i < base.transform.childCount; i++)
		{
			CreditsSectionBase component = base.transform.GetChild(i).GetComponent<CreditsSectionBase>();
			if ((bool)component)
			{
				creditsSections.Add(component);
			}
		}
	}

	private void Start()
	{
		foreach (CreditsSectionBase creditsSection in creditsSections)
		{
			creditsSection.gameObject.SetActive(value: false);
		}
		if ((bool)activateOnEnd)
		{
			activateOnEnd.SetActive(value: false);
		}
		GameCameras.instance.cameraController.IsBloomForced = true;
		StartCoroutine(Sequence());
	}

	private IEnumerator Sequence()
	{
		screenFader.AlphaSelf = 1f;
		if (silentSnapshot != null)
		{
			silentSnapshot.TransitionTo(0f);
		}
		yield return new WaitForSeconds(startPause);
		musicSource.Play();
		for (int i = 0; i < creditsSections.Count; i++)
		{
			CreditsSectionBase creditsSection = creditsSections[i];
			creditsSection.gameObject.SetActive(value: true);
			if (i == 0 && musicSnapshot != null)
			{
				musicSnapshot.TransitionTo(creditsSection.FadeUpDuration);
			}
			yield return new WaitForSeconds(screenFader.FadeTo(0f, creditsSection.FadeUpDuration));
			yield return creditsSection.Show();
			if (i >= creditsSections.Count - 1 && silentSnapshot != null)
			{
				silentSnapshot.TransitionTo(creditsSection.FadeDownDuration + timeBetweenScreens);
			}
			yield return new WaitForSeconds(screenFader.FadeTo(1f, creditsSection.FadeDownDuration));
			creditsSection.gameObject.SetActive(value: false);
			yield return new WaitForSeconds(timeBetweenScreens);
		}
		yield return new WaitForSeconds(endPause);
		StartCoroutine(cutSceneHelper.Skip());
		GameCameras.instance.cameraController.IsBloomForced = false;
	}
}
