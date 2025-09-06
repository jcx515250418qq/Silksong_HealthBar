using UnityEngine;

public class SetPosIfPlayerdataBool : MonoBehaviour
{
	[PlayerDataField(typeof(bool), true)]
	public string playerDataBool;

	public bool setX;

	public float XPos;

	public bool setY;

	public float YPos;

	public bool onceOnly;

	private bool hasSet;

	private PlayerData playerData;

	private void OnEnable()
	{
		if (hasSet && onceOnly)
		{
			return;
		}
		if (playerData == null)
		{
			playerData = PlayerData.instance;
		}
		if (playerData.GetBool(playerDataBool))
		{
			if (setX)
			{
				base.transform.localPosition = new Vector3(XPos, base.transform.localPosition.y, base.transform.localPosition.z);
			}
			if (setY)
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, YPos, base.transform.localPosition.z);
			}
			hasSet = true;
		}
	}
}
