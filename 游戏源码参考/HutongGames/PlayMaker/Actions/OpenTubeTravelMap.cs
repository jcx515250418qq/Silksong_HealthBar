using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	public class OpenTubeTravelMap : FsmStateAction
	{
		public FsmGameObject SpawnedMap;

		public FsmEvent ClosedEvent;

		[ObjectType(typeof(TubeTravelLocations))]
		[UIHint(UIHint.Variable)]
		public FsmEnum StoreLocation;

		[ObjectType(typeof(TubeTravelLocations))]
		public FsmEnum ThisLocation;

		private TubeTravelMap spawnedMap;

		public override void Reset()
		{
			SpawnedMap = null;
			ClosedEvent = null;
			StoreLocation = null;
			ThisLocation = null;
		}

		public override void OnEnter()
		{
			if ((bool)SpawnedMap.Value)
			{
				spawnedMap = SpawnedMap.Value.GetComponent<TubeTravelMap>();
			}
			if ((bool)spawnedMap)
			{
				spawnedMap.LocationConfirmed += OnLocationConfirmed;
				spawnedMap.PaneClosed += OnPaneClosed;
				spawnedMap.AutoSelectLocation = (TubeTravelLocations)(object)ThisLocation.Value;
				spawnedMap.Open();
			}
			else
			{
				Finish();
			}
		}

		private void OnLocationConfirmed(TubeTravelLocations targetLocation)
		{
			spawnedMap.LocationConfirmed -= OnLocationConfirmed;
			StoreLocation.Value = targetLocation;
		}

		private void OnPaneClosed()
		{
			spawnedMap.PaneClosed -= OnPaneClosed;
			base.Fsm.Event(ClosedEvent);
			Finish();
		}
	}
}
