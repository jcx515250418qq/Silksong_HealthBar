using System;
using UnityEngine;

public static class DirectionUtils
{
	public const int Right = 0;

	public const int Up = 1;

	public const int Left = 2;

	public const int Down = 3;

	public static int GetCardinalDirection(float degrees)
	{
		return NegSafeMod(Mathf.RoundToInt(degrees / 90f), 4);
	}

	public static int NegSafeMod(int val, int len)
	{
		return (val % len + len) % len;
	}

	public static int GetX(int cardinalDirection)
	{
		return (cardinalDirection % 4) switch
		{
			0 => 1, 
			2 => -1, 
			_ => 0, 
		};
	}

	public static int GetY(int cardinalDirection)
	{
		return (cardinalDirection % 4) switch
		{
			1 => 1, 
			3 => -1, 
			_ => 0, 
		};
	}

	public static float GetAngle(int cardinalDirection)
	{
		return cardinalDirection switch
		{
			2 => 180f, 
			0 => 0f, 
			1 => 90f, 
			3 => 270f, 
			_ => throw new ArgumentOutOfRangeException("cardinalDirection", cardinalDirection, null), 
		};
	}

	public static float GetAngle(HitInstance.HitDirection hitDirection)
	{
		return hitDirection switch
		{
			HitInstance.HitDirection.Left => 180f, 
			HitInstance.HitDirection.Right => 0f, 
			HitInstance.HitDirection.Up => 90f, 
			HitInstance.HitDirection.Down => 270f, 
			_ => throw new ArgumentOutOfRangeException("hitDirection", hitDirection, null), 
		};
	}
}
