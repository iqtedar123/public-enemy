﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPlayer : MonoBehaviour
{

	public static float speed;
	public static float range;
	public static int damage;
	public static int miscLayer = 1 << 11;

	private float maxX;
	private float maxY;
	private float minX;
	private float minY;
	private Vector3 size;
	private Vector3 startPos;
	public static AudioSource audioSrc;
	void Start ()
	{
		float cameraDistance = (transform.position - Camera.main.transform.position).z;
		maxX = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, cameraDistance)).x;
		maxY = Camera.main.ViewportToWorldPoint (new Vector3 (0, 1, cameraDistance)).y;
		minX = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, cameraDistance)).x;
		minY = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, cameraDistance)).y;
		size = GetComponent<Renderer> ().bounds.size;
		startPos = transform.position - (transform.right * 0.738f);

		// Run update before bullet is drawn
		this.Update ();
	}

	void Update ()
	{
		transform.Translate (new Vector3 (1, 0, 0) * speed * Time.deltaTime);

		if (transform.position.x > maxX + size.x / 2
		    || transform.position.x < minX - size.x / 2
		    || transform.position.y > maxY + size.y / 2
		    || transform.position.y < minY - size.y / 2) {
			Destroy (gameObject);
			return;
		}

		var layerMask = ~((1 << gameObject.layer) | miscLayer);
		RaycastHit2D hitInfo = Physics2D.Linecast (startPos, transform.position, layerMask);
		if (hitInfo.collider != null) {
			if (hitInfo.collider.gameObject.tag == "Enemy") {
				var enemyHealth = hitInfo.collider.gameObject.GetComponent<Health> ();
				enemyHealth.Damage (damage);
				if(audioSrc != null)
				{
					//Enemy hurt sound effect has been set. 
					//Play the Sound. 
					audioSrc.Play();
				}
			}
			Destroy (gameObject);
		}
	}

}
