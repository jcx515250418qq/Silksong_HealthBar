using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetHeroNailImbuement : FsmStateAction
	{
		public FsmOwnerDefault Target;

		[ObjectType(typeof(NailElements))]
		public FsmEnum Element;

		public override void Reset()
		{
			Target = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				HeroNailImbuement component = safe.GetComponent<HeroNailImbuement>();
				if ((bool)component)
				{
					component.SetElement((NailElements)(object)Element.Value);
				}
			}
			Finish();
		}
	}
}
