using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PreselectOption : MonoBehaviour
{
	public Selectable itemToHighlight;

	public void HighlightDefault(bool deselect = false)
	{
		EventSystem current = EventSystem.current;
		if (!deselect && !(current.currentSelectedGameObject == null) && current.currentSelectedGameObject.activeInHierarchy)
		{
			return;
		}
		UIManager.HighlightSelectableNoSound(itemToHighlight);
		foreach (Transform item in itemToHighlight.transform)
		{
			Animator component = item.GetComponent<Animator>();
			if (component != null)
			{
				component.ResetTrigger("hide");
				component.SetTrigger("show");
				break;
			}
		}
	}

	public void SetDefaultHighlight(Button button)
	{
		itemToHighlight = button;
	}

	public void DeselectAll()
	{
		StartCoroutine(ForceDeselect());
	}

	private IEnumerator ForceDeselect()
	{
		yield return new WaitForSeconds(0.165f);
		UIManager.instance.eventSystem.SetSelectedGameObject(null);
	}
}
