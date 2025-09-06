using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	public class CountCrestUnlockPoints : FsmStateAction
	{
		[ObjectType(typeof(ToolCrestList))]
		public FsmObject CrestList;

		[UIHint(UIHint.Variable)]
		public FsmInt StoreCurrentPoints;

		[UIHint(UIHint.Variable)]
		public FsmInt StoreMaxPoints;

		public override void Reset()
		{
			base.Reset();
			CrestList = null;
			StoreCurrentPoints = null;
			StoreMaxPoints = null;
		}

		public override void OnEnter()
		{
			int num = 0;
			int num2 = 0;
			ToolCrestList toolCrestList = CrestList.Value as ToolCrestList;
			if (toolCrestList != null)
			{
				foreach (ToolCrest item in toolCrestList)
				{
					if (item.IsHidden || !item.IsBaseVersion || item.IsUpgradedVersionUnlocked)
					{
						continue;
					}
					ToolCrest.SlotInfo[] slots = item.Slots;
					for (int i = 0; i < slots.Length; i++)
					{
						_ = ref slots[i];
						num2++;
					}
					if (!item.IsUnlocked)
					{
						continue;
					}
					ToolCrest.SlotInfo[] slots2 = item.Slots;
					List<ToolCrestsData.SlotData> slots3 = item.SaveData.Slots;
					for (int j = 0; j < slots2.Length; j++)
					{
						if (!slots2[j].IsLocked || (slots3 != null && j < slots3.Count && slots3[j].IsUnlocked))
						{
							num++;
						}
					}
				}
			}
			StoreCurrentPoints.Value = num;
			StoreMaxPoints.Value = num2;
			Finish();
		}
	}
}
