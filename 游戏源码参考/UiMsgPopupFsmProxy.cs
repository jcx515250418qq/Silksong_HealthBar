public class UiMsgPopupFsmProxy : UIMsgPopupBaseBase
{
	public void Activated()
	{
		UIMsgPopupBaseBase.UpdatePosition(base.transform);
	}

	public void Deactivated()
	{
		if (UIMsgPopupBaseBase.LastActiveMsgShared == base.transform)
		{
			UIMsgPopupBaseBase.LastActiveMsgShared = null;
		}
	}
}
