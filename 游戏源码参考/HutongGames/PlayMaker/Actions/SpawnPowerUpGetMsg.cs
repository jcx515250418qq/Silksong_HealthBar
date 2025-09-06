namespace HutongGames.PlayMaker.Actions
{
	public class SpawnPowerUpGetMsg : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(PowerUpGetMsg))]
		public FsmGameObject MsgPrefab;

		[RequiredField]
		[ObjectType(typeof(PowerUpGetMsg.PowerUps))]
		public FsmEnum PowerUp;

		public override void Reset()
		{
			MsgPrefab = null;
			PowerUp = null;
		}

		public override void OnEnter()
		{
			PowerUpGetMsg.Spawn(MsgPrefab.Value.GetComponent<PowerUpGetMsg>(), (PowerUpGetMsg.PowerUps)(object)PowerUp.Value, base.Finish);
		}
	}
}
