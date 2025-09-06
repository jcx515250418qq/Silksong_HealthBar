using UnityEngine;

[ExecuteInEditMode]
public class SetGlobalShaderPos : MonoBehaviour
{
	[SerializeField]
	private string variableName;

	[SerializeField]
	private Vector3 offset;

	[SerializeField]
	private string playModeVariable;

	private void OnEnable()
	{
		if (!string.IsNullOrEmpty(playModeVariable))
		{
			Shader.SetGlobalFloat(playModeVariable, Application.isPlaying ? 1 : 0);
		}
		UpdateValue();
	}

	private void Update()
	{
		UpdateValue();
	}

	private void UpdateValue()
	{
		if (!string.IsNullOrEmpty(variableName))
		{
			Vector3 vector = base.transform.position + offset;
			Shader.SetGlobalVector(variableName, vector);
		}
	}
}
