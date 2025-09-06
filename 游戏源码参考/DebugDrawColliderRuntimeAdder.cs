using UnityEngine;

public abstract class DebugDrawColliderRuntimeAdder : MonoBehaviour
{
	protected virtual void Awake()
	{
		AddDebugDrawComponent();
	}

	public abstract void AddDebugDrawComponent();
}
