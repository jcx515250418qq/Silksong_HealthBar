using TMProOld;
using UnityEngine;

public class GetTMProLeftVertex : MonoBehaviour
{
	private TextMeshPro textMesh;

	private void Start()
	{
		textMesh = GetComponent<TextMeshPro>();
	}

	public float GetLeftmostVertex()
	{
		return textMesh.mesh.bounds.extents.x;
	}
}
