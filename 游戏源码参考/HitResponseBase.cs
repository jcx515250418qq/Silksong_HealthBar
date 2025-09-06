using UnityEngine;

public abstract class HitResponseBase : DebugDrawColliderRuntimeAdder
{
	public delegate void HitInDirectionDelegate(GameObject source, HitInstance.HitDirection direction);

	public abstract bool IsActive { get; set; }

	public event HitInDirectionDelegate HitInDirection;

	protected void SendHitInDirection(GameObject source, HitInstance.HitDirection direction)
	{
		this.HitInDirection?.Invoke(source, direction);
	}

	public override void AddDebugDrawComponent()
	{
		if (base.enabled)
		{
			if (base.gameObject.layer == 8)
			{
				DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.TerrainCollider);
			}
			else
			{
				DebugDrawColliderRuntime.AddOrUpdate(base.gameObject);
			}
		}
	}
}
