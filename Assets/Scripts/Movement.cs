﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{

	public static GameObject[] waypointArray;
	public static int enemiesCount = 0;


	public float spawnInterval;
	public float speed;
	public GameObject bullet;
	public LineRenderer shootLine;

	private float lastBulletTime = 0f;
    public bool playerCanMove = true;
    public WeaponObj[] availableWeapons;
    public int currentWeapon = 0;
    public int currentClip;
    public int gunAmmo;
    public bool purchasedMinimap = false;
    public Text ammoText;
    public Text gunCapacityText;
	public Text enemiesCountText;
	public AudioSource enemy_hit_sound;
	public AudioSource bullet_sound;
	// Use this for initialization
	void Start ()
	{
		waypointArray = GameObject.FindGameObjectsWithTag ("Enemy");
		Movement.enemiesCount = waypointArray.Length;
		enemiesCountText.text = Movement.enemiesCount.ToString();
		UpdateGun ();
        
	}

	// Update is called once per frame
	void Update ()
	{
        //Update position of player
        if (playerCanMove)
        {
            var currentPosition = transform.position;
            currentPosition.x += Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;
            currentPosition.y += Input.GetAxisRaw("Vertical") * speed * Time.deltaTime;
            transform.position = new Vector3(currentPosition.x, currentPosition.y);
        }
		

		// Update player direction.
		faceMouse ();

		// Fire button is pressed.
		if (Input.GetButton ("Fire1")) {
			// TODO: Different functions can be called based on weapon type. For now, just gun.
			fireGun ();
		}
        if (Input.GetKeyDown(KeyCode.R)){
            gunAmmo += currentClip;
            currentClip = 0;
            StartCoroutine(ReloadGun());
        }

		enemiesCountText.text = Movement.enemiesCount.ToString();
	}

	public void UpdateGun() {
		currentClip = availableWeapons[currentWeapon].clipCapacity;
		gunAmmo = availableWeapons[currentWeapon].ammoCapacity;
		ammoText.text = currentClip.ToString();
		gunCapacityText.text = gunAmmo.ToString();
	}

	private void faceMouse ()
	{
		// Get the in-world mouse position using the screen mouse position.
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		// The direction vector between the character and the mouse.
		transform.up = new Vector3 (
			mousePosition.x - transform.position.x,
			mousePosition.y - transform.position.y,
			0
		);
	}
    public IEnumerator ReloadGun()
    {
        yield return new WaitForSeconds(1);
        if(gunAmmo < availableWeapons[currentWeapon].clipCapacity)
        {
            currentClip = gunAmmo;
            gunAmmo = 0;
        } else
        {
            currentClip = availableWeapons[currentWeapon].clipCapacity;
            gunAmmo -= currentClip;
        }
        ammoText.text = currentClip.ToString();
        gunCapacityText.text = gunAmmo.ToString();
    }

	private void fireGun ()
	{
        var currentGun = availableWeapons[currentWeapon];
		if (Time.time - lastBulletTime >= currentGun.fireRate && currentClip > 0) {
			lastBulletTime = Time.time;
            currentClip -= 1;
            ammoText.text = currentClip.ToString();
            BulletPlayer.speed = currentGun.speed;
            BulletPlayer.damage = currentGun.damage;
            BulletPlayer.range = currentGun.range;
			BulletPlayer.audioSrc = enemy_hit_sound;
            if(currentClip == 0) StartCoroutine(ReloadGun());
			if (bullet_sound != null)
			{
				Debug.Log("bullet fired");
				bullet_sound.Play();
			}
			var bulletIns = Instantiate (bullet, transform.position, transform.rotation);
			bulletIns.transform.Translate (new Vector3 (0.303f, 0.738f, 0)); 
			bulletIns.transform.Rotate (Vector3.forward * 90);
			bulletIns.layer = LayerMask.NameToLayer ("Player");
		}
	}

}
