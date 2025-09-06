using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image effects/Custom/Uber Postprocess")]
public class UberPostprocess : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Shader that implements all of the postprocesses")]
	private Shader postprocessShader;

	private List<IPostprocessModule> modules;

	private Material material;

	private void OnEnable()
	{
		material = new Material(postprocessShader);
		material.name = "UberPostprocess_Material";
		material.hideFlags = HideFlags.HideAndDontSave;
		modules = new List<IPostprocessModule>();
	}

	private void OnDisable()
	{
		Object.DestroyImmediate(material);
		material = null;
		modules.Clear();
		modules = null;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (modules == null)
		{
			modules = new List<IPostprocessModule>();
		}
		modules.Clear();
		GetComponents(modules);
		for (int i = 0; i < modules.Count; i++)
		{
			IPostprocessModule postprocessModule = modules[i];
			if ((postprocessModule as MonoBehaviour).enabled)
			{
				material.EnableKeyword(postprocessModule.EffectKeyword);
				modules[i].UpdateProperties(material);
			}
			else
			{
				material.DisableKeyword(postprocessModule.EffectKeyword);
			}
		}
		Graphics.Blit(source, destination, material, 0);
		if ((bool)GameCameraTextureDisplay.Instance)
		{
			GameCameraTextureDisplay.Instance.UpdateDisplay(source, material);
		}
	}
}
