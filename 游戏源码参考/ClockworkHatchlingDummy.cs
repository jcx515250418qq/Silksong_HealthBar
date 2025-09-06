using GlobalSettings;
using UnityEngine;

public class ClockworkHatchlingDummy : MonoBehaviour
{
	[SerializeField]
	private tk2dSprite sprite;

	[SerializeField]
	private ToolItem representingTool;

	[SerializeField]
	private ParticleSystem ptPoisonTrail;

	private void OnValidate()
	{
		if (sprite == null)
		{
			sprite = GetComponent<tk2dSprite>();
		}
	}

	private void OnEnable()
	{
		CheckPoison();
	}

	private void CheckPoison()
	{
		if (Gameplay.PoisonPouchTool.IsEquipped && !ToolItemManager.IsCustomToolOverride)
		{
			Color poisonPouchTintColour = Gameplay.PoisonPouchTintColour;
			if ((bool)representingTool)
			{
				sprite.EnableKeyword("CAN_HUESHIFT");
				sprite.SetFloat(PoisonTintBase.HueShiftPropId, representingTool.PoisonHueShift);
			}
			else
			{
				sprite.EnableKeyword("RECOLOUR");
				sprite.color = poisonPouchTintColour;
			}
			ptPoisonTrail.Play();
		}
		else
		{
			sprite.DisableKeyword("CAN_HUESHIFT");
			sprite.DisableKeyword("RECOLOUR");
			sprite.color = Color.white;
		}
	}
}
