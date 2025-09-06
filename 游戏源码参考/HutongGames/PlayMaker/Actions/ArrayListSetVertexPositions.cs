using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Set mesh vertex positions based on vector3 found in an arrayList")]
	public class ArrayListSetVertexPositions : ArrayListActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[ActionSection("Target")]
		[Tooltip("The GameObject to set the mesh vertex positions to")]
		[CheckForComponent(typeof(MeshFilter))]
		public FsmGameObject mesh;

		public bool everyFrame;

		private Mesh _mesh;

		private Vector3[] _vertices;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			mesh = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			GameObject value = mesh.Value;
			if (value == null)
			{
				Finish();
				return;
			}
			MeshFilter component = value.GetComponent<MeshFilter>();
			if (component == null)
			{
				Finish();
				return;
			}
			_mesh = component.mesh;
			if (SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
			{
				SetVertexPositions();
			}
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			SetVertexPositions();
		}

		public void SetVertexPositions()
		{
			if (!isProxyValid())
			{
				return;
			}
			_vertices = new Vector3[proxy.arrayList.Count];
			int num = 0;
			foreach (Vector3 array in proxy.arrayList)
			{
				_vertices[num] = array;
				num++;
			}
			_mesh.vertices = _vertices;
		}
	}
}
