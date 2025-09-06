namespace HutongGames.PlayMaker.Actions
{
	public class RecordPlayerStory : FsmStateAction
	{
		[ObjectType(typeof(PlayerStory.EventTypes))]
		public FsmEnum EventType;

		public override void Reset()
		{
			EventType = null;
		}

		public override void OnEnter()
		{
			PlayerStory.RecordEvent((PlayerStory.EventTypes)(object)EventType.Value);
			Finish();
		}
	}
}
