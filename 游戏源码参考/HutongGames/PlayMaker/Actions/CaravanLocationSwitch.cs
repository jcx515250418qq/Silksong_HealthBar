using System;
using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	public sealed class CaravanLocationSwitch : FsmStateAction
	{
		public class LocationSwitch
		{
			[ObjectType(typeof(CaravanTroupeLocations))]
			public FsmEnum Location;

			public FsmEvent MatchEvent;
		}

		public LocationSwitch[] Locations;

		public override void Reset()
		{
			Locations = Array.Empty<LocationSwitch>();
		}

		public override void OnEnter()
		{
			if (Locations.Length != 0 && GameManager.instance != null)
			{
				PlayerData playerData = GameManager.instance.playerData;
				if (playerData != null)
				{
					LocationSwitch[] locations = Locations;
					foreach (LocationSwitch locationSwitch in locations)
					{
						if (!locationSwitch.Location.IsNone && playerData.CaravanTroupeLocation == (CaravanTroupeLocations)(object)locationSwitch.Location.Value && locationSwitch.MatchEvent != null)
						{
							base.Fsm.Event(locationSwitch.MatchEvent);
							break;
						}
					}
				}
			}
			Finish();
		}
	}
}
