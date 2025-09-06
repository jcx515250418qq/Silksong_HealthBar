using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeoRock : MonoBehaviour, IBreakerBreakable
{
	[SerializeField]
	public GeoRockData geoRockData;

	private GameManager gm;

	private PlayMakerFSM fsm;

	private FsmInt hitsInt;

	private FsmFloat attackDirFloat;

	public BreakableBreaker.BreakableTypes BreakableType => BreakableBreaker.BreakableTypes.Basic;

	GameObject IBreakerBreakable.gameObject => base.gameObject;

	private void Awake()
	{
		fsm = GetComponent<PlayMakerFSM>();
		hitsInt = fsm.FsmVariables.GetFsmInt("Hits");
		attackDirFloat = fsm.FsmVariables.GetFsmFloat("Attack Direction");
	}

	private void OnEnable()
	{
		SceneManager.activeSceneChanged += LevelActivated;
		gm = GameManager.instance;
		gm.SavePersistentObjects += SaveState;
	}

	private void OnDisable()
	{
		SceneManager.activeSceneChanged -= LevelActivated;
		if (gm != null)
		{
			gm.SavePersistentObjects -= SaveState;
		}
	}

	private void Start()
	{
		SetMyId();
	}

	private void LevelActivated(Scene sceneFrom, Scene sceneTo)
	{
		SetMyId();
		GeoRockData geoRockData = SceneData.instance.FindMyState(this.geoRockData);
		if (geoRockData != null)
		{
			this.geoRockData.hitsLeft = geoRockData.hitsLeft;
			hitsInt.Value = geoRockData.hitsLeft;
		}
		else
		{
			UpdateHitsLeftFromFsm();
		}
	}

	private void SaveState()
	{
		SetMyId();
		UpdateHitsLeftFromFsm();
		SceneData.instance.SaveMyState(geoRockData);
	}

	private void SetMyId()
	{
		if (string.IsNullOrEmpty(geoRockData.id))
		{
			geoRockData.id = base.name;
		}
		if (string.IsNullOrEmpty(geoRockData.sceneName))
		{
			geoRockData.sceneName = GameManager.GetBaseSceneName(base.gameObject.scene.name);
		}
	}

	private void UpdateHitsLeftFromFsm()
	{
		geoRockData.hitsLeft = hitsInt.Value;
	}

	public void BreakFromBreaker(BreakableBreaker breaker)
	{
		for (int num = hitsInt.Value; num > 1; num--)
		{
			fsm.SendEvent("HIT SKIP EFFECTS");
		}
		HitFromBreaker(breaker);
	}

	public void HitFromBreaker(BreakableBreaker breaker)
	{
		attackDirFloat.Value = ((breaker.transform.position.x > base.transform.position.x) ? 180 : 0);
		fsm.SendEvent("TAKE DAMAGE");
	}
}
