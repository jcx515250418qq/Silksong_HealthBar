using UnityEngine;

public abstract class UnlockablePropBase : MonoBehaviour
{
	[ContextMenu("Test Unlock")]
	private void TestOpen()
	{
		Open();
	}

	[ContextMenu("Test Unlock", true)]
	private bool CanTest()
	{
		return Application.isPlaying;
	}

	public abstract void Open();

	public abstract void Opened();
}
