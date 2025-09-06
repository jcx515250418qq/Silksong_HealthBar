using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Debug/Switch Vibration List")]
public sealed class SwitchVibrationList : ScriptableObject
{
	private const string VIBRATION_DIRECTORY = "Assets/Audio/Vibration Files";

	public List<TextAsset> vibrations = new List<TextAsset>();
}
