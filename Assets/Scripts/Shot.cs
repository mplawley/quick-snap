using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Shot //DOES NOT extend MonoBehaviour
{
	public static List<Shot> shots = new List<Shot>(); //List of all shots
	public static string prefsName = "QuickSnap_Shots";

	public Vector3 position; //Position of the camera
	public Quaternion rotation; //Rotation of the camera
	public Vector3 target; //Where the camera is pointed

	// Generates a single-line <shot> entry for an XML document
	public string ToXML()
	{
		string ss = "<shot ";
		//Position
		ss += "x=\"" + position.x + "\" ";
		ss += "y=\"" + position.y + "\" ";
		ss += "z=\"" + position.z + "\" ";
		//Rotation
		ss += "qx=\"" + rotation.x + "\" ";
		ss += "qy=\"" + rotation.y + "\" ";
		ss += "qz=\"" + rotation.z + "\" ";
		ss += "qw=\"" + rotation.w + "\" ";
		//Target
		ss += "tx=\"" + target.x + "\" ";
		ss += "ty=\"" + target.y + "\" ";
		ss += "tz=\"" + target.z + "\" ";
		//Close
		ss += " />";

		return ss;
	}

	//Takes a PT_XMLHashtable from PT_XMLReader of a <shot> entry in XML and parses it into a Shot
	public static Shot ParseShotXML(PT_XMLHashtable xHT)
	{
		Shot sh = new Shot();
		//Position
		sh.position.x = float.Parse(xHT.att("x"));
		sh.position.y = float.Parse(xHT.att("y"));
		sh.position.z = float.Parse(xHT.att("z"));
		//Rotation
		sh.rotation.x = float.Parse(xHT.att("qx"));
		sh.rotation.y = float.Parse(xHT.att("qy"));
		sh.rotation.z = float.Parse(xHT.att("qz"));
		sh.rotation.w = float.Parse(xHT.att("qw"));
		//Target
		sh.target.x = float.Parse(xHT.att("tx"));
		sh.target.y = float.Parse(xHT.att("ty"));
		sh.target.z = float.Parse(xHT.att("tz"));

		return sh;
	}

	//Loads all of the Shots from PlayerPrefs
	public static void LoadShots()
	{
		//Empty the shots List<Shot>
		shots = new List<Shot>();

		if (!PlayerPrefs.HasKey(prefsName))
		{
			//If there are no shots, return
			return;
		}

		//Get the full XML and parse it
		string shotsXML = PlayerPrefs.GetString(prefsName);
		PT_XMLReader xmlr = new PT_XMLReader();
		xmlr.Parse(shotsXML);

		//Pull the PT_XMLHashList of all <shot>s
		PT_XMLHashList hl = xmlr.xml["xml"][0]["shot"];
		for (int i = 0; i < hl.Count; i++)
		{
			//Parse each <shot> in the PT_XMLHashlist into a Shot
			PT_XMLHashtable ht = hl[i];
			Shot sh = ParseShotXML(ht);

			//Add it to the List<shot> shots
			shots.Add(sh);
		}
	}

	//Save List<Shot> shots to Player Prefers
	public static void SaveShots()
	{
		string xs = Shot.XML;

		Utils.tr(xs); //Trace all the XML to the Console

		//Set the PlayerPrefs
		PlayerPrefs.SetString(prefsName, xs);

		Utils.tr("PlayerPrefs." + prefsName + " has been set.");
	}

	//Convert all Shot.shots to XML
	public static string XML
	{
		get
		{
			//Start an XML string
			string xs = "<xml>\n";

			//Add each of the Shots as a <shot> in XML
			foreach (Shot sh in shots)
			{
				xs += sh.ToXML() + "\n";
			}

			//Add the closing XML tag
			xs += "</xml>";
			return xs;
		}
	}
}
