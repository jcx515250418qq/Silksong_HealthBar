using System.Reflection;
using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class GGCountCompletedBossDoors : FSMUtility.GetIntFsmStateAction
{
	public override int IntValue
	{
		get
		{
			int num = 0;
			FieldInfo[] fields = typeof(PlayerData).GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.FieldType == typeof(BossSequenceDoor.Completion) && ((BossSequenceDoor.Completion)fieldInfo.GetValue(GameManager.instance.playerData)).completed)
				{
					num++;
				}
			}
			return num;
		}
	}
}
