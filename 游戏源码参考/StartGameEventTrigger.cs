using UnityEngine.EventSystems;

public class StartGameEventTrigger : MenuButtonListCondition, ISubmitHandler, IEventSystemHandler, IPointerClickHandler
{
	public bool permaDeath;

	public bool bossRush;

	public void OnSubmit(BaseEventData eventData)
	{
		UIManager.instance.StartNewGame(permaDeath, bossRush);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		OnSubmit(eventData);
	}

	public override bool IsFulfilled()
	{
		bool result = true;
		if (permaDeath && GameManager.instance.GetStatusRecordInt("RecPermadeathMode") == 0)
		{
			result = false;
		}
		if (bossRush && GameManager.instance.GetStatusRecordInt("RecBossRushMode") == 0)
		{
			result = false;
		}
		return result;
	}
}
