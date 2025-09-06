using UnityEngine;
using UnityEngine.Audio;

public class RegionSetAudio : MonoBehaviour
{
	[SerializeField]
	private AudioMixerSnapshot atmosSnapshotEnter;

	[SerializeField]
	private AudioMixerSnapshot enviroSnapshotEnter;

	[SerializeField]
	private AudioMixerSnapshot atmosSnapshotExit;

	[SerializeField]
	private AudioMixerSnapshot enviroSnapshotExit;

	public float transitionTime;

	private CustomSceneManager sm;

	private bool sceneSetup;

	private bool entered;

	private bool queueEnter;

	private void OnEnable()
	{
		HeroController hc = HeroController.instance;
		if (hc.isHeroInPosition)
		{
			SubscribeSceneManager();
			return;
		}
		HeroController.HeroInPosition temp = null;
		temp = delegate
		{
			SubscribeSceneManager();
			hc.heroInPosition -= temp;
		};
		hc.heroInPosition += temp;
	}

	private void SubscribeSceneManager()
	{
		sm = GameManager.instance.sm;
		sm.AudioSnapshotsApplied += OnSceneManagerAppliedSnapshots;
	}

	private void UnsubscribeSceneManager()
	{
		if (!sceneSetup)
		{
			sm.AudioSnapshotsApplied -= OnSceneManagerAppliedSnapshots;
			sm = null;
		}
	}

	private void OnSceneManagerAppliedSnapshots()
	{
		UnsubscribeSceneManager();
		sceneSetup = true;
		if (queueEnter)
		{
			DoEnter();
		}
	}

	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		DoEnter();
	}

	private void OnTriggerStay2D(Collider2D otherCollider)
	{
		DoEnter();
	}

	private void DoEnter()
	{
		if (!sceneSetup)
		{
			queueEnter = true;
		}
		else if (!entered)
		{
			if (atmosSnapshotEnter != null)
			{
				AudioManager.TransitionToAtmosOverride(atmosSnapshotEnter, transitionTime);
			}
			if (enviroSnapshotEnter != null)
			{
				enviroSnapshotEnter.TransitionTo(transitionTime);
			}
			entered = true;
		}
	}

	private void OnTriggerExit2D(Collider2D otherCollider)
	{
		queueEnter = false;
		if (entered)
		{
			if (atmosSnapshotExit != null)
			{
				AudioManager.TransitionToAtmosOverride(atmosSnapshotExit, transitionTime);
			}
			if (enviroSnapshotExit != null)
			{
				enviroSnapshotExit.TransitionTo(transitionTime);
			}
			entered = false;
		}
	}
}
