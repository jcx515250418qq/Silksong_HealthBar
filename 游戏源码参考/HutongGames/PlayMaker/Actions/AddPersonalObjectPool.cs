using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class AddPersonalObjectPool : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault Target;

		[RequiredField]
		public FsmGameObject Prefab;

		[RequiredField]
		public FsmInt Amount;

		public FsmBool SharePooledInScene;

		public FsmBool FinalCall;

		public override void Reset()
		{
			Target = null;
			Prefab = null;
			Amount = null;
			SharePooledInScene = null;
			FinalCall = true;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (SharePooledInScene.Value)
			{
				PersonalObjectPool.EnsurePooledInScene(safe, Prefab.Value, Amount.Value, FinalCall.Value);
			}
			else
			{
				safe.AddComponentIfNotPresent<PersonalObjectPool>().startupPool.Add(new StartupPool(Amount.Value, Prefab.Value));
			}
			Finish();
		}
	}
}
