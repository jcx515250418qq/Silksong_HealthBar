using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GameObjectIsActive : FSMUtility.CheckFsmStateAction
	{
		public FsmOwnerDefault Target;

		public Space ActiveSpace;

		public override bool IsTrue
		{
			get
			{
				GameObject safe = Target.GetSafe(this);
				if (!safe)
				{
					return false;
				}
				if (ActiveSpace != Space.Self)
				{
					return safe.activeInHierarchy;
				}
				return safe.activeSelf;
			}
		}

		public override void Reset()
		{
			base.Reset();
			Target = null;
			ActiveSpace = Space.Self;
		}
	}
}
