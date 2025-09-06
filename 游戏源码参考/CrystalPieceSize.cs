using UnityEngine;

public class CrystalPieceSize : MonoBehaviour
{
	private void OnEnable()
	{
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, Random.Range(-0.01f, 0.01f));
		float num = ((Random.Range(0, 100) >= 75) ? Random.Range(0.9f, 1.2f) : Random.Range(0.65f, 0.85f));
		base.transform.localScale = new Vector3(num, num, num);
	}
}
