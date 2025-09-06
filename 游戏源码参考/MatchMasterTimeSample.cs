using UnityEngine;

public class MatchMasterTimeSample : MonoBehaviour
{
	public AudioSource master;

	public AudioSource slave1;

	public AudioSource slave2;

	public AudioSource slave3;

	public AudioSource slave4;

	private void Update()
	{
		slave1.timeSamples = master.timeSamples;
		slave2.timeSamples = master.timeSamples;
		slave3.timeSamples = master.timeSamples;
		slave4.timeSamples = master.timeSamples;
	}
}
