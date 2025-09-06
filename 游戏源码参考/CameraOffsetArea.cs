using UnityEngine;

public class CameraOffsetArea : MonoBehaviour
{
	[SerializeField]
	private Vector2 offset;

	public Vector2 Offset => offset;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (CameraLockArea.IsInApplicableGameState() && collision.CompareTag("Player"))
		{
			GameCameras.instance.cameraTarget.AddOffsetArea(this);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			GameCameras.instance.cameraTarget.RemoveOffsetArea(this);
		}
	}
}
