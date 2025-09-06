using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("iTween")]
	[Tooltip("Instantly changes a GameObject's position to a supplied destination then returns it to it's starting position over time.")]
	public class iTweenMoveFrom : iTweenFsmAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
		public FsmString id;

		[Tooltip("Move From a transform rotation.")]
		public FsmGameObject transformPosition;

		[Tooltip("The position the GameObject will animate from. If Transform Position is defined this is used as a local offset.")]
		public FsmVector3 vectorPosition;

		[Tooltip("The time in seconds the animation will take to complete.")]
		public FsmFloat time;

		[Tooltip("The time in seconds the animation will wait before beginning.")]
		public FsmFloat delay;

		[Tooltip("Can be used instead of time to allow animation based on speed. When you define speed the time variable is ignored.")]
		public FsmFloat speed;

		[Tooltip("The shape of the easing curve applied to the animation.")]
		public iTween.EaseType easeType = iTween.EaseType.linear;

		[Tooltip("The type of loop to apply once the animation has completed.")]
		public iTween.LoopType loopType;

		[Tooltip("Whether to animate in local or world space.")]
		public Space space;

		[ActionSection("LookAt")]
		[Tooltip("Whether or not the GameObject will orient to its direction of travel. False by default.")]
		public FsmBool orientToPath;

		[Tooltip("A target object the GameObject will look at.")]
		public FsmGameObject lookAtObject;

		[Tooltip("A target position the GameObject will look at.")]
		public FsmVector3 lookAtVector;

		[Tooltip("The time in seconds the object will take to look at either the Look At Target or Orient To Path. 0 by default")]
		public FsmFloat lookTime;

		[Tooltip("Restricts rotation to the supplied axis only.")]
		public AxisRestriction axis;

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
			lookTime = 0f;
			axis = AxisRestriction.none;
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
			if (!(ownerDefaultTarget == null))
			{
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
				hashtable.Add("islocal", space == Space.Self);
				hashtable.Add("name", id.IsNone ? "" : id.Value);
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
				itweenType = "move";
				iTween.MoveFrom(ownerDefaultTarget, hashtable);
			}
		}
	}
}
