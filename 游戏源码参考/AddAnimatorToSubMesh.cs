using TMProOld;
using UnityEngine;

public sealed class AddAnimatorToSubMesh : MonoBehaviour
{
	[SerializeField]
	private RuntimeAnimatorController controller;

	private void Start()
	{
		TMP_SubMesh[] componentsInChildren = GetComponentsInChildren<TMP_SubMesh>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.AddComponentIfNotPresent<Animator>().runtimeAnimatorController = controller;
		}
	}

	private void OnTransformChildrenChanged()
	{
		TMP_SubMesh[] componentsInChildren = GetComponentsInChildren<TMP_SubMesh>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.AddComponentIfNotPresent<Animator>().runtimeAnimatorController = controller;
		}
	}
}
