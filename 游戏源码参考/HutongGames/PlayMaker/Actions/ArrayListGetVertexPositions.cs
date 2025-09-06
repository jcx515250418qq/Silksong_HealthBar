using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Store mesh vertex positions into an arrayList")]
	public class ArrayListGetVertexPositions : ArrayListActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[ActionSection("Source")]
		[Tooltip("the GameObject to get the mesh from")]
		[CheckForComponent(typeof(MeshFilter))]
		public FsmGameObject mesh;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			mesh = null;
		}

		public override void OnEnter()
		{
			if (SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
			{
				getVertexPositions();
			}
			Finish();
		}

		public void getVertexPositions()
		{
			if (!isProxyValid())
			{
				return;
			}
			proxy.arrayList.Clear();
			GameObject value = mesh.Value;
			if (!(value == null))
			{
				MeshFilter component = value.GetComponent<MeshFilter>();
				if (!(component == null))
				{
					proxy.arrayList.InsertRange(0, component.mesh.vertices);
				}
			}
		}
	}
}
