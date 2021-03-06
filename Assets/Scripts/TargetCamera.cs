﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TargetCamera : MonoBehaviour
{
	public static TargetCamera S;

	public bool editMode = true;
	public GameObject fpCamera; //First-person Camera

	//Maximum deviation in Shot.position allows
	public float maxPosDeviation = 1f;

	//Maximum deviation in Shot.target allowed
	public float maxTarDeviation = 0.5f;

	//Easing for these deviations
	public string deviationEasing = Easing.Out;
	public float passingAccuracy = 0.7f;

	[Header("---------------")]

	public int shotNum;
	public Text shotCounter, shotRating;
	public Image checkMark;
	GameObject checkMarkGameObject;
	public Rect camRectNormal; //Pulled from camera.rect
	public Shot lastShot;
	public int numShots;
	public Shot[] playerShots;
	public float[] playerRatings;
	public GameObject whiteOut;

	[Header("Danger zone")]
	public bool checkToDeletePlayerPrefs = false; //Danger zone

	void Awake()
	{
		S = this;
	}

	void Start()
	{
		//Load all the shots from playerPrefs
		Shot.LoadShots();

		//If there were shots stored in PlayerPrefs
		if (Shot.shots.Count > 0)
		{
			shotNum = 0;
			ResetPlayerShotsAndRatings();
			ShowShot(Shot.shots[shotNum]);
		}

		//Hide the cursor (Note: this doesn't work in the Unity Editr unless the Game pane is set to Maximize on Play)
		Cursor.visible = false;

		camRectNormal = GetComponent<Camera>().rect;

		//Get checkMark Image as a GameObject so we can enable/disable it
		checkMarkGameObject = checkMark.gameObject;
	}

	void ResetPlayerShotsAndRatings()
	{
		numShots = Shot.shots.Count;

		//Initialize playerShots & playerRatings with default values
		playerShots = new Shot[numShots];
		playerRatings = new float[numShots];
	}

	// Update is called once per frame
	void Update()
	{
		Shot sh;

		//Mouse input
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) //If Left or Right mouse button is pressed this frame...
		{
			sh = new Shot();

			//Grab the position and rotation of fpCamera
			sh.position = fpCamera.transform.position;
			sh.rotation = fpCamera.transform.rotation;

			//Shoot a ray from the camera and see what it hits
			Ray ray = new Ray(sh.position, fpCamera.transform.forward);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit))
			{
				sh.target = hit.point;
			}

			if (editMode)
			{
				if (Input.GetMouseButtonDown(0))
				{
					//Left button records a new shot
					Shot.shots.Add(sh);
					shotNum = Shot.shots.Count - 1;
				}
				else if (Input.GetMouseButtonDown(1))
				{
					//Right button replaced the current shot
					Shot.ReplaceShot(shotNum, sh);
					ShowShot(Shot.shots[shotNum]);
				}

				//Reset information about the player when editing shots
				ResetPlayerShotsAndRatings();
			}
			else
			{
				//Test this shot against the current Shot
				float acc = Shot.Compare(Shot.shots[shotNum], sh);
				lastShot = sh;
				playerShots[shotNum] = sh;
				playerRatings[shotNum] = acc;

				//Show the shot just taken by the player
				ShowShot(sh);

				//Return to the current shot after waiting 1 second
				Invoke("ShowCurrentShot", 1);

				//Player the shutter sound
				this.GetComponent<AudioSource>().Play();
			}
		}

		//Keyboard input
		//Use Q and E keys to cycle shots
		//Note: either of these will throw an error if Shot.shots is empty
		if (Input.GetKeyDown(KeyCode.Q))
		{
			shotNum--;
			if (shotNum < 0)
			{
				shotNum = Shot.shots.Count - 1;
			}
			ShowShot(Shot.shots[shotNum]);
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			shotNum++;
			if (shotNum >= Shot.shots.Count)
			{
				shotNum = 0;
			}
			ShowShot(Shot.shots[shotNum]);
		}

		//If in editMode and left Shift is held down...
		if (editMode && Input.GetKey(KeyCode.LeftShift))
		{
			//Use Shift-S to Save
			if (Input.GetKeyDown(KeyCode.S))
			{
				Shot.SaveShots();
			}

			//Use Shift-X to output XML to console
			if (Input.GetKeyDown(KeyCode.X))
			{
				Utils.tr(Shot.XML);
			}
		}

		//Update the UI Texts
		shotCounter.text = (shotNum + 1).ToString() + " of " + Shot.shots.Count;
		if (Shot.shots.Count == 0)
		{
			shotCounter.text = "No shots exist"; //Shot.shots.Count doesn't require .ToString() because it is assumed when the left side of the + operator is a string
		}
		//shotRating.text = "";

		if (playerRatings.Length > shotNum && playerShots[shotNum] != null)
		{
			float rating = Mathf.Round(playerRatings[shotNum] * 100f);
			if (rating < 0)
			{
				rating = 0;
			}

			shotRating.text = rating.ToString() + "%";
			checkMarkGameObject.SetActive((playerRatings[shotNum] > passingAccuracy)); //The > comparison is used to generate true or false here
		}
		else
		{
			shotRating.text = "";
			checkMarkGameObject.SetActive(false);
		}

		//Hold tab to maximize the Target window
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			//Maximize when Tab is pressed
			GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
		}
		if (Input.GetKeyUp(KeyCode.Tab))
		{
			//Return to normal when Tab is released
			GetComponent<Camera>().rect = camRectNormal;
		}
	}

	public void ShowShot(Shot sh)
	{
		//Call WhiteOutTargetWindow() and let it handle its own timing
		StartCoroutine(WhiteOutTargetWindow());

		//Position the TargetCamera with the Shot
		transform.position = sh.position;
		transform.rotation = sh.rotation;
	}

	public void ShowCurrentShot()
	{
		ShowShot(Shot.shots[shotNum]);
	}

	public IEnumerator WhiteOutTargetWindow()
	{
		whiteOut.SetActive(true);
		yield return new WaitForSeconds(0.05f);
		whiteOut.SetActive(false);
	}

	//OnDrawGizmos() is called ANY time Gizmos need to be drawn, even when Unity isn't playing
	public void OnDrawGizmos()
	{
		List<Shot> shots = Shot.shots;
		for (int i = 0; i < shots.Count; i++)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(shots[i].position, 0.5f);

			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(shots[i].position, shots[i].target);

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(shots[i].target, 0.25f);
		}

		if (checkToDeletePlayerPrefs)
		{
			Shot.DeleteShots(); //Delete all the shots

			//Uncheck checkToDeletePlayerPrefs
			checkToDeletePlayerPrefs = false;
			shotNum = 0; //Set shotNum to 0
		}

		//Show the player's last shot attempt
		if (lastShot != null)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(lastShot.position, 0.25f);
			Gizmos.color = Color.white;
			Gizmos.DrawLine(lastShot.position, lastShot.target);
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(lastShot.target, 0.125f);
		}
	}
}
