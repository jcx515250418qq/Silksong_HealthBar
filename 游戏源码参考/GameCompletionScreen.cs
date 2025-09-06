using TMProOld;
using UnityEngine;

public class GameCompletionScreen : MonoBehaviour
{
	public TextMeshPro percentageNumber;

	public TextMeshPro playTimeNumber;

	private GameManager gm;

	private void Start()
	{
		gm = GameManager.instance;
		PlayerData playerData = gm.playerData;
		playerData.CountGameCompletion();
		SaveStats saveStats = new SaveStats(playerData, null);
		percentageNumber.text = saveStats.GetCompletionPercentage();
		playTimeNumber.text = saveStats.GetPlaytimeHHMMSS();
	}
}
