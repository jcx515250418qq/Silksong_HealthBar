namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	public class CompareHPBool : FsmStateAction
	{
		[RequiredField]
		public FsmGameObject enemy;

		public FsmInt compareTo;

		public FsmBool equalBool;

		public FsmBool lessThanBool;

		public FsmBool greaterThanBool;

		public bool everyFrame;

		private int hp;

		private HealthManager healthManager;

		public override void Reset()
		{
			hp = 0;
			compareTo = 0;
			equalBool = null;
			lessThanBool = null;
			greaterThanBool = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			healthManager = enemy.Value.GetComponent<HealthManager>();
			DoCompare();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoCompare();
		}

		private void DoCompare()
		{
			if (healthManager != null)
			{
				hp = healthManager.hp;
			}
			if (hp == compareTo.Value)
			{
				equalBool.Value = true;
			}
			else
			{
				equalBool.Value = false;
			}
			if (hp < compareTo.Value)
			{
				lessThanBool.Value = true;
			}
			else
			{
				lessThanBool.Value = false;
			}
			if (hp > compareTo.Value)
			{
				greaterThanBool.Value = true;
			}
			else
			{
				greaterThanBool.Value = false;
			}
		}
	}
}
