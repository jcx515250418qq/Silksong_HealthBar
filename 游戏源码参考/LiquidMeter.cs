using TMProOld;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;

public class LiquidMeter : MonoBehaviour
{
	[SerializeField]
	private GameObject liquidParent;

	[SerializeField]
	private NestedFadeGroupSpriteRenderer liquidSprite;

	[SerializeField]
	private Transform liquidOffset;

	[SerializeField]
	private MinMaxFloat posRangeY;

	[SerializeField]
	private TMP_Text countText;

	[SerializeField]
	private GameObject glandParent;

	private string countFormatText;

	public void SetDisplay(ToolItemStatesLiquid toolItem)
	{
		bool hasInfiniteRefills = toolItem.HasInfiniteRefills;
		liquidParent.SetActive(!hasInfiniteRefills);
		glandParent.SetActive(hasInfiniteRefills);
		if (!hasInfiniteRefills)
		{
			int refillsMax = toolItem.RefillsMax;
			int refillsLeft = toolItem.LiquidSavedData.RefillsLeft;
			float t = (float)refillsLeft / (float)refillsMax;
			liquidSprite.Color = toolItem.LiquidColor;
			if (refillsLeft <= 0)
			{
				liquidSprite.AlphaSelf = 0f;
			}
			liquidOffset.SetLocalPositionY(posRangeY.GetLerpedValue(t));
			if (countFormatText == null)
			{
				countFormatText = countText.text;
			}
			countText.text = string.Format(countFormatText, refillsLeft, refillsMax);
		}
	}
}
