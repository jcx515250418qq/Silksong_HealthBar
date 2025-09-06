using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	public class GetCaravanLocationScene : FsmStateAction
	{
		[ObjectType(typeof(CaravanTroupeLocations))]
		public FsmEnum Location;

		[UIHint(UIHint.Variable)]
		public FsmString StoreSceneName;

		public FsmEvent InvalidEvent;

		public override void Reset()
		{
			Location = null;
			StoreSceneName = null;
			InvalidEvent = null;
		}

		public override void OnEnter()
		{
			if (!Location.IsNone)
			{
				string sceneName = CaravanLocationScenes.GetSceneName((CaravanTroupeLocations)(object)Location.Value);
				StoreSceneName.Value = sceneName;
				if (string.IsNullOrEmpty(sceneName))
				{
					base.Fsm.Event(InvalidEvent);
				}
			}
			Finish();
		}
	}
}
