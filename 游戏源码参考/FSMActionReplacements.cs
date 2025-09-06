using UnityEngine;

public static class FSMActionReplacements
{
	public enum Directions
	{
		Right = 0,
		Up = 1,
		Left = 2,
		Down = 3,
		Unknown = 4
	}

	public static void SetMaterialColor(Component me, Color color)
	{
		Renderer component = me.GetComponent<Renderer>();
		if (component != null)
		{
			component.material.color = color;
		}
	}

	public static Directions CheckDirectionWithBrokenBehaviour(float angle)
	{
		if (angle < 45f)
		{
			return Directions.Right;
		}
		if (angle < 135f)
		{
			return Directions.Up;
		}
		if (angle < 225f)
		{
			return Directions.Left;
		}
		if (angle < 360f)
		{
			return Directions.Down;
		}
		return Directions.Unknown;
	}
}
