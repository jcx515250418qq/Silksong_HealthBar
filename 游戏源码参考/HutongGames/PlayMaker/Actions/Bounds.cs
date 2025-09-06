using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("gets the local or gloabal bounding box measures")]
	public class Bounds : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject1;

		[Tooltip("gets the local or global bounding box scale")]
		public FsmVector3 scale;

		[Tooltip("Should the scale be global? If it's rotated you probably want local axis for the scale")]
		public bool global;

		public bool everyFrame;

		public override void Reset()
		{
			gameObject1 = null;
			everyFrame = false;
			global = false;
		}

		public void GetEm()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject1);
			if (global)
			{
				scale.Value = ownerDefaultTarget.GetComponent<Renderer>().bounds.size;
				return;
			}
			Mesh sharedMesh = ownerDefaultTarget.GetComponent<MeshFilter>().sharedMesh;
			scale.Value = sharedMesh.bounds.size;
		}

		public override void OnEnter()
		{
			GetEm();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			GetEm();
		}
	}
}
