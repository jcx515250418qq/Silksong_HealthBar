using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("GameObject")]
	[Tooltip("Set Mesh Renderer to active or inactive. Can only be one Mesh Renderer on object. ")]
	public class GetMeshRendererBounds : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmFloat width;

		[UIHint(UIHint.Variable)]
		public FsmFloat height;

		public FsmFloat widthMax;

		public FsmFloat heightMax;

		public override void Reset()
		{
			gameObject = null;
			width = null;
			height = null;
			widthMax = null;
			heightMax = null;
		}

		public override void OnEnter()
		{
			if (gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				MeshRenderer component = ownerDefaultTarget.GetComponent<MeshRenderer>();
				if (component != null)
				{
					width.Value = component.bounds.size.x;
				}
				foreach (Transform item in ownerDefaultTarget.transform)
				{
					MeshRenderer component2 = item.GetComponent<MeshRenderer>();
					if (component2 != null)
					{
						float x = component2.bounds.size.x;
						float y = component2.bounds.size.y;
						if (x > width.Value)
						{
							width.Value = x;
						}
						if (y > height.Value)
						{
							height.Value = y;
						}
					}
				}
				if (!widthMax.IsNone && width.Value > widthMax.Value)
				{
					width.Value = 0f;
				}
				height.Value = component.bounds.size.y;
				if (!heightMax.IsNone && height.Value > heightMax.Value)
				{
					height.Value = 0f;
				}
			}
			Finish();
		}
	}
}
