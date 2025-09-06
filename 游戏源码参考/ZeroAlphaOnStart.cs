using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ZeroAlphaOnStart : MonoBehaviour
{
	private void Start()
	{
		GetComponent<CanvasGroup>().alpha = 0f;
	}
}
