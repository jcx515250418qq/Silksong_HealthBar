using UnityEngine;
using UnityEngine.Audio;

public class VentRegion : MonoBehaviour
{
	private const string ENTER_EVENT = "VENT REGION ENTER";

	private const string EXIT_EVENT = "VENT REGION EXIT";

	[SerializeField]
	private TrackTriggerObjects trackTrigger;

	[SerializeField]
	private bool invert;

	[Space]
	[SerializeField]
	private AudioMixerSnapshot enviroSnapshotEnter;

	[SerializeField]
	private float transitionTime = 0.5f;

	[SerializeField]
	private bool resetToSceneEnviro;

	[SerializeField]
	private GameObject windPlayer;

	[Space]
	[SerializeField]
	private WorldRumbleManager rumbleManager;

	[SerializeField]
	private float rumbleMagnitudeMultiplier = 1f;

	private AudioSource windPlayerSource;

	private AudioMixerSnapshot enviroSnapshotExit;

	private bool hasEntered;

	private bool entered;

	private bool active = true;

	private float windVolume;

	private Transform windPlayerTransform;

	private Transform heroTransform;

	private GameManager gm;

	private void Awake()
	{
		EventRegister.GetRegisterGuaranteed(base.gameObject, "HERO HAZARD DEATH").ReceivedEvent += HeroHazardDeath;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "HAZARD RESPAWNED").ReceivedEvent += HeroHazardRespawned;
	}

	private void Start()
	{
		gm = GameManager.instance;
		gm.SceneInit += OnSceneInit;
		if ((bool)gm.hero_ctrl)
		{
			OnSceneInit();
		}
		windPlayerSource = windPlayer.GetComponent<AudioSource>();
		windPlayerTransform = windPlayer.transform;
		OnInsideStateChanged(trackTrigger.IsInside);
		trackTrigger.InsideStateChanged += OnInsideStateChanged;
	}

	private void OnDestroy()
	{
		if ((bool)gm)
		{
			gm.SceneInit -= OnSceneInit;
			gm = null;
		}
	}

	private void OnSceneInit()
	{
		heroTransform = gm.hero_ctrl.transform;
		if (resetToSceneEnviro)
		{
			enviroSnapshotExit = gm.GetSceneManager().GetComponent<CustomSceneManager>().enviroSnapshot;
		}
	}

	private void OnInsideStateChanged(bool isInside)
	{
		if (invert)
		{
			isInside = !isInside;
		}
		if (isInside)
		{
			if (active && !entered)
			{
				EnterRegion();
			}
		}
		else if (active && entered)
		{
			ExitRegion();
		}
	}

	private void EnterRegion()
	{
		if (enviroSnapshotEnter != null)
		{
			enviroSnapshotEnter.TransitionTo(transitionTime);
		}
		windPlayerSource.Play();
		windVolume = 0f;
		EventRegister.SendEvent("VENT REGION ENTER");
		if (!hasEntered)
		{
			GameManager.instance.GetSceneManager().GetComponent<CustomSceneManager>().CancelEnviroSnapshot();
			hasEntered = true;
		}
		if ((bool)rumbleManager)
		{
			rumbleManager.AddMagnitudeMultiplier(this, rumbleMagnitudeMultiplier);
		}
		entered = true;
	}

	private void ExitRegion()
	{
		if (enviroSnapshotExit != null && resetToSceneEnviro)
		{
			enviroSnapshotExit.TransitionTo(transitionTime);
		}
		EventRegister.SendEvent("VENT REGION EXIT");
		if ((bool)rumbleManager)
		{
			rumbleManager.RemoveMagnitudeMultiplier(this);
		}
		entered = false;
	}

	private void Update()
	{
		if (!active)
		{
			return;
		}
		if (entered)
		{
			if ((bool)heroTransform)
			{
				windPlayerTransform.position = heroTransform.position;
			}
			if (windVolume < 1f)
			{
				windVolume += transitionTime * Time.deltaTime;
				if (windVolume > 1f)
				{
					windVolume = 1f;
				}
				windPlayerSource.volume = windVolume;
			}
		}
		else if (windVolume > 0f)
		{
			windVolume -= transitionTime * Time.deltaTime;
			if (windVolume < 0f)
			{
				windVolume = 0f;
			}
			windPlayerSource.volume = windVolume;
		}
	}

	public void HeroHazardDeath()
	{
		active = false;
	}

	public void HeroHazardRespawned()
	{
		active = true;
	}
}
