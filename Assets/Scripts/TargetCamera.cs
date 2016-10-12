using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TargetCamera : MonoBehaviour
{
	public bool editMode = true;
	public GameObject fpCamera; //First-person Camera

	[Header("---------------")]

	public int shotNum;
	public Text shotCounter, shotRating;
	public Image checkMark;

	public Rect camRectNormal; //Pulled from camera.rect

	void Start()
	{
		//Load all the shots from playerPrefs
		Shot.LoadShots();

		//If there were shots stored in PlayerPrefs
		if (Shot.shots.Count > 0)
		{
			shotNum = 0;
			ShowShot(Shot.shots[shotNum]);
		}

		//Hide the cursor (Note: this doesn't work in the Unity Editr unless the Game pane is set to Maximize on Play)
		Cursor.visible = false;
	}

	// Update is called once per frame
	void Update()
	{
		Shot sh;

		//Mouse input
		if (Input.GetMouseButtonDown(0)) //Left mouse button
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

			//Position the targetCamera with the Shot
			ShowShot(sh);

			Utils.tr(sh.ToXML());

			//Record a new shot
			Shot.shots.Add(sh);
			shotNum = Shot.shots.Count - 1;
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
		shotRating.text = "";
	}

	public void ShowShot(Shot sh)
	{
		//Position the TargetCamera with the Shot
		transform.position = sh.position;
		transform.rotation = sh.rotation;
	}
}
