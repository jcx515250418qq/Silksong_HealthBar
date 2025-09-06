using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Captures the current pose of a hierarchy as an animation clip.\n\nUseful to blend from an arbitrary pose (e.g. a rag-doll death) back to a known animation (e.g. idle).")]
	public class CapturePoseAsAnimationClip : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("The GameObject root of the hierarchy to capture.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Capture position keys.")]
		public FsmBool position;

		[Tooltip("Capture rotation keys.")]
		public FsmBool rotation;

		[Tooltip("Capture scale keys.")]
		public FsmBool scale;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(AnimationClip))]
		[Tooltip("Store the result in an Object variable of type AnimationClip.")]
		public FsmObject storeAnimationClip;

		public override void Reset()
		{
			gameObject = null;
			position = false;
			rotation = true;
			scale = false;
			storeAnimationClip = null;
		}

		public override void OnEnter()
		{
			DoCaptureAnimationClip();
			Finish();
		}

		private void DoCaptureAnimationClip()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			AnimationClip animationClip = new AnimationClip();
			foreach (Transform item in ownerDefaultTarget.transform)
			{
				CaptureTransform(item, "", animationClip);
			}
			storeAnimationClip.Value = animationClip;
		}

		private void CaptureTransform(Transform transform, string path, AnimationClip clip)
		{
			path += transform.name;
			if (position.Value)
			{
				CapturePosition(transform, path, clip);
			}
			if (rotation.Value)
			{
				CaptureRotation(transform, path, clip);
			}
			if (scale.Value)
			{
				CaptureScale(transform, path, clip);
			}
			foreach (Transform item in transform)
			{
				CaptureTransform(item, path + "/", clip);
			}
		}

		private void CapturePosition(Transform transform, string path, AnimationClip clip)
		{
			SetConstantCurve(clip, path, "localPosition.x", transform.localPosition.x);
			SetConstantCurve(clip, path, "localPosition.y", transform.localPosition.y);
			SetConstantCurve(clip, path, "localPosition.z", transform.localPosition.z);
		}

		private void CaptureRotation(Transform transform, string path, AnimationClip clip)
		{
			SetConstantCurve(clip, path, "localRotation.x", transform.localRotation.x);
			SetConstantCurve(clip, path, "localRotation.y", transform.localRotation.y);
			SetConstantCurve(clip, path, "localRotation.z", transform.localRotation.z);
			SetConstantCurve(clip, path, "localRotation.w", transform.localRotation.w);
		}

		private void CaptureScale(Transform transform, string path, AnimationClip clip)
		{
			SetConstantCurve(clip, path, "localScale.x", transform.localScale.x);
			SetConstantCurve(clip, path, "localScale.y", transform.localScale.y);
			SetConstantCurve(clip, path, "localScale.z", transform.localScale.z);
		}

		private void SetConstantCurve(AnimationClip clip, string childPath, string propertyPath, float value)
		{
			AnimationCurve animationCurve = AnimationCurve.Linear(0f, value, 100f, value);
			animationCurve.postWrapMode = WrapMode.Loop;
			clip.SetCurve(childPath, typeof(Transform), propertyPath, animationCurve);
		}
	}
}
