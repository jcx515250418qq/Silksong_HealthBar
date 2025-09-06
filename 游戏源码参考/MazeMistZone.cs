public class MazeMistZone : SceneTransitionZoneBase
{
	protected override string TargetScene => "Dust_Maze_09_entrance";

	protected override string TargetGate => "Death Respawn Marker";

	protected override void OnPreTransition()
	{
		base.OnPreTransition();
		MazeController.ResetSaveData();
		PlayerData instance = PlayerData.instance;
		instance.tempRespawnType = 0;
		instance.tempRespawnMarker = "Death Respawn Marker";
		GameManager instance2 = GameManager.instance;
		instance2.RespawningHero = true;
		instance2.TimePasses();
	}
}
