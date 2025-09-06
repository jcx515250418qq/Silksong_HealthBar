using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetInteractableLabel : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(InteractableBase))]
		public FsmOwnerDefault Target;

		[ObjectType(typeof(InteractableBase.PromptLabels))]
		public FsmEnum Label;

		public FsmBool UseChildren;

		public override void Reset()
		{
			Target = null;
			Label = null;
			UseChildren = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (!safe)
			{
				return;
			}
			if (!UseChildren.Value)
			{
				InteractableBase component = safe.GetComponent<InteractableBase>();
				if ((bool)component)
				{
					component.InteractLabel = (InteractableBase.PromptLabels)(object)Label.Value;
				}
			}
			else
			{
				InteractableBase[] componentsInChildren = safe.GetComponentsInChildren<InteractableBase>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].InteractLabel = (InteractableBase.PromptLabels)(object)Label.Value;
				}
			}
			Finish();
		}
	}
}
