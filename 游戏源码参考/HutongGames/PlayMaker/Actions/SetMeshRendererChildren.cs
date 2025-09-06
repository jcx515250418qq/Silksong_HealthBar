using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("GameObject")]
	[Tooltip("Set Mesh Renderer of object's children to active or inactive. Can only be one Mesh Renderer on each object. ")]
	public class SetMeshRendererChildren : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmBool active;

		public override void Reset()
		{
			gameObject = null;
			active = false;
		}

		public override void OnEnter()
		{
			if (gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if (ownerDefaultTarget != null)
				{
					foreach (Transform item in ownerDefaultTarget.transform)
					{
						MeshRenderer component = item.GetComponent<MeshRenderer>();
						if (component != null)
						{
							component.enabled = active.Value;
						}
					}
				}
			}
			Finish();
		}
	}
}
