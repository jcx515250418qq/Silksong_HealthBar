using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GetCostFromReference : FsmStateAction
	{
		[ObjectType(typeof(CostReference))]
		public FsmObject Reference;

		[UIHint(UIHint.Variable)]
		public FsmInt StoreValue;

		public override void Reset()
		{
			Reference = null;
			StoreValue = null;
		}

		public override void OnEnter()
		{
			CostReference costReference = Reference.Value as CostReference;
			if (costReference != null)
			{
				StoreValue.Value = costReference.Value;
			}
			else
			{
				Debug.LogError("Cost reference not assigned!", base.Owner);
			}
			Finish();
		}
	}
}
