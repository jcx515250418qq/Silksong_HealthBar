using UnityEngine;
using UnityEngine.UI;

public class ModalDialog : MonoBehaviour
{
	public CanvasGroup content;

	public Selectable defaultHighlight;

	public CanvasGroup modalWindow => GetComponent<CanvasGroup>();

	public void HighlightDefault()
	{
		if (defaultHighlight != null)
		{
			defaultHighlight.Select();
			{
				foreach (Transform item in defaultHighlight.transform)
				{
					Animator component = item.GetComponent<Animator>();
					if (component != null)
					{
						component.ResetTrigger("hide");
						component.SetTrigger("show");
						break;
					}
				}
				return;
			}
		}
		Debug.LogError("No default highlight item defined.");
	}
}
