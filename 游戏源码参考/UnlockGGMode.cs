using UnityEngine;

public class UnlockGGMode : MonoBehaviour
{
	private void Start()
	{
		GameManager.instance.SetStatusRecordInt("RecBossRushMode", 1);
		GameManager.instance.SaveStatusRecords();
	}
}
