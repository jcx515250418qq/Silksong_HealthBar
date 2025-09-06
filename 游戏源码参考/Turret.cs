using UnityEngine;

public class Turret : MonoBehaviour
{
	public Bullet bulletPrefab;

	public Transform gun;

	public GameObject testPrefab;

	private void Awake()
	{
	}

	private void Update()
	{
		Plane plane = new Plane(Vector3.up, base.transform.position);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (plane.Raycast(ray, out var enter))
		{
			Quaternion to = Quaternion.LookRotation(Vector3.Normalize(ray.GetPoint(enter) - base.transform.position));
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, 360f * Time.deltaTime);
			if (Input.GetMouseButtonDown(0))
			{
				bulletPrefab.Spawn(gun.position, gun.rotation);
			}
			if (Input.GetMouseButtonDown(1))
			{
				testPrefab.Spawn(gun.position, gun.rotation);
			}
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			bulletPrefab.DestroyPooled();
		}
		if (Input.GetKeyDown(KeyCode.Z))
		{
			bulletPrefab.DestroyAll();
		}
	}
}
