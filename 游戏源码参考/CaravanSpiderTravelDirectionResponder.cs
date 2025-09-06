using System;
using UnityEngine;

public class CaravanSpiderTravelDirectionResponder : MonoBehaviour
{
	[SerializeField]
	private CurveOffsetAnimation animator;

	[SerializeField]
	private float speed;

	private void OnEnable()
	{
		if ((bool)animator)
		{
			float caravanSpiderTravelDirection = PlayerData.instance.CaravanSpiderTravelDirection;
			float x = speed * Mathf.Cos(caravanSpiderTravelDirection * (MathF.PI / 180f));
			float y = speed * Mathf.Sin(caravanSpiderTravelDirection * (MathF.PI / 180f));
			animator.Offset = new Vector3(x, y, 0f);
		}
	}
}
