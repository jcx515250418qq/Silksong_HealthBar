using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class ResetDynamicHierarchy : MonoBehaviour, AntRegion.INotify
{
	private struct BehaviourActivation
	{
		public MonoBehaviour Behaviour;

		public bool Enabled;
	}

	private struct ColliderActivation
	{
		public Collider2D Collider;

		public bool Enabled;
	}

	private class State
	{
		public Transform Self;

		public bool ActiveSelf;

		public Transform Parent;

		public Vector3 Position;

		public Quaternion Rotation;

		public Vector3 Scale;

		public Rigidbody2D Body;

		public bool WasBodyKinematic;

		public BehaviourActivation[] BehaviourEnabledStates;

		public ColliderActivation[] ColliderEnabledStates;

		public bool Disconnected;

		public void Apply(bool alsoRoot)
		{
			if (Disconnected)
			{
				return;
			}
			Self.SetParent(Parent);
			if (alsoRoot || (bool)Parent)
			{
				if (Self.gameObject.activeSelf != ActiveSelf)
				{
					Self.gameObject.SetActive(ActiveSelf);
				}
				Self.localPosition = Position;
			}
			Self.localRotation = Rotation;
			Self.localScale = Scale;
			if ((bool)Body)
			{
				Body.linearVelocity = Vector2.zero;
				Body.angularVelocity = 0f;
				Body.isKinematic = WasBodyKinematic;
			}
			BehaviourActivation[] behaviourEnabledStates = BehaviourEnabledStates;
			for (int i = 0; i < behaviourEnabledStates.Length; i++)
			{
				BehaviourActivation behaviourActivation = behaviourEnabledStates[i];
				behaviourActivation.Behaviour.enabled = behaviourActivation.Enabled;
			}
			ColliderActivation[] colliderEnabledStates = ColliderEnabledStates;
			for (int i = 0; i < colliderEnabledStates.Length; i++)
			{
				ColliderActivation colliderActivation = colliderEnabledStates[i];
				colliderActivation.Collider.enabled = colliderActivation.Enabled;
			}
		}
	}

	private readonly List<State> initialStates = new List<State>();

	private readonly Dictionary<Transform, State> lookup = new Dictionary<Transform, State>();

	private readonly UniqueList<Transform> disconnectedTransforms = new UniqueList<Transform>();

	private static UniqueList<ResetDynamicHierarchy> activeResets = new UniqueList<ResetDynamicHierarchy>();

	private bool checkedInitialStates;

	private void Awake()
	{
		CaptureStatesRecursive(base.transform, initialStates);
	}

	private void OnDestroy()
	{
		activeResets.Remove(this);
	}

	public static void ForceReconnectAll()
	{
		activeResets.ReserveListUsage();
		foreach (ResetDynamicHierarchy item in activeResets.List)
		{
			item.ReconnectAll();
			item.DoReset(alsoRoot: true);
		}
		activeResets.ReleaseListUsage();
		activeResets.Clear();
	}

	private void ReconnectAll()
	{
		disconnectedTransforms.ReserveListUsage();
		foreach (Transform item in disconnectedTransforms.List)
		{
			if (!(item == null))
			{
				Reconnect(item, applyRoot: false, recursive: true);
			}
		}
		disconnectedTransforms.ReleaseListUsage();
		disconnectedTransforms.Clear();
	}

	public void DoReset(bool alsoRoot = false)
	{
		foreach (State initialState in initialStates)
		{
			initialState.Apply(alsoRoot);
		}
	}

	private void CreateLookup()
	{
		if (lookup.Count != 0 || initialStates.Count <= 0)
		{
			return;
		}
		foreach (State initialState in initialStates)
		{
			lookup[initialState.Self] = initialState;
		}
	}

	public void Disconnect(Transform target, bool recursive)
	{
		CreateLookup();
		if (!lookup.TryGetValue(target, out var value))
		{
			return;
		}
		value.Disconnected = true;
		disconnectedTransforms.Add(target);
		activeResets.Add(this);
		if (!recursive)
		{
			return;
		}
		foreach (Transform item in target)
		{
			Disconnect(item, recursive: true);
		}
	}

	public void Reconnect(Transform target, bool applyRoot, bool recursive)
	{
		if (!lookup.TryGetValue(target, out var value) || !value.Disconnected)
		{
			return;
		}
		value.Disconnected = false;
		disconnectedTransforms.Remove(target);
		if (disconnectedTransforms.Count == 0)
		{
			activeResets.Remove(this);
		}
		if (!base.gameObject.activeInHierarchy)
		{
			value.Apply(applyRoot);
		}
		if (!recursive)
		{
			return;
		}
		foreach (Transform item in target)
		{
			Reconnect(item, applyRoot, recursive: true);
		}
	}

	private static void CaptureStatesRecursive(Transform target, List<State> addState)
	{
		Rigidbody2D component = target.GetComponent<Rigidbody2D>();
		addState.Add(new State
		{
			Self = target,
			ActiveSelf = target.gameObject.activeSelf,
			Body = component,
			WasBodyKinematic = (component ? component.isKinematic : ((bool)component)),
			Parent = target.parent,
			Position = target.localPosition,
			Rotation = target.localRotation,
			Scale = target.localScale,
			BehaviourEnabledStates = target.GetComponents<MonoBehaviour>().Select(delegate(MonoBehaviour b)
			{
				BehaviourActivation result = default(BehaviourActivation);
				result.Behaviour = b;
				result.Enabled = b.enabled;
				return result;
			}).ToArray(),
			ColliderEnabledStates = target.GetComponents<Collider2D>().Select(delegate(Collider2D c)
			{
				ColliderActivation result2 = default(ColliderActivation);
				result2.Collider = c;
				result2.Enabled = c.enabled;
				return result2;
			}).ToArray()
		});
		foreach (Transform item in target)
		{
			CaptureStatesRecursive(item, addState);
		}
	}

	public void EnteredAntRegion(AntRegion antRegion)
	{
		if (checkedInitialStates)
		{
			return;
		}
		foreach (State initialState in initialStates)
		{
			if ((bool)initialState.Self.GetComponent<FlingChildrenOnStart>())
			{
				initialState.Self.gameObject.AddComponentIfNotPresent<AntRegionFlingChildrenNotifier>();
			}
		}
		checkedInitialStates = true;
	}

	public void ExitedAntRegion(AntRegion antRegion)
	{
	}
}
