using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public abstract class ComponentAction<T> : FsmStateAction where T : Component
	{
		protected GameObject cachedGameObject;

		protected T cachedComponent;

		protected int cacheVersion;

		public Transform cachedTransform { get; private set; }

		protected Rigidbody rigidbody => cachedComponent as Rigidbody;

		protected Rigidbody2D rigidbody2d => cachedComponent as Rigidbody2D;

		protected Renderer renderer => cachedComponent as Renderer;

		protected Animation animation => cachedComponent as Animation;

		protected AudioSource audio => cachedComponent as AudioSource;

		protected Camera camera => cachedComponent as Camera;

		protected Light light => cachedComponent as Light;

		protected bool UpdateCache(GameObject go)
		{
			if (cachedGameObject == go)
			{
				return cachedComponent;
			}
			if (go == null)
			{
				return false;
			}
			cacheVersion++;
			cachedComponent = go.GetComponent<T>();
			cachedGameObject = go;
			_ = cachedComponent == null;
			return cachedComponent != null;
		}

		protected bool UpdateCachedTransform(GameObject go)
		{
			if (cachedGameObject == go)
			{
				return cachedTransform;
			}
			if (go == null)
			{
				return false;
			}
			cachedTransform = go.transform;
			cachedComponent = cachedTransform as T;
			cachedGameObject = go;
			return cachedTransform != null;
		}

		protected bool UpdateCacheAndTransform(GameObject go)
		{
			if (!UpdateCache(go))
			{
				return false;
			}
			cachedTransform = go.transform;
			return true;
		}

		protected bool UpdateCacheAddComponent(GameObject go)
		{
			if (cachedGameObject == go)
			{
				return cachedComponent;
			}
			if (go == null)
			{
				return false;
			}
			cachedComponent = go.GetComponent<T>();
			cachedGameObject = go;
			if (cachedComponent == null)
			{
				cachedComponent = go.AddComponent<T>();
				cachedComponent.hideFlags = HideFlags.DontSaveInEditor;
			}
			return cachedComponent != null;
		}

		protected void SendEvent(FsmEventTarget eventTarget, FsmEvent fsmEvent)
		{
			base.Fsm.Event(cachedGameObject, eventTarget, fsmEvent);
		}
	}
	public abstract class ComponentAction<T1, T2> : FsmStateAction where T1 : Component where T2 : Component
	{
		protected GameObject cachedGameObject1;

		protected GameObject cachedGameObject2;

		protected T1 cachedComponent1;

		protected T2 cachedComponent2;

		protected Transform cachedTransform2;

		protected bool UpdateCache(GameObject go1, GameObject go2)
		{
			if (go1 == null || go2 == null)
			{
				return false;
			}
			if (cachedComponent1 == null || cachedGameObject1 != go1)
			{
				cachedComponent1 = go1.GetComponent<T1>();
				cachedGameObject1 = go1;
				if (cachedComponent1 == null)
				{
					return false;
				}
			}
			if (cachedComponent2 == null || cachedGameObject2 != go2)
			{
				cachedComponent2 = go2.GetComponent<T2>();
				cachedGameObject2 = go2;
				if (cachedComponent2 == null)
				{
					return false;
				}
			}
			cachedTransform2 = cachedGameObject2.transform;
			return true;
		}
	}
}
