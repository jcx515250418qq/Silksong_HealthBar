using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Rotates a 2d Game Object on it's z axis so its forward vector points at a Target. Rotates th eobject per frame via speed.")]
	public class LookAt2dGameObjectSmooth : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The GameObject to Look At.")]
		public FsmGameObject targetObject;

		[Tooltip("Set the GameObject starting offset. In degrees. 0 if your object is facing right, 180 if facing left etc...")]
		public FsmFloat rotationOffset;

		[RequiredField]
		[Tooltip("Speed the object rotates at to meet its target angle (in degrees per frame).")]
		public FsmFloat speed;

		[Title("Draw Debug Line")]
		[Tooltip("Draw a debug line from the GameObject to the Target.")]
		public FsmBool debug;

		[Tooltip("Color to use for the debug line.")]
		public FsmColor debugLineColor;

		private GameObject go;

		private GameObject goTarget;

		private Vector3 lookAtPos;

		public override void Reset()
		{
			gameObject = null;
			targetObject = null;
			debug = false;
			debugLineColor = Color.green;
		}

		public override void OnEnter()
		{
			DoLookAt();
		}

		public override void OnUpdate()
		{
			DoLookAt();
		}

		private void DoLookAt()
		{
			go = base.Fsm.GetOwnerDefaultTarget(gameObject);
			goTarget = targetObject.Value;
			if (go == null || targetObject == null)
			{
				return;
			}
			float y = goTarget.transform.position.y - go.transform.position.y;
			float x = goTarget.transform.position.x - go.transform.position.x;
			float num;
			for (num = Mathf.Atan2(y, x) * (180f / MathF.PI); num < 0f; num += 360f)
			{
			}
			if (go.transform.eulerAngles.z < num - rotationOffset.Value)
			{
				go.transform.Rotate(0f, 0f, speed.Value);
				if (go.transform.eulerAngles.z > num - rotationOffset.Value)
				{
					go.transform.rotation = Quaternion.Euler(0f, 0f, num - rotationOffset.Value);
				}
			}
			if (go.transform.eulerAngles.z > num - rotationOffset.Value)
			{
				go.transform.Rotate(0f, 0f, 0f - speed.Value);
				if (go.transform.eulerAngles.z < num - rotationOffset.Value)
				{
					go.transform.rotation = Quaternion.Euler(0f, 0f, num - rotationOffset.Value);
				}
			}
			if (debug.Value)
			{
				Debug.DrawLine(go.transform.position, goTarget.transform.position, debugLineColor.Value);
			}
		}
	}
}
