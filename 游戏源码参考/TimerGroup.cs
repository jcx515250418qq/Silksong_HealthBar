using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Timer Group")]
public class TimerGroup : ScriptableObject
{
	[SerializeField]
	private float delay;

	public double EndTime { get; private set; }

	public float TimeLeft => (float)(EndTime - Time.timeAsDouble);

	public bool HasEnded => Time.timeAsDouble >= EndTime;

	public void ResetTimer()
	{
		EndTime = Time.timeAsDouble + (double)delay;
	}
}
