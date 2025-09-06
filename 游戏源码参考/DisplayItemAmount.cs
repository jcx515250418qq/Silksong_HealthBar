using TMProOld;
using TeamCherry.SharedUtils;
using UnityEngine;

public class DisplayItemAmount : MonoBehaviour
{
	[PlayerDataField(typeof(int), true)]
	[SerializeField]
	private string playerDataInt;

	[SerializeField]
	private TextMeshPro textObject;

	private PlayerData playerData;

	private void OnEnable()
	{
		if (playerData == null)
		{
			playerData = PlayerData.instance;
		}
		Refresh();
	}

	private void Refresh()
	{
		string text = playerData.GetVariable<int>(playerDataInt).ToString();
		textObject.text = text;
	}
}
