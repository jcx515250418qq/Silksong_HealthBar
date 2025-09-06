using TMProOld;
using UnityEngine;

public class QuestItemDescriptionText : MonoBehaviour
{
	[SerializeField]
	private TMP_Text rangeText;

	[SerializeField]
	private SpriteRenderer rangeTextIcon;

	private bool gotInitialValues;

	private string initialRangeText;

	private Vector3 initialRangeIconScale;

	private void Awake()
	{
		GetInitialValues();
	}

	private void GetInitialValues()
	{
		if (!gotInitialValues)
		{
			gotInitialValues = true;
			if ((bool)rangeText)
			{
				initialRangeText = rangeText.text;
			}
			if ((bool)rangeTextIcon)
			{
				initialRangeIconScale = rangeTextIcon.transform.localScale;
			}
		}
	}

	public void ResetDisplay()
	{
		GetInitialValues();
		if ((bool)rangeText)
		{
			rangeText.text = initialRangeText;
		}
		if ((bool)rangeTextIcon)
		{
			rangeTextIcon.transform.localScale = initialRangeIconScale;
		}
	}

	public void SetDisplay(FullQuestBase fullQuest, FullQuestBase.QuestTarget target, int counter)
	{
		GetInitialValues();
		if ((bool)rangeText)
		{
			rangeText.text = string.Format(initialRangeText, counter, target.Count);
		}
		if ((bool)rangeTextIcon)
		{
			rangeTextIcon.sprite = fullQuest.GetCounterSpriteOverride(target, 0);
			float counterIconScale = fullQuest.CounterIconScale;
			rangeTextIcon.transform.localScale = new Vector3(counterIconScale, counterIconScale, 1f).MultiplyElements(initialRangeIconScale);
		}
	}
}
