using System.Collections;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveLoreSceneController : MonoBehaviour
{
	[SerializeField]
	private PlayerDataTest activePDTest;

	[SerializeField]
	private TrackTriggerObjects requireInside;

	[Space]
	[SerializeField]
	private float startWaitTime;

	[SerializeField]
	private string sceneName;

	[Space]
	[SerializeField]
	private Transform wholeSceneParent;

	[SerializeField]
	private GameObject loreSceneOnly;

	[SerializeField]
	private Vector2 scenePosOffset;

	[Space]
	[SerializeField]
	private NestedFadeGroupBase screenFader;

	[SerializeField]
	private AnimationCurve screenFaderToOtherCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private AnimationCurve screenFaderToMainCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float lightFadeUpDuration;

	[SerializeField]
	private PlayParticleEffects transitionToOtherParticles;

	[SerializeField]
	private float blankerHoldDuration;

	[SerializeField]
	private float blankerFadeDownDuration;

	[SerializeField]
	private float minPlayDuration;

	[SerializeField]
	private float stopPlayGrace;

	[SerializeField]
	private float blankerFadeUpDuration;

	[SerializeField]
	private PlayParticleEffects transitionToMainParticles;

	[SerializeField]
	private float blankerFadeBackDuration;

	[Space]
	[SerializeField]
	private LocalisedTextCollection needolinText;

	[SerializeField]
	private string needolinStartedEvent;

	[SerializeField]
	private string needolinEndedEvent;

	private Transform otherSceneParent;

	private bool isOtherSceneLoaded;

	private Vector3 mainScenePosition;

	private Vector3 altScenePosition;

	private bool wasTransitioning;

	private float stopPlayGraceLeft;

	private bool needolinGraceToggle;

	private Color? initialAmbientLightColor;

	private Coroutine currentTransitionRoutine;

	private bool preventCancel;

	private bool isInOtherScene;

	private void Awake()
	{
		screenFader.AlphaSelf = 0f;
		if ((bool)loreSceneOnly)
		{
			loreSceneOnly.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		if (isOtherSceneLoaded)
		{
			SceneManager.UnloadSceneAsync(sceneName);
		}
	}

	private void Update()
	{
		bool isPerforming = HeroPerformanceRegion.IsPerforming;
		if (isInOtherScene && !preventCancel)
		{
			if (isPerforming)
			{
				stopPlayGraceLeft = stopPlayGrace;
				if (!needolinGraceToggle)
				{
					needolinGraceToggle = true;
					if ((bool)transitionToMainParticles)
					{
						transitionToMainParticles.StopParticleSystems();
					}
				}
			}
			else
			{
				stopPlayGraceLeft -= Time.deltaTime;
				if (needolinGraceToggle)
				{
					needolinGraceToggle = false;
					if ((bool)transitionToMainParticles)
					{
						transitionToMainParticles.PlayParticleSystems();
					}
				}
			}
			if (stopPlayGraceLeft > 0f)
			{
				return;
			}
		}
		needolinGraceToggle = false;
		if (isPerforming == wasTransitioning)
		{
			return;
		}
		if (currentTransitionRoutine != null)
		{
			if (preventCancel)
			{
				return;
			}
			StopCoroutine(currentTransitionRoutine);
		}
		if (isPerforming)
		{
			if (!activePDTest.IsFulfilled || ((bool)requireInside && !requireInside.IsInside))
			{
				return;
			}
			currentTransitionRoutine = StartCoroutine(TransitionToOtherScene());
		}
		else
		{
			currentTransitionRoutine = StartCoroutine(TransitionToMainScene());
		}
		wasTransitioning = isPerforming;
	}

	private IEnumerator TransitionToOtherScene()
	{
		yield return new WaitForSeconds(startWaitTime);
		EventRegister.SendEvent(needolinStartedEvent);
	}

	private IEnumerator TransitionToMainScene()
	{
		EventRegister.SendEvent(needolinEndedEvent);
		currentTransitionRoutine = null;
		yield break;
	}

	private IEnumerator SetupOtherScene()
	{
		bool flag = false;
		Scene scene = default(Scene);
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			Scene sceneAt = SceneManager.GetSceneAt(i);
			if (!(sceneAt.name != sceneName))
			{
				flag = true;
				isOtherSceneLoaded = true;
				scene = sceneAt;
				break;
			}
		}
		if (!flag)
		{
			yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			isOtherSceneLoaded = true;
			scene = SceneManager.GetSceneByName(sceneName);
		}
		GameObject[] rootGameObjects = scene.GetRootGameObjects();
		if (rootGameObjects.Length == 0)
		{
			Debug.LogError("Other scene did not have any root GameObjects", this);
		}
		else
		{
			otherSceneParent = rootGameObjects[0].transform;
		}
	}
}
