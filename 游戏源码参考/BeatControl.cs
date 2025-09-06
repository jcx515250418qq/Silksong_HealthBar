using UnityEngine;

public class BeatControl : MonoBehaviour
{
	public float beatIncrease;

	private float oldBeatValue;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.B))
		{
			beatIncrease += 0.25f;
		}
		if (beatIncrease != oldBeatValue)
		{
			oldBeatValue = beatIncrease;
			Shader.SetGlobalFloat("_BeatSpeedIncrease", beatIncrease);
			Shader.SetGlobalFloat("_BeatMagnitudeIncrease", beatIncrease);
		}
	}

	private void OnDestroy()
	{
		Shader.SetGlobalFloat("_BeatSpeedIncrease", 0f);
		Shader.SetGlobalFloat("_BeatMagnitudeIncrease", 0f);
	}
}
