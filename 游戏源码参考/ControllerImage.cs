using System;
using GlobalEnums;
using UnityEngine;

[Serializable]
public class ControllerImage
{
	public string name;

	public GamepadType gamepadType;

	public Sprite sprite;

	public ControllerButtonPositions buttonPositions;

	public float displayScale = 1f;

	public float offsetY;
}
