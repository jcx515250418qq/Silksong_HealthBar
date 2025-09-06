using System.Collections;
using UnityEngine;

public class StartAudioPlayerAfterHeroInPosition : MonoBehaviour
{
	protected IEnumerator Start()
	{
		yield return null;
		while (HeroController.instance == null || !HeroController.instance.isHeroInPosition)
		{
			yield return null;
		}
		AudioSource component = GetComponent<AudioSource>();
		if (component != null)
		{
			component.Play();
		}
	}
}
