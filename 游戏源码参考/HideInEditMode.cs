using UnityEngine;

[ExecuteInEditMode]
public class HideInEditMode : MonoBehaviour, ISceneLintUpgrader
{
	[SerializeField]
	private bool isVisible;

	private bool isInCameraRender;

	private bool subscribed;

	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		if (!base.gameObject.activeSelf)
		{
			return "HideInEditMode object is disabled - is this intended? (" + base.gameObject.name + ")";
		}
		return null;
	}
}
