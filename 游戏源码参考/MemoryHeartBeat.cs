using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

public class MemoryHeartBeat : MonoBehaviour
{
	[SerializeField]
	private MinMaxFloat beatDelay;

	[SerializeField]
	private RandomAudioClipTable beatTable;

	[SerializeField]
	private AudioSource audioSource;

	[Space]
	[SerializeField]
	private AudioLowPassFilter lowPassFilter;

	[SerializeField]
	private float lowPassRegularCutoff;

	[SerializeField]
	private float lowPassSceneCutoff;

	[Space]
	[SerializeField]
	private float fovOffset;

	[SerializeField]
	private AnimationCurve fovOffsetCurve;

	[SerializeField]
	private float fovOffsetDuration;

	[Space]
	[SerializeField]
	private GameObject[] enableOnBeat;

	[Space]
	[SerializeField]
	private string beatEvent;

	private int beatEventId;

	private Coroutine beatRoutine;

	private bool isInSpecialScene;

	private Coroutine fovRoutine;

	private float volume;

	public float Multiplier { get; set; }

	private void OnValidate()
	{
		if (fovOffsetDuration > beatDelay.Start)
		{
			fovOffsetDuration = beatDelay.Start;
		}
	}

	private void Awake()
	{
		EventRegister.GetRegisterGuaranteed(base.gameObject, "HEARTBEAT_SCENE_START").ReceivedEvent += delegate
		{
			isInSpecialScene = true;
		};
		EventRegister.GetRegisterGuaranteed(base.gameObject, "HEARTBEAT_SCENE_END").ReceivedEvent += delegate
		{
			isInSpecialScene = false;
		};
		beatEventId = EventRegister.GetEventHashCode(beatEvent);
		Multiplier = 1f;
		volume = audioSource.volume;
	}

	private void OnEnable()
	{
		beatRoutine = StartCoroutine(BeatRoutine());
	}

	private void OnDisable()
	{
		StopCoroutine(beatRoutine);
		beatRoutine = null;
		GameCameras silentInstance = GameCameras.SilentInstance;
		if ((bool)silentInstance && (bool)silentInstance.forceCameraAspect)
		{
			silentInstance.forceCameraAspect.SetExtraFovOffset(0f);
		}
	}

	private IEnumerator BeatRoutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(beatDelay.GetRandomValue());
			lowPassFilter.cutoffFrequency = (isInSpecialScene ? lowPassSceneCutoff : lowPassRegularCutoff);
			audioSource.volume = 1f;
			beatTable.PlayOneShot(audioSource);
			audioSource.volume *= volume * Multiplier;
			if (Mathf.Abs(fovOffset) > Mathf.Epsilon)
			{
				if (fovRoutine != null)
				{
					StopCoroutine(fovRoutine);
				}
				fovRoutine = StartCoroutine(TransitionFovOffset());
			}
			enableOnBeat.SetAllActive(value: true);
			EventRegister.SendEvent(beatEventId);
		}
	}

	private IEnumerator TransitionFovOffset()
	{
		ForceCameraAspect cam = GameCameras.instance.forceCameraAspect;
		for (float elapsed = 0f; elapsed < fovOffsetDuration; elapsed += Time.deltaTime)
		{
			float num = fovOffsetCurve.Evaluate(elapsed / fovOffsetDuration);
			cam.SetExtraFovOffset(fovOffset * num * Multiplier);
			yield return null;
		}
		cam.SetExtraFovOffset(fovOffset * fovOffsetCurve.Evaluate(1f) * Multiplier);
		fovRoutine = null;
	}
}
