using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public abstract class TweenComponentBase<T> : TweenActionBase where T : Component
	{
		[DisplayOrder(0)]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Game Object to tween.")]
		public FsmOwnerDefault gameObject;

		protected GameObject cachedGameObject;

		protected T cachedComponent;

		public override void Reset()
		{
			base.Reset();
			gameObject = null;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null)
			{
				Finish();
			}
			if (!UpdateCache(ownerDefaultTarget))
			{
				Finish();
			}
		}

		protected bool UpdateCache(GameObject go)
		{
			if (go == null)
			{
				return false;
			}
			if (cachedComponent == null || cachedGameObject != go)
			{
				cachedComponent = go.GetComponent<T>();
				cachedGameObject = go;
				if (cachedComponent == null)
				{
					LogWarning("Missing component: " + typeof(T).FullName + " on: " + go.name);
				}
			}
			return cachedComponent != null;
		}

		protected override void DoTween()
		{
			throw new NotImplementedException();
		}

		public override string ErrorCheck()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != null && ownerDefaultTarget.GetComponent<T>() == null)
			{
				if (typeof(T) == typeof(RectTransform))
				{
					return "This Tween only works with UI GameObjects";
				}
				return "GameObject missing component:\n" + typeof(T);
			}
			return "";
		}
	}
}
