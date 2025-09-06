using UnityEngine;

[ExecuteInEditMode]
public class SetGlobalShaderTime : MonoBehaviour
{
	private static readonly int _unscaledGlobalTime = Shader.PropertyToID("_UnscaledGlobalTime");

	private void Update()
	{
		UpdateTime(Time.unscaledTime);
	}

	private void UpdateTime(float time)
	{
		Shader.SetGlobalFloat(_unscaledGlobalTime, time);
	}
}
