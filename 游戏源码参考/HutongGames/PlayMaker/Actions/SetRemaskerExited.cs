using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetRemaskerExited : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmBool WasDisabled;

		public FsmBool Recursive;

		public override void Reset()
		{
			Target = null;
			WasDisabled = null;
			Recursive = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				if (Recursive.Value)
				{
					Remasker[] componentsInChildren = safe.GetComponentsInChildren<Remasker>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].Exited(WasDisabled.Value);
					}
				}
				else
				{
					Remasker component = safe.GetComponent<Remasker>();
					if ((bool)component)
					{
						component.Exited(WasDisabled.Value);
					}
				}
			}
			Finish();
		}
	}
}
