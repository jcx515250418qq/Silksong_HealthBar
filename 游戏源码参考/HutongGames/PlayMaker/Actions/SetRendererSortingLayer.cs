using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetRendererSortingLayer : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmString SortingLayerName;

		public FsmInt SortingOrder;

		public override void Reset()
		{
			Target = null;
			SortingLayerName = null;
			SortingOrder = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				Renderer component = safe.GetComponent<Renderer>();
				if ((bool)component)
				{
					component.sortingLayerName = SortingLayerName.Value;
					component.sortingOrder = SortingOrder.Value;
				}
			}
			Finish();
		}
	}
}
