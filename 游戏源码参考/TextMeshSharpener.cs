using System;
using UnityEngine;

public class TextMeshSharpener : MonoBehaviour
{
	private float lastPixelHeight = -1f;

	private TextMesh textMesh;

	private void Start()
	{
		textMesh = GetComponent<TextMesh>();
		resize();
	}

	private void Update()
	{
		if ((float)Camera.main.pixelHeight != lastPixelHeight || (Application.isEditor && !Application.isPlaying))
		{
			resize();
		}
	}

	private void resize()
	{
		float num = Camera.main.pixelHeight;
		float num2 = Camera.main.orthographicSize * 2f / num;
		float num3 = 128f;
		textMesh.characterSize = num2 * Camera.main.orthographicSize / Math.Max(base.transform.localScale.x, base.transform.localScale.y);
		textMesh.fontSize = (int)Math.Round(num3 / textMesh.characterSize);
		lastPixelHeight = num;
	}
}
