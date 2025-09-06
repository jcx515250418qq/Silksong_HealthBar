using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("iTween")]
	[Tooltip("Adds the supplied vector to a GameObject's position.")]
	public class iTweenMoveBy : iTweenFsmAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
		public FsmString id;

		[RequiredField]
		[Tooltip("The vector to add to the GameObject's position.")]
		public FsmVector3 vector;

		[Tooltip("For the time in seconds the animation will take to complete.")]
		public FsmFloat time;

		[Tooltip("For the time in seconds the animation will wait before beginning.")]
		public FsmFloat delay;

		[Tooltip("Can be used instead of time to allow animation based on speed. When you define speed the time variable is ignored.")]
		public FsmFloat speed;

		[Tooltip("For the shape of the easing curve applied to the animation.")]
		public iTween.EaseType easeType = iTween.EaseType.linear;

		[Tooltip("For the type of loop to apply once the animation has completed.")]
		public iTween.LoopType loopType;

		public Space space;

		[ActionSection("LookAt")]
		[Tooltip("For whether or not the GameObject will orient to its direction of travel. False by default.")]
		public FsmBool orientToPath;

		[Tooltip("For a target the GameObject will look at.")]
		public FsmGameObject lookAtObject;

		[Tooltip("For a target the GameObject will look at.")]
		public FsmVector3 lookAtVector;

		[Tooltip("For the time in seconds the object will take to look at either the 'looktarget' or 'orienttopath'. 0 by default")]
		public FsmFloat lookTime;

		[Tooltip("Restricts rotation to the supplied axis only. Just put there strinc like 'x' or 'xz'")]
		public AxisRestriction axis;

		public override void Reset()
		{
			base.Reset();
			id = new FsmString
			{
				UseVariable = true
			};
			time = 1f;
			delay = 0f;
			loopType = iTween.LoopType.none;
			vector = new FsmVector3
			{
				UseVariable = true
			};
			speed = new FsmFloat
			{
				UseVariable = true
			};
			space = Space.World;
			orientToPath = false;
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
				Hashtable hashtable = new Hashtable();
				hashtable.Add("amount", vector.IsNone ? Vector3.zero : vector.Value);
				hashtable.Add(speed.IsNone ? "time" : "speed", (!speed.IsNone) ? speed.Value : (time.IsNone ? 1f : time.Value));
				hashtable.Add("delay", delay.IsNone ? 0f : delay.Value);
				hashtable.Add("easetype", easeType);
				hashtable.Add("looptype", loopType);
				hashtable.Add("oncomplete", "iTweenOnComplete");
				hashtable.Add("oncompleteparams", itweenID);
				hashtable.Add("onstart", "iTweenOnStart");
				hashtable.Add("onstartparams", itweenID);
				hashtable.Add("ignoretimescale", !realTime.IsNone && realTime.Value);
				hashtable.Add("space", space);
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
				iTween.MoveBy(ownerDefaultTarget, hashtable);
			}
		}
	}
}
