using System;
using UnityEngine;

public class PositionShadeMarkerPerDoor : MonoBehaviour
{
	[Serializable]
	public struct DoorShadePosition
	{
		public GameObject door;

		public Vector2 position;
	}

	public GameObject shadeMarker;

	public DoorShadePosition[] shadePositions;

	public void Start()
	{
		if (!shadeMarker)
		{
			return;
		}
		DoorShadePosition[] array = shadePositions;
		for (int i = 0; i < array.Length; i++)
		{
			DoorShadePosition doorShadePosition = array[i];
			if (doorShadePosition.door.name == GameManager.instance.entryGateName)
			{
				shadeMarker.transform.SetPosition2D(doorShadePosition.position);
				break;
			}
		}
	}
}
