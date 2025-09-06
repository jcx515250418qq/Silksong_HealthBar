namespace HutongGames.PlayMaker.Actions
{
	public class SetDeathRespawnMarker : FSMUtility.GetComponentFsmStateAction<RespawnMarker>
	{
		protected override void DoAction(RespawnMarker marker)
		{
			HeroController instance = HeroController.instance;
			GameManager instance2 = GameManager.instance;
			instance.SetBenchRespawn(spawnType: marker.GetComponent<RestBench>() ? 1 : 0, spawnMarker: marker, sceneName: instance2.GetSceneNameString());
		}
	}
}
