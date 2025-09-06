using GlobalEnums;
using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class SetDeathRespawnV2 : FsmStateAction
{
	public FsmString RespawnMarkerName;

	public FsmInt RespawnType;

	public FsmBool RespawnFacingRight;

	[ObjectType(typeof(MapZone))]
	public FsmEnum RespawnMapZone;

	[ObjectType(typeof(ExtraRestZones))]
	public FsmEnum RespawnExtraRestZone;

	public override void Reset()
	{
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
		if (!RespawnMapZone.IsNone)
		{
			playerData.mapZone = (MapZone)(object)RespawnMapZone.Value;
		}
		if (!RespawnExtraRestZone.IsNone)
		{
			playerData.extraRestZone = (ExtraRestZones)(object)RespawnExtraRestZone.Value;
		}
		Finish();
	}
}
