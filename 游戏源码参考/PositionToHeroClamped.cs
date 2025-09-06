using UnityEngine;

public class PositionToHeroClamped : MonoBehaviour
{
	public Vector2 ClampAreaOffset;

	public Vector2 ClampAreaSize;

	public bool PositionX;

	public bool PositionY;

	private Transform hero;

	private void OnDrawGizmosSelected()
	{
		GetPositionAndSize(out var pos, out var size);
		Gizmos.DrawWireCube(pos, size);
	}

	private void Start()
	{
		hero = HeroController.instance.transform;
	}

	private void Update()
	{
		Vector2 vector = hero.position;
		Vector2 vector2 = base.transform.position;
		GetPositionAndSize(out var pos, out var size);
		Vector2 vector3 = pos;
		Vector2 vector4 = (Vector2)size / 2f;
		Vector2 vector5 = vector3 - vector4;
		Vector2 vector6 = vector3 + vector4;
		if (PositionX)
		{
			vector2.x = vector.x;
			vector2.x = Mathf.Clamp(vector2.x, vector5.x, vector6.x);
		}
		if (PositionY)
		{
			vector2.y = vector.y;
			vector2.y = Mathf.Clamp(vector2.y, vector5.y, vector6.y);
		}
		base.transform.position = vector2;
	}

	private void GetPositionAndSize(out Vector3 pos, out Vector3 size)
	{
		Transform parent = base.transform.parent;
		if (parent == null)
		{
			pos = ClampAreaOffset.ToVector3(base.transform.position.z);
			size = ClampAreaSize.ToVector3(1f);
			return;
		}
		Vector3 original = parent.TransformPoint(ClampAreaOffset);
		float? z = base.transform.position.z;
		pos = original.Where(null, null, z);
		Vector3 original2 = parent.TransformVector(ClampAreaSize);
		z = 1f;
		size = original2.Where(null, null, z);
	}
}
