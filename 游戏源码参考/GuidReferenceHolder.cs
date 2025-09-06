using UnityEngine;

public class GuidReferenceHolder : MonoBehaviour
{
	[SerializeField]
	private GameObject localOverride;

	[SerializeField]
	private GuidReference reference;

	public GameObject ReferencedGameObject
	{
		get
		{
			if (!localOverride)
			{
				return reference.gameObject;
			}
			return localOverride;
		}
	}
}
