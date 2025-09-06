using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Find overlaps with GameObject colliders in the scene.")]
	public class FindOverlaps : ComponentAction<Transform>
	{
		public enum Shape
		{
			Box = 0,
			Sphere = 1,
			Capsule = 2
		}

		[Tooltip("GameObject position to use for the test shape. Set to none to use world origin (0,0,0).")]
		public FsmOwnerDefault position;

		[Tooltip("Offset position of the shape.")]
		public FsmVector3 offset;

		[Tooltip("Shape to find overlaps against.")]
		public Shape shape;

		[HideIf("HideRadius")]
		[Tooltip("Radius of sphere/capsule.")]
		public FsmFloat radius;

		[HideIf("HideBox")]
		[Tooltip("Size of box.")]
		public FsmVector3 box;

		[HideIf("HideCapsule")]
		[Tooltip("The height of the capsule.")]
		public FsmFloat height;

		[Tooltip("Maximum number of overlaps to detect.")]
		public FsmInt maxOverlaps;

		[ActionSection("Filter")]
		[UIHint(UIHint.LayerMask)]
		[Tooltip("LayerMask name to filter the overlapping objects")]
		public FsmInt layerMask;

		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		[Tooltip("Include self in the array.")]
		public FsmBool includeSelf;

		[Tooltip("Set how often to cast a ray. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nBecause Overlaps can get expensive use the highest repeat interval you can get away with.")]
		public FsmInt repeatInterval;

		[ActionSection("Output")]
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
		[Tooltip("Store overlapping GameObjects in an array.")]
		public FsmArray storeOverlapping;

		[Tooltip("Event to send if overlaps were found.")]
		public FsmEvent foundOverlaps;

		[Tooltip("Event to send if no overlaps were found.")]
		public FsmEvent noOverlaps;

		[ActionSection("Debug")]
		[Tooltip("The color to use for the debug line.")]
		public FsmColor debugColor;

		[Tooltip("Draw a gizmo in the scene view to visualize the shape.")]
		public FsmBool debug;

		private Collider[] colliders;

		private int repeat;

		public Vector3 center { get; private set; }

		public Quaternion orientation { get; private set; }

		public Vector3 capsulePoint1 { get; private set; }

		public Vector3 capsulePoint2 { get; private set; }

		public int targetMask { get; private set; }

		public override void Reset()
		{
			position = null;
			offset = null;
			shape = Shape.Box;
			radius = new FsmFloat
			{
				Value = 1f
			};
			box = new FsmVector3
			{
				Value = new Vector3(1f, 1f, 1f)
			};
			height = new FsmFloat
			{
				Value = 1f
			};
			storeOverlapping = null;
			maxOverlaps = new FsmInt
			{
				Value = 50
			};
			repeatInterval = new FsmInt
			{
				Value = 1
			};
			foundOverlaps = null;
			includeSelf = null;
			layerMask = null;
			invertMask = null;
			noOverlaps = null;
			debugColor = new FsmColor
			{
				Value = Color.yellow
			};
			debug = null;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public bool HideBox()
		{
			return shape != Shape.Box;
		}

		public bool HideRadius()
		{
			if (shape != Shape.Sphere)
			{
				return shape != Shape.Capsule;
			}
			return false;
		}

		public bool HideCapsule()
		{
			return shape != Shape.Capsule;
		}

		public override void OnEnter()
		{
			colliders = new Collider[Mathf.Clamp(maxOverlaps.Value, 0, int.MaxValue)];
			DoGetOverlap();
			if (repeatInterval.Value == 0)
			{
				Finish();
			}
		}

		public override void OnFixedUpdate()
		{
			repeat--;
			if (repeat == 0)
			{
				DoGetOverlap();
			}
		}

		private void DoGetOverlap()
		{
			repeat = repeatInterval.Value;
			InitShapeCenter();
			targetMask = layerMask.Value;
			if (invertMask.Value)
			{
				targetMask = ~targetMask;
			}
			int num = 0;
			switch (shape)
			{
			case Shape.Box:
				num = Physics.OverlapBoxNonAlloc(center, box.Value / 2f, colliders, orientation, targetMask);
				break;
			case Shape.Sphere:
				num = Physics.OverlapSphereNonAlloc(center, radius.Value, colliders, targetMask);
				break;
			case Shape.Capsule:
				num = Physics.OverlapCapsuleNonAlloc(capsulePoint1, capsulePoint2, radius.Value, colliders, targetMask);
				break;
			}
			if (num == 0)
			{
				storeOverlapping.Values = new object[0];
			}
			else if (includeSelf.Value)
			{
				storeOverlapping.Values = new object[num];
				for (int i = 0; i < num; i++)
				{
					storeOverlapping.Values[i] = colliders[i].gameObject;
				}
			}
			else
			{
				List<object> list = new List<object>();
				for (int j = 0; j < num; j++)
				{
					GameObject gameObject = colliders[j].gameObject;
					if (gameObject != cachedGameObject)
					{
						list.Add(gameObject);
					}
				}
				storeOverlapping.Values = list.ToArray();
			}
			base.Fsm.Event((num > 0) ? foundOverlaps : noOverlaps);
		}

		public void InitShapeCenter()
		{
			center = offset.Value;
			orientation = Quaternion.identity;
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(position);
			if (UpdateCachedTransform(ownerDefaultTarget))
			{
				center = base.cachedTransform.TransformPoint(offset.Value);
				orientation = base.cachedTransform.rotation;
				if (shape == Shape.Capsule)
				{
					float num = height.Value / 2f - radius.Value;
					capsulePoint1 = base.cachedTransform.TransformPoint(new Vector3(0f, 0f - num, 0f));
					capsulePoint2 = base.cachedTransform.TransformPoint(new Vector3(0f, num, 0f));
				}
			}
		}
	}
}
