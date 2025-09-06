using UnityEngine;

public class ParentSwayConfig : MonoBehaviour
{
	[SerializeField]
	private bool applyMapZoneSway = true;

	public bool ApplyMapZoneSway => applyMapZoneSway;

	public bool HasIdleSway => true;
}
