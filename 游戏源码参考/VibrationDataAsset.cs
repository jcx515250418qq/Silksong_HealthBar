using UnityEngine;

[CreateAssetMenu(menuName = "Platforms/Vibration/Vibration Data Asset")]
public sealed class VibrationDataAsset : ScriptableObject
{
	[SerializeField]
	private VibrationData vibrationData;

	[SerializeField]
	private bool disable;

	[TextArea(1, 20)]
	[SerializeField]
	private string comments;

	public VibrationData VibrationData
	{
		get
		{
			if (!disable)
			{
				return vibrationData;
			}
			return default(VibrationData);
		}
	}

	public static implicit operator VibrationData(VibrationDataAsset asset)
	{
		if ((bool)asset)
		{
			return asset.VibrationData;
		}
		return default(VibrationData);
	}

	public void SetData(VibrationData vibrationData)
	{
		this.vibrationData = vibrationData;
	}
}
