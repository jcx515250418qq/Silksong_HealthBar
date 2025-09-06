using UnityEngine;

public class CustomTag : MonoBehaviour
{
	public enum CustomTagTypes
	{
		Bomb = 0,
		DustBomb = 1,
		LightningBola = 2,
		LightningBolaBall = 3,
		SkinnyMosquito = 4,
		CogworkHatchling = 5,
		PreventBlackThread = 6
	}

	[SerializeField]
	private CustomTagTypes customTag;

	public CustomTagTypes CustomTagType => customTag;

	public string GetCustomTagAsString()
	{
		return customTag.ToString();
	}

	public bool IsSuckable()
	{
		if (customTag != 0 && customTag != CustomTagTypes.DustBomb && customTag != CustomTagTypes.LightningBola)
		{
			return customTag == CustomTagTypes.SkinnyMosquito;
		}
		return true;
	}
}
