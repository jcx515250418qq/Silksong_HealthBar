namespace HutongGames.PlayMaker.Actions
{
	public class CheckHeroConfig : FSMUtility.CheckFsmStateAction
	{
		[ObjectType(typeof(HeroControllerConfig))]
		public FsmObject Config;

		public override bool IsTrue
		{
			get
			{
				HeroControllerConfig heroControllerConfig = Config.Value as HeroControllerConfig;
				if (!heroControllerConfig)
				{
					return false;
				}
				HeroController instance = HeroController.instance;
				if (!instance)
				{
					return false;
				}
				HeroControllerConfig config = instance.Config;
				if (!config)
				{
					return false;
				}
				return config == heroControllerConfig;
			}
		}

		public override void Reset()
		{
			base.Reset();
			Config = null;
		}
	}
}
