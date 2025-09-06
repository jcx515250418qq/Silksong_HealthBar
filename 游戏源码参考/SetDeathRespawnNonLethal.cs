using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class SetDeathRespawnNonLethal : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	public FsmGameObject gameManager;

	public FsmString respawnMarkerName;

	public FsmInt respawnType;

	public FsmBool respawnFacingRight;

	public override void Reset()
	{
		gameManager = new FsmGameObject();
	}

	public override void OnEnter()
	{
		GameManager instance = GameManager.instance;
		if (instance != null)
		{
			instance.SetNonlethalDeathRespawn(respawnMarkerName.Value, respawnType.Value, respawnFacingRight.Value);
		}
		Finish();
	}
}
