using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Draw a debug Gizmo.\nNote: you can enable/disable Gizmos in the Game View toolbar.")]
	public class DebugDrawShape : ComponentAction<Transform>
	{
		public enum ShapeType
		{
			Sphere = 0,
			Cube = 1,
			WireSphere = 2,
			WireCube = 3
		}

		[RequiredField]
		[Tooltip("Draw the Gizmo at a GameObject's position.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The type of Gizmo to draw:\nSphere, Cube, WireSphere, or WireCube.")]
		public ShapeType shape;

		[Tooltip("The color to use.")]
		public FsmColor color;

		[HideIf("HideRadius")]
		[Tooltip("Use this for sphere gizmos")]
		public FsmFloat radius;

		[HideIf("HideSize")]
		[Tooltip("Use this for cube gizmos")]
		public FsmVector3 size;

		public bool HideRadius()
		{
			if (shape != 0)
			{
				return shape != ShapeType.WireSphere;
			}
			return false;
		}

		public bool HideSize()
		{
			if (shape != ShapeType.Cube)
			{
				return shape != ShapeType.WireCube;
			}
			return false;
		}

		public override void Reset()
		{
			gameObject = null;
			shape = ShapeType.Sphere;
			color = new FsmColor
			{
				Value = Color.grey
			};
			radius = new FsmFloat
			{
				Value = 1f
			};
			size = new Vector3(1f, 1f, 1f);
		}

		public override void Awake()
		{
			base.BlocksFinish = false;
		}

		public override void OnDrawActionGizmos()
		{
			if (base.Fsm == null)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCachedTransform(ownerDefaultTarget))
			{
				Gizmos.color = color.Value;
				switch (shape)
				{
				case ShapeType.Sphere:
					Gizmos.DrawSphere(base.cachedTransform.position, radius.Value);
					break;
				case ShapeType.WireSphere:
					Gizmos.DrawWireSphere(base.cachedTransform.position, radius.Value);
					break;
				case ShapeType.Cube:
					Gizmos.DrawCube(base.cachedTransform.position, size.Value);
					break;
				case ShapeType.WireCube:
					Gizmos.DrawWireCube(base.cachedTransform.position, size.Value);
					break;
				}
			}
		}
	}
}
