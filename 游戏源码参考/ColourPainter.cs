using System.Collections.Generic;
using UnityEngine;

public class ColourPainter : MonoBehaviour
{
	public Color colour;

	public int chance;

	public float delay;

	public List<SpriteRenderer> splatList;

	private BoxCollider2D boxCollider;

	private float timer;

	private bool active;

	private bool painted;

	private void Awake()
	{
		boxCollider = GetComponent<BoxCollider2D>();
	}

	private void Update()
	{
		if (!active)
		{
			return;
		}
		if (timer < delay)
		{
			timer += Time.deltaTime;
			return;
		}
		foreach (SpriteRenderer splat in splatList)
		{
			splat.color = colour;
		}
		boxCollider.enabled = false;
		active = false;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Extra Tag")
		{
			splatList.Add(collision.gameObject.GetComponent<SpriteRenderer>());
		}
	}

	public void DoPaint()
	{
		splatList.Clear();
		timer = 0f;
		active = true;
		boxCollider.enabled = true;
	}
}
