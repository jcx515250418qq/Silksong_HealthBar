using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("iTween")]
	[Tooltip("Changes a GameObject's position over time to a supplied destination.")]
	public class iTweenMoveTo : iTweenFsmAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
		public FsmString id;

		[Tooltip("Move To a transform position.")]
		public FsmGameObject transformPosition;

		[Tooltip("Position the GameObject will animate to. If Transform Position is defined this is used as a local offset.")]
		public FsmVector3 vectorPosition;

		[Tooltip("The time in seconds the animation will take to complete.")]
		public FsmFloat time;

		[Tooltip("The time in seconds the animation will wait before beginning.")]
		public FsmFloat delay;

		[Tooltip("Can be used instead of time to allow animation based on speed. When you define speed the time variable is ignored.")]
		public FsmFloat speed;

		[Tooltip("Whether to animate in local or world space.")]
		public Space space;

		[Tooltip("The shape of the easing curve applied to the animation.")]
		public iTween.EaseType easeType = iTween.EaseType.linear;

		[Tooltip("The type of loop to apply once the animation has completed.")]
		public iTween.LoopType loopType;

		[ActionSection("LookAt")]
		[Tooltip("Whether or not the GameObject will orient to its direction of travel. False by default.")]
		public FsmBool orientToPath;

		[Tooltip("A target object the GameObject will look at.")]
		public FsmGameObject lookAtObject;

		[Tooltip("A target position the GameObject will look at.")]
		public FsmVector3 lookAtVector;

		[Tooltip("The time in seconds the object will take to look at either the Look Target or Orient To Path. 0 by default")]
		public FsmFloat lookTime;

		[Tooltip("Restricts rotation to the supplied axis only.")]
		public AxisRestriction axis;

		[ActionSection("Path")]
		[Tooltip("Whether to automatically generate a curve from the GameObject's current position to the beginning of the path. True by default.")]
		public FsmBool moveToPath;

		[Tooltip("How much of a percentage (from 0 to 1) to look ahead on a path to influence how strict Orient To Path is and how much the object will anticipate each curve.")]
		public FsmFloat lookAhead;

		[CompoundArray("Path Nodes", "Transform", "Vector")]
		[Tooltip("A list of objects to draw a Catmull-Rom spline through for a curved animation path.")]
		public FsmGameObject[] transforms;

		[Tooltip("A list of positions to draw a Catmull-Rom through for a curved animation path. If Transform is defined, this value is added as a local offset.")]
		public FsmVector3[] vectors;

		[Tooltip("Reverse the path so object moves from End to Start node.")]
		public FsmBool reverse;

		private Vector3[] tempVct3;

		public override void OnDrawActionGizmos()
		{
			if (transforms.Length < 2)
			{
				return;
			}
			tempVct3 = new Vector3[transforms.Length];
			for (int i = 0; i < transforms.Length; i++)
			{
				if (transforms[i].IsNone)
				{
					tempVct3[i] = (vectors[i].IsNone ? Vector3.zero : vectors[i].Value);
				}
				else if (transforms[i].Value == null)
				{
					tempVct3[i] = (vectors[i].IsNone ? Vector3.zero : vectors[i].Value);
				}
				else
				{
					tempVct3[i] = transforms[i].Value.transform.position + (vectors[i].IsNone ? Vector3.zero : vectors[i].Value);
				}
			}
			iTween.DrawPathGizmos(tempVct3, Color.yellow);
		}

		public override void Reset()
		{
			base.Reset();
			id = new FsmString
			{
				UseVariable = true
			};
			transformPosition = new FsmGameObject
			{
				UseVariable = true
			};
			vectorPosition = new FsmVector3
			{
				UseVariable = true
			};
			time = 1f;
			delay = 0f;
			loopType = iTween.LoopType.none;
			speed = new FsmFloat
			{
				UseVariable = true
			};
			space = Space.World;
			orientToPath = new FsmBool
			{
				Value = true
			};
			lookAtObject = new FsmGameObject
			{
				UseVariable = true
			};
			lookAtVector = new FsmVector3
			{
				UseVariable = true
			};
			lookTime = new FsmFloat
			{
				UseVariable = true
			};
			moveToPath = true;
			lookAhead = new FsmFloat
			{
				UseVariable = true
			};
			transforms = new FsmGameObject[0];
			vectors = new FsmVector3[0];
			tempVct3 = new Vector3[0];
			axis = AxisRestriction.none;
			reverse = false;
		}

		public override void OnEnter()
		{
			OnEnteriTween(gameObject);
			if (loopType != 0)
			{
				IsLoop(aValue: true);
			}
			DoiTween();
		}

		public override void OnExit()
		{
			OnExitiTween(gameObject);
		}

		private void DoiTween()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 vector = (vectorPosition.IsNone ? Vector3.zero : vectorPosition.Value);
			if (!transformPosition.IsNone && (bool)transformPosition.Value)
			{
				vector = ((space == Space.World || ownerDefaultTarget.transform.parent == null) ? (transformPosition.Value.transform.position + vector) : (ownerDefaultTarget.transform.parent.InverseTransformPoint(transformPosition.Value.transform.position) + vector));
			}
			Hashtable hashtable = new Hashtable();
			hashtable.Add("position", vector);
			hashtable.Add(speed.IsNone ? "time" : "speed", (!speed.IsNone) ? speed.Value : (time.IsNone ? 1f : time.Value));
			hashtable.Add("delay", delay.IsNone ? 0f : delay.Value);
			hashtable.Add("easetype", easeType);
			hashtable.Add("looptype", loopType);
			hashtable.Add("oncomplete", "iTweenOnComplete");
			hashtable.Add("oncompleteparams", itweenID);
			hashtable.Add("onstart", "iTweenOnStart");
			hashtable.Add("onstartparams", itweenID);
			hashtable.Add("ignoretimescale", !realTime.IsNone && realTime.Value);
			hashtable.Add("name", id.IsNone ? "" : id.Value);
			hashtable.Add("islocal", space == Space.Self);
			hashtable.Add("axis", (axis == AxisRestriction.none) ? "" : Enum.GetName(typeof(AxisRestriction), axis));
			if (!orientToPath.IsNone)
			{
				hashtable.Add("orienttopath", orientToPath.Value);
			}
			if (!lookAtObject.IsNone)
			{
				hashtable.Add("looktarget", lookAtVector.IsNone ? lookAtObject.Value.transform.position : (lookAtObject.Value.transform.position + lookAtVector.Value));
			}
			else if (!lookAtVector.IsNone)
			{
				hashtable.Add("looktarget", lookAtVector.Value);
			}
			if (!lookAtObject.IsNone || !lookAtVector.IsNone)
			{
				hashtable.Add("looktime", lookTime.IsNone ? 0f : lookTime.Value);
			}
			if (transforms.Length >= 2)
			{
				tempVct3 = new Vector3[transforms.Length];
				if (!reverse.IsNone && reverse.Value)
				{
					for (int i = 0; i < transforms.Length; i++)
					{
						if (transforms[i].IsNone)
						{
							tempVct3[tempVct3.Length - 1 - i] = (vectors[i].IsNone ? Vector3.zero : vectors[i].Value);
						}
						else if (transforms[i].Value == null)
						{
							tempVct3[tempVct3.Length - 1 - i] = (vectors[i].IsNone ? Vector3.zero : vectors[i].Value);
						}
						else
						{
							tempVct3[tempVct3.Length - 1 - i] = ((space == Space.World) ? transforms[i].Value.transform.position : transforms[i].Value.transform.localPosition) + (vectors[i].IsNone ? Vector3.zero : vectors[i].Value);
						}
					}
				}
				else
				{
					for (int j = 0; j < transforms.Length; j++)
					{
						if (transforms[j].IsNone)
						{
							tempVct3[j] = (vectors[j].IsNone ? Vector3.zero : vectors[j].Value);
						}
						else if (transforms[j].Value == null)
						{
							tempVct3[j] = (vectors[j].IsNone ? Vector3.zero : vectors[j].Value);
						}
						else
						{
							tempVct3[j] = ((space == Space.World) ? transforms[j].Value.transform.position : ownerDefaultTarget.transform.parent.InverseTransformPoint(transforms[j].Value.transform.position)) + (vectors[j].IsNone ? Vector3.zero : vectors[j].Value);
						}
					}
				}
				hashtable.Add("path", tempVct3);
				hashtable.Add("movetopath", moveToPath.IsNone || moveToPath.Value);
				hashtable.Add("lookahead", lookAhead.IsNone ? 1f : lookAhead.Value);
			}
			itweenType = "move";
			iTween.MoveTo(ownerDefaultTarget, hashtable);
		}
	}
}
