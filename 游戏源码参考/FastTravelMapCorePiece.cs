using UnityEngine;

public class FastTravelMapCorePiece : MonoBehaviour, IFastTravelMapPiece
{
	[SerializeField]
	private Object[] anyPiece;

	public bool IsVisible
	{
		get
		{
			bool flag = false;
			Object[] array = anyPiece;
			for (int i = 0; i < array.Length; i++)
			{
				IFastTravelMapPiece fastTravelMapPiece = (IFastTravelMapPiece)array[i];
				if (fastTravelMapPiece != null)
				{
					flag = true;
					if (fastTravelMapPiece.IsVisible)
					{
						return true;
					}
				}
			}
			return !flag;
		}
	}

	private void OnValidate()
	{
		if (anyPiece == null)
		{
			anyPiece = new Object[0];
		}
		for (int i = 0; i < anyPiece.Length; i++)
		{
			Object @object = anyPiece[i];
			if (@object is IFastTravelMapPiece)
			{
				continue;
			}
			GameObject gameObject = @object as GameObject;
			if (gameObject != null)
			{
				IFastTravelMapPiece component = gameObject.GetComponent<IFastTravelMapPiece>();
				if (component != null)
				{
					anyPiece[i] = component as Component;
					continue;
				}
			}
			anyPiece[i] = null;
		}
	}

	private void Awake()
	{
		OnValidate();
		IFastTravelMap componentInParent = GetComponentInParent<IFastTravelMap>();
		if (componentInParent != null)
		{
			componentInParent.Opening += OnOpening;
		}
	}

	private void OnOpening()
	{
		base.gameObject.SetActive(IsVisible);
	}
}
