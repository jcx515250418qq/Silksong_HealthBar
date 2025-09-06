using GlobalSettings;
using UnityEngine;

public abstract class UIMsgPopupBaseBase : MonoBehaviour
{
	protected static Transform LastActiveMsgShared;

	public static float MinYPos { get; set; } = float.MinValue;

	protected static void UpdatePosition(Transform transform)
	{
		if ((bool)LastActiveMsgShared)
		{
			if (transform != LastActiveMsgShared)
			{
				SetPos(LastActiveMsgShared.localPosition + UI.UIMsgPopupStackOffset);
			}
		}
		else
		{
			SetPos(UI.UIMsgPopupStartPosition);
		}
		LastActiveMsgShared = transform;
		void SetPos(Vector3 pos)
		{
			if (pos.y < MinYPos)
			{
				pos.y = MinYPos;
			}
			transform.localPosition = pos;
		}
	}
}
