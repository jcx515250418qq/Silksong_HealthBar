using System;
using System.Collections;
using GlobalEnums;
using UnityEngine;

public class AreaTitleController : MonoBehaviour
{
	[Serializable]
	private struct Area
	{
		public PlayerDataTest Test;

		public string Identifier;

		public bool IsSubArea;

		[ModifiableProperty]
		[Conditional("IsSubArea", false, false, false)]
		public bool AlwaysSmallTitle;

		[PlayerDataField(typeof(bool), false)]
		public string VisitedBool;
	}

	[SerializeField]
	private string waitForEvent;

	[Space]
	[SerializeField]
	private Area[] orderedAreas;

	[SerializeField]
	[HideInInspector]
	private Area area;

	[SerializeField]
	private bool displayRight;

	[SerializeField]
	private string doorTrigger = "";

	[SerializeField]
	private string doorException = "";

	[SerializeField]
	private bool onlyOnRevisit;

	[SerializeField]
	private bool recordVisitedOnSkip;

	[SerializeField]
	private float unvisitedPause = 2f;

	[SerializeField]
	private float visitedPause = 2f;

	[SerializeField]
	private bool waitForTrigger;

	[SerializeField]
	private bool ignoreFirstSceneCheckIfUnvisited;

	[SerializeField]
	private bool alwaysBlockInteractIfUnvisited;

	[SerializeField]
	private bool triggerEnterWaitForHeroInPosition;

	[SerializeField]
	private bool onlyIfUnvisited;

	private GameObject areaTitle;

	private bool played;

	private bool waitingForEvent;

	private bool waitingForHero;

	private Area currentAreaData;

	private HeroController hc;

	private HeroController.HeroInPosition heroInPositionResponder;

	private bool started;

	private bool triggerEntered;

	private void OnValidate()
	{
		if (!string.IsNullOrEmpty(area.Identifier))
		{
			orderedAreas = new Area[1] { area };
			area = default(Area);
		}
	}

	private void Awake()
	{
		if (!string.IsNullOrEmpty(waitForEvent))
		{
			EventRegister register = EventRegister.GetRegisterGuaranteed(base.gameObject, waitForEvent);
			Action temp = null;
			temp = delegate
			{
				waitingForEvent = false;
				FindAreaTitle();
				DoPlay();
				register.ReceivedEvent -= temp;
			};
			register.ReceivedEvent += temp;
			waitingForEvent = true;
		}
	}

	private void Start()
	{
		if (!triggerEntered && !string.IsNullOrEmpty(waitForEvent))
		{
			return;
		}
		hc = HeroController.instance;
		if (!hc.isHeroInPosition)
		{
			heroInPositionResponder = delegate
			{
				waitingForHero = false;
				if (base.isActiveAndEnabled)
				{
					FindAreaTitle();
					if (triggerEntered)
					{
						Play();
					}
					else
					{
						DoPlay();
					}
				}
				hc.heroInPositionDelayed -= heroInPositionResponder;
				heroInPositionResponder = null;
			};
			hc.heroInPositionDelayed += heroInPositionResponder;
			waitingForHero = true;
		}
		else if (!waitingForEvent)
		{
			FindAreaTitle();
			if (triggerEntered)
			{
				Play();
			}
			else
			{
				DoPlay();
			}
		}
		else if (waitForTrigger && triggerEntered)
		{
			Play();
		}
		started = true;
	}

	private void OnDisable()
	{
		triggerEntered = false;
	}

	private void FindAreaTitle()
	{
		if ((bool)ManagerSingleton<AreaTitle>.Instance)
		{
			areaTitle = ManagerSingleton<AreaTitle>.Instance.gameObject;
		}
	}

	private void DoPlay()
	{
		if (!waitForTrigger && !waitingForEvent && !waitingForHero)
		{
			Play();
		}
	}

	protected void OnDestroy()
	{
		if (hc != null && heroInPositionResponder != null)
		{
			hc.heroInPositionDelayed -= heroInPositionResponder;
			hc = null;
			heroInPositionResponder = null;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (base.isActiveAndEnabled && !played && collision.CompareTag("Player"))
		{
			triggerEntered = true;
			if (!waitingForHero && started)
			{
				Play();
			}
		}
	}

	public void Play()
	{
		if (played)
		{
			return;
		}
		played = true;
		currentAreaData = GetAreaTitle();
		if (!string.IsNullOrEmpty(doorTrigger))
		{
			if (HeroController.instance.GetEntryGateName() == doorTrigger)
			{
				CheckArea();
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}
		else if (!string.IsNullOrEmpty(doorException))
		{
			if (HeroController.instance.GetEntryGateName() != doorException)
			{
				CheckArea();
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}
		else
		{
			CheckArea();
		}
	}

	private Area GetAreaTitle()
	{
		if (orderedAreas != null)
		{
			for (int i = 0; i < orderedAreas.Length; i++)
			{
				Area result = orderedAreas[i];
				if (result.Test.IsFulfilled)
				{
					return result;
				}
			}
		}
		return default(Area);
	}

	private void CheckArea()
	{
		Finish();
	}

	private void Finish()
	{
		GameManager instance = GameManager.instance;
		if (string.IsNullOrEmpty(currentAreaData.Identifier))
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		if (instance != null && instance.IsFirstLevelForPlayer)
		{
			PlayerData playerData = GameManager.instance.playerData;
			if (!string.IsNullOrEmpty(currentAreaData.VisitedBool) && playerData.GetBool(currentAreaData.VisitedBool))
			{
				if (!string.IsNullOrEmpty(currentAreaData.Identifier))
				{
					playerData.currentArea = currentAreaData.Identifier;
				}
				base.gameObject.SetActive(value: false);
				return;
			}
			if (!ignoreFirstSceneCheckIfUnvisited)
			{
				base.gameObject.SetActive(value: false);
				return;
			}
		}
		if (currentAreaData.IsSubArea)
		{
			StartCoroutine(VisitPause(null));
			return;
		}
		PlayerData playerData2 = GameManager.instance.playerData;
		string currentArea = playerData2.currentArea;
		bool flag = !string.IsNullOrEmpty(currentAreaData.VisitedBool) && playerData2.GetBool(currentAreaData.VisitedBool);
		if (onlyIfUnvisited && flag)
		{
			return;
		}
		if ((!flag && onlyOnRevisit) || currentAreaData.Identifier == currentArea)
		{
			if (recordVisitedOnSkip && !string.IsNullOrEmpty(currentAreaData.VisitedBool))
			{
				GameManager.instance.playerData.SetBool(currentAreaData.VisitedBool, value: true);
				playerData2.currentArea = currentAreaData.Identifier;
			}
			base.gameObject.SetActive(value: false);
		}
		else
		{
			Action afterDelay = delegate
			{
				playerData2.currentArea = currentAreaData.Identifier;
			};
			StartCoroutine((flag || currentAreaData.AlwaysSmallTitle) ? VisitPause(afterDelay) : UnvisitPause(afterDelay));
		}
	}

	public void ForcePlay()
	{
		if ((bool)areaTitle)
		{
			areaTitle.SetActive(value: true);
			PlayMakerFSM fSM = FSMUtility.GetFSM(areaTitle);
			if ((bool)fSM)
			{
				FSMUtility.SetBool(fSM, "Visited", value: true);
				FSMUtility.SetBool(fSM, "NPC Title", value: false);
				FSMUtility.SetBool(fSM, "City Title", IsCityTitle());
				FSMUtility.SetBool(fSM, "Display Right", displayRight);
				FSMUtility.SetString(fSM, "Area Event", currentAreaData.Identifier);
			}
		}
	}

	public void ForcePlayLarge()
	{
		if ((bool)areaTitle)
		{
			areaTitle.SetActive(value: true);
			PlayMakerFSM fSM = FSMUtility.GetFSM(areaTitle);
			if ((bool)fSM)
			{
				FSMUtility.SetBool(fSM, "Visited", value: false);
				FSMUtility.SetBool(fSM, "NPC Title", value: false);
				FSMUtility.SetBool(fSM, "City Title", IsCityTitle());
				FSMUtility.SetBool(fSM, "Display Right", displayRight);
				FSMUtility.SetString(fSM, "Area Event", currentAreaData.Identifier);
			}
			base.gameObject.SetActive(value: false);
		}
	}

	private IEnumerator VisitPause(Action afterDelay)
	{
		yield return new WaitForSeconds(visitedPause);
		afterDelay?.Invoke();
		if (!areaTitle)
		{
			yield break;
		}
		if (areaTitle.gameObject.activeInHierarchy)
		{
			while ((bool)InteractManager.BlockingInteractable)
			{
				yield return null;
			}
			float timeOut = 5f;
			while (areaTitle.gameObject.activeInHierarchy && timeOut > 0f)
			{
				timeOut -= Time.deltaTime;
				yield return null;
			}
			areaTitle.gameObject.SetActive(value: false);
		}
		areaTitle.SetActive(value: true);
		PlayMakerFSM fSM = FSMUtility.GetFSM(areaTitle);
		if ((bool)fSM)
		{
			FSMUtility.SetBool(fSM, "Visited", value: true);
			FSMUtility.SetBool(fSM, "NPC Title", value: false);
			FSMUtility.SetBool(fSM, "City Title", IsCityTitle());
			FSMUtility.SetBool(fSM, "Display Right", displayRight);
			FSMUtility.SetString(fSM, "Area Event", currentAreaData.Identifier);
		}
		if (!string.IsNullOrEmpty(currentAreaData.VisitedBool))
		{
			GameManager.instance.playerData.SetBool(currentAreaData.VisitedBool, value: true);
		}
	}

	private IEnumerator UnvisitPause(Action afterDelay)
	{
		if ((bool)InteractManager.BlockingInteractable || alwaysBlockInteractIfUnvisited)
		{
			InteractManager.IsDisabled = true;
		}
		yield return new WaitForSeconds(unvisitedPause);
		afterDelay?.Invoke();
		if (!areaTitle)
		{
			yield break;
		}
		InteractManager.IsDisabled = true;
		while ((bool)InteractManager.BlockingInteractable)
		{
			while ((bool)InteractManager.BlockingInteractable)
			{
				yield return null;
			}
			float timeOut = 5f;
			while (areaTitle.gameObject.activeInHierarchy && timeOut > 0f)
			{
				timeOut -= Time.deltaTime;
				yield return null;
			}
		}
		areaTitle.SetActive(value: false);
		areaTitle.SetActive(value: true);
		PlayMakerFSM fSM = FSMUtility.GetFSM(areaTitle);
		if ((bool)fSM)
		{
			FSMUtility.SetBool(fSM, "Visited", value: false);
			FSMUtility.SetBool(fSM, "NPC Title", value: false);
			FSMUtility.SetBool(fSM, "City Title", IsCityTitle());
			FSMUtility.SetString(fSM, "Area Event", currentAreaData.Identifier);
			if (!string.IsNullOrEmpty(currentAreaData.VisitedBool))
			{
				GameManager.instance.playerData.SetBool(currentAreaData.VisitedBool, value: true);
			}
		}
	}

	private bool IsCityTitle()
	{
		MapZone currentMapZoneEnum = GameManager.instance.GetCurrentMapZoneEnum();
		if (currentMapZoneEnum == MapZone.CITY_OF_SONG || (uint)(currentMapZoneEnum - 22) <= 1u || (uint)(currentMapZoneEnum - 26) <= 1u)
		{
			return true;
		}
		return false;
	}
}
