using TMProOld;
using UnityEngine;

public class InventoryArrowContainer : MonoBehaviour
{
	[SerializeField]
	private GameObject arrowVariant;

	[SerializeField]
	private GameObject promptVariant;

	[SerializeField]
	private TextMeshPro label;

	[SerializeField]
	private float labelLeftInset;

	[SerializeField]
	private float labelRightInset;

	protected void Start()
	{
		Setup();
		ManagerSingleton<InputHandler>.Instance.RefreshActiveControllerEvent += Setup;
	}

	private void OnDestroy()
	{
		if ((bool)ManagerSingleton<InputHandler>.UnsafeInstance)
		{
			ManagerSingleton<InputHandler>.UnsafeInstance.RefreshActiveControllerEvent -= Setup;
		}
	}

	private void Setup()
	{
		bool isControllerImplicit = Platform.Current.IsControllerImplicit;
		arrowVariant.SetActive(!isControllerImplicit);
		promptVariant.SetActive(isControllerImplicit);
		if (isControllerImplicit && (bool)label)
		{
			Vector4 margin = label.margin;
			margin.x += labelLeftInset;
			margin.z += labelRightInset;
			label.margin = margin;
		}
		base.enabled = false;
	}
}
