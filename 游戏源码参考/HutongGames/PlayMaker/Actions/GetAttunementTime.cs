using GlobalSettings;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	public class GetAttunementTime : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeVariable;

		public FsmFloat baseTime;

		public FsmFloat extraTime;

		public override void Reset()
		{
			storeVariable = null;
			baseTime = 0f;
			extraTime = 0f;
		}

		public override void OnEnter()
		{
			GetTime();
		}

		private void GetTime()
		{
			int num = GameManager.instance.playerData.attunementLevel;
			if (Gameplay.MusicianCharmTool.IsEquipped)
			{
				num++;
			}
			float num2 = baseTime.Value;
			switch (num)
			{
			case 2:
				num2 *= 0.9f;
				break;
			case 3:
				num2 *= 0.75f;
				break;
			case 4:
				num2 *= 0.6f;
				break;
			case 5:
				num2 *= 0.5f;
				break;
			case 6:
				num2 *= 0.45f;
				break;
			}
			num2 += extraTime.Value;
			storeVariable.Value = num2;
		}
	}
}
