using TMProOld;
using UnityEngine;

public class ControlReminderSingle : MonoBehaviour
{
	[SerializeField]
	private ActionButtonIcon actionIcon;

	[SerializeField]
	private TMP_Text actionPromptText;

	[SerializeField]
	private TMP_Text actionText;

	public void Activate(ControlReminder.SingleConfig config)
	{
		actionIcon.SetAction(ControlReminder.MapActionToAction(config.Button));
		actionPromptText.text = (config.Prompt.IsEmpty ? string.Empty : ((string)config.Prompt));
		actionText.text = config.Text;
		base.gameObject.SetActive(value: true);
	}
}
