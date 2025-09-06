using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Lens CA And Distortion")]
public class LensCAAndDistortion : MonoBehaviour
{
	public Shader LensShader;

	[Range(-10f, 10f)]
	public float RedCyan;

	[Range(-10f, 10f)]
	public float GreenMagenta;

	[Range(-10f, 10f)]
	public float BlueYellow;

	public bool TrimExtents;

	public Texture2D TrimTexture;

	[Range(-1f, 1f)]
	public float Zoom;

	[Range(-5f, 5f)]
	public float BarrelDistortion;

	private Material curMaterial;

	private Material material
	{
		get
		{
			if (curMaterial == null)
			{
				curMaterial = new Material(LensShader);
				curMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return curMaterial;
		}
	}

	private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if (LensShader != null)
		{
			material.SetFloat("_RedCyan", RedCyan * 10f);
			material.SetFloat("_GreenMagenta", GreenMagenta * 10f);
			material.SetFloat("_BlueYellow", BlueYellow * 10f);
			material.SetFloat("_Zoom", 0f - Zoom);
			material.SetFloat("_BarrelDistortion", BarrelDistortion);
			material.SetTexture("_BorderTex", TrimTexture);
			if (TrimExtents)
			{
				material.SetInt("_BorderOnOff", 1);
			}
			else
			{
				material.SetInt("_BorderOnOff", 0);
			}
			Graphics.Blit(sourceTexture, destTexture, material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);
		}
	}

	private void Update()
	{
	}

	private void OnDisable()
	{
		if ((bool)curMaterial)
		{
			Object.DestroyImmediate(curMaterial);
		}
	}
}
