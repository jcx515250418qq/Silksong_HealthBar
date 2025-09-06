using System.Linq;

namespace HutongGames.PlayMaker.Actions
{
	public class StoreTrackTriggerListAsArray : FsmStateAction
	{
		[CheckForComponent(typeof(TrackTriggerObjects))]
		public FsmGameObject TrackTrigger;

		[UIHint(UIHint.Variable)]
		public FsmArray Array;

		private TrackTriggerObjects track;

		public override void Reset()
		{
			TrackTrigger = null;
			Array = null;
		}

		public override void OnEnter()
		{
			TrackTriggerObjects component = TrackTrigger.Value.GetComponent<TrackTriggerObjects>();
			FsmArray array = Array;
			object[] values = component.InsideGameObjects.ToArray();
			array.Values = values;
			Finish();
		}
	}
}
