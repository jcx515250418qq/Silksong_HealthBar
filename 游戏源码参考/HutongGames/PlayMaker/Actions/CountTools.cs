using System.Collections.Generic;
using System.Linq;

namespace HutongGames.PlayMaker.Actions
{
	public class CountTools : FsmStateAction
	{
		public FsmBool UnlockedOnly;

		[ObjectType(typeof(ToolDamageFlags))]
		public FsmEnum DamageFlag;

		[UIHint(UIHint.Variable)]
		public FsmInt StoreCount;

		public override void Reset()
		{
			UnlockedOnly = null;
			DamageFlag = null;
			StoreCount = null;
		}

		public override void OnEnter()
		{
			IEnumerable<ToolItem> source = (UnlockedOnly.Value ? ToolItemManager.GetUnlockedTools() : ToolItemManager.GetAllTools());
			ToolDamageFlags flag = (ToolDamageFlags)(object)DamageFlag.Value;
			if (flag != 0)
			{
				source = source.Where((ToolItem tool) => (tool.DamageFlags & flag) != 0);
			}
			StoreCount.Value = source.Count();
			Finish();
		}
	}
}
