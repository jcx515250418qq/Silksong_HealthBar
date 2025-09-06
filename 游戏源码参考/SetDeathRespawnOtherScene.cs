using GlobalEnums;
using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class SetDeathRespawnOtherScene : FsmStateAction
{
	public FsmString RespawnSceneName;

	public FsmString RespawnMarkerName;

	public FsmInt RespawnType;

	public FsmBool RespawnFacingRight;

	[ObjectType(typeof(MapZone))]
	public FsmEnum RespawnMapZone;

	[ObjectType(typeof(ExtraRestZones))]
	public FsmEnum RespawnExtraRestZone;

	public override void Reset()
	{
		RespawnSceneName = null;
		RespawnMarkerName = null;
		RespawnType = null;
		RespawnFacingRight = null;
		RespawnMapZone = new FsmEnum
		{
			UseVariable = true
		};
		RespawnExtraRestZone = new FsmEnum
		{
			UseVariable = true
		};
	}

	public override void OnEnter()
	{
		GameManager instance = GameManager.instance;
		PlayerData playerData = instance.playerData;
		instance.SetDeathRespawnSimple(RespawnMarkerName.Value, RespawnType.Value, RespawnFacingRight.Value);
		if (!RespawnSceneName.IsNone)
		{
			playerData.respawnScene = RespawnSceneName.Value;
		}
		if (!RespawnMapZone.IsNone)
		{
			playerData.mapZone = (MapZone)(object)RespawnMapZone.Value;
		}
		else
		{
			SceneTeleportMap.SceneInfo sceneInfo = SceneTeleportMap.GetTeleportMap()[RespawnSceneName.Value];
			if (sceneInfo != null)
			{
				playerData.mapZone = sceneInfo.MapZone;
			}
		}
		if (!RespawnExtraRestZone.IsNone)
		{
			playerData.extraRestZone = (ExtraRestZones)(object)RespawnExtraRestZone.Value;
		}
		Finish();
	}
}
