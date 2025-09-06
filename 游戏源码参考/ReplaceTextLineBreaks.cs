using TMProOld;
using UnityEngine;

public class ReplaceTextLineBreaks : MonoBehaviour
{
	private TextMeshPro textMesh;

	private void Start()
	{
		textMesh = GetComponent<TextMeshPro>();
		string text = textMesh.text;
		text = text.Replace("<br>", "\n");
		textMesh.text = text;
	}
}
