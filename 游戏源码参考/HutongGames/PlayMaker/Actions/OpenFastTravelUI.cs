using GlobalEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class OpenFastTravelUI : FsmStateAction
	{
		public FsmGameObject SpawnedUI;

		public FsmEvent ClosedEvent;

		[ObjectType(typeof(FastTravelLocations))]
		[UIHint(UIHint.Variable)]
		public FsmEnum StoreLocation;

		[ObjectType(typeof(FastTravelLocations))]
		public FsmEnum ThisLocation;

		[ObjectType(typeof(AudioClip))]
		[UIHint(UIHint.Variable)]
		public FsmObject StoreDestinationTune;

		private FastTravelMap spawnedMap;

		public override void Reset()
		{
			SpawnedUI = null;
			ClosedEvent = null;
			StoreLocation = null;
			ThisLocation = null;
			StoreDestinationTune = null;
		}

		public override void OnEnter()
		{
			if ((bool)SpawnedUI.Value)
			{
				spawnedMap = SpawnedUI.Value.GetComponent<FastTravelMap>();
			}
			if ((bool)spawnedMap)
			{
				spawnedMap.LocationConfirmed += OnLocationConfirmed;
				spawnedMap.PaneClosed += OnPaneClosed;
				spawnedMap.AutoSelectLocation = (FastTravelLocations)(object)ThisLocation.Value;
				spawnedMap.Open();
			}
			else
			{
				Finish();
			}
		}

		private void OnLocationConfirmed(FastTravelLocations targetLocation)
		{
			spawnedMap.LocationConfirmed -= OnLocationConfirmed;
			StoreLocation.Value = targetLocation;
			StoreDestinationTune.Value = null;
		}

		private void OnPaneClosed()
		{
			spawnedMap.PaneClosed -= OnPaneClosed;
			base.Fsm.Event(ClosedEvent);
			Finish();
		}
	}
}
