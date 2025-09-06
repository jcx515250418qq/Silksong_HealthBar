using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class SetDeathRespawn : FsmStateAction
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
		if (gameManager.Value != null)
		{
			GameManager component = gameManager.Value.GetComponent<GameManager>();
			if (component != null)
			{
				component.SetDeathRespawnSimple(respawnMarkerName.Value, respawnType.Value, respawnFacingRight.Value);
			}
			Finish();
		}
	}
}
