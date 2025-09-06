using System.Collections.Generic;
using TeamCherry.SharedUtils;

namespace HutongGames.PlayMaker.Actions
{
	public class SceneTravelerTempEval : FsmStateAction
	{
		public enum TravelerTypes
		{
			Mapper = 0
		}

		private class TempAreaBools
		{
			public string SeenBool;

			public string LeftBool;

			public TempAreaBools(string seenBool, string leftBool)
			{
				SeenBool = seenBool;
				LeftBool = leftBool;
			}
		}

		[ObjectType(typeof(TravelerTypes))]
		[RequiredField]
		public FsmEnum TravelerType;

		public FsmString SeenPDBool;

		public FsmEvent StillHereEvent;

		public FsmEvent LeftEvent;

		private readonly Dictionary<TravelerTypes, List<TempAreaBools>> travelerBools = new Dictionary<TravelerTypes, List<TempAreaBools>> { 
		{
			TravelerTypes.Mapper,
			new List<TempAreaBools>
			{
				new TempAreaBools("SeenMapperBoneForest", "MapperLeftBoneForest"),
				new TempAreaBools("SeenMapperDocks", "MapperLeftDocks"),
				new TempAreaBools("SeenMapperWilds", "MapperLeftWilds"),
				new TempAreaBools("SeenMapperCrawl", "MapperLeftCrawl"),
				new TempAreaBools("SeenMapperGreymoor", "MapperLeftGreymoor"),
				new TempAreaBools("SeenMapperBellhart", "MapperLeftBellhart"),
				new TempAreaBools("SeenMapperShellwood", "MapperLeftShellwood"),
				new TempAreaBools("SeenMapperHuntersNest", "MapperLeftHuntersNest"),
				new TempAreaBools("SeenMapperJudgeSteps", "MapperLeftJudgeSteps"),
				new TempAreaBools("SeenMapperDustpens", "MapperLeftDustpens"),
				new TempAreaBools("SeenMapperPeak", "MapperLeftPeak"),
				new TempAreaBools("SeenMapperShadow", "MapperLeftShadow"),
				new TempAreaBools("SeenMapperCoralCaverns", "MapperLeftCoralCaverns")
			}
		} };

		public override void Reset()
		{
			SeenPDBool = null;
			StillHereEvent = null;
			LeftEvent = null;
		}

		public override void OnEnter()
		{
			PlayerData instance = PlayerData.instance;
			string value = SeenPDBool.Value;
			TravelerTypes key = (TravelerTypes)(object)TravelerType.Value;
			if (travelerBools.ContainsKey(key))
			{
				List<TempAreaBools> list = travelerBools[key];
				foreach (TempAreaBools item in list)
				{
					if (item.SeenBool == value && instance.GetVariable<bool>(item.LeftBool))
					{
						base.Fsm.Event(LeftEvent);
						Finish();
						return;
					}
				}
				foreach (TempAreaBools item2 in list)
				{
					if (item2.SeenBool != value && instance.GetVariable<bool>(item2.SeenBool))
					{
						instance.SetVariable(item2.LeftBool, value: true);
					}
				}
			}
			base.Fsm.Event(StillHereEvent);
			Finish();
		}
	}
}
