using TMProOld;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class InventoryItemButtonPromptDisplay : MonoBehaviour, InventoryItemButtonPromptDisplayList.IPromptDisplayListOrder
{
	[SerializeField]
	private ActionButtonIcon icon;

	[SerializeField]
	private TMP_Text useText;

	[SerializeField]
	private TMP_Text responseText;

	[SerializeField]
	private NestedFadeGroupBase mainGroup;

	[SerializeField]
	[Range(0f, 1f)]
	private float disabledGroupAlpha;

	[SerializeField]
	private TMP_Text conditionText;

	public int order { get; set; }

	Transform InventoryItemButtonPromptDisplayList.IPromptDisplayListOrder.transform => base.transform;

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Show(InventoryItemButtonPromptData source, bool forceDisabled)
	{
		if ((bool)icon)
		{
			icon.SetAction(source.MenuAction);
		}
		if ((bool)useText)
		{
			useText.text = (source.UseText.IsEmpty ? string.Empty : ((string)source.UseText));
		}
		if ((bool)responseText)
		{
			responseText.text = source.ResponseText;
		}
		bool flag = !source.ConditionText.IsEmpty;
		if ((bool)mainGroup)
		{
			mainGroup.AlphaSelf = ((flag || forceDisabled) ? disabledGroupAlpha : 1f);
		}
		if ((bool)conditionText)
		{
			conditionText.text = (flag ? ((string)source.ConditionText) : string.Empty);
		}
		base.gameObject.SetActive(value: true);
	}
}
