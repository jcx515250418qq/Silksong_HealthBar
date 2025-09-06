using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GetDamageFromReference : FsmStateAction
	{
		[ObjectType(typeof(DamageReference))]
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
			DamageReference damageReference = Reference.Value as DamageReference;
			if (damageReference != null)
			{
				StoreValue.Value = damageReference.Value;
			}
			else
			{
				Debug.LogError("Damage reference not assigned!", base.Owner);
			}
			Finish();
		}
	}
}
