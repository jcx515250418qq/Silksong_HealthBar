using UnityEngine;

public class PermadeathUnlock : MonoBehaviour
{
	private void Start()
	{
		Unlock();
	}

	public static void Unlock()
	{
		GameManager.instance.SetStatusRecordInt("RecPermadeathMode", 1);
		GameManager.instance.SaveStatusRecords();
	}
}
