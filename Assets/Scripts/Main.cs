using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
	
	public List<Dictionary<string,object>> featureData;
	private List<int> gameboardIDs;
	private List<string> gameboardImages;

	public string gameState;

	public string currentFeaturedID;
	public string currentFeaturedURL;

	public string currentMajor;
	public string currentMinor;

	void Awake() {
		gameState = "Init";
		//
		gameboardIDs = new List<int>();
		gameboardImages = new List<string>();
		//
		LoadGameboardData();
		LoadBeaconCSVfile();
	}

	void Start () {
		// Debug.Log ( "GALLERY : " + GetGalleryIDfromBeacon (401.ToString (), 5349.ToString ()) );
		// Debug.Log ( "IMAGE : " + GetImageURL(GetFeaturedIDfromBeacon (401.ToString (), 5349.ToString ())) );
		//currentFeaturedID = GetFeaturedIDfromBeacon (401.ToString (), 5349.ToString ());
		//currentFeaturedURL = GetImageURL(GetFeaturedIDfromBeacon (401.ToString (), 5349.ToString ()));
		//
		//StartCoroutine("LoadFeaturedTexture");
	}

	void ResetGame() {
		currentFeaturedID = "-1";
	}
		
	void Update () {
		switch (gameState) {
			case "Init":
				break;
			case "TitleScreen":
				break;
			case "PlayGame":
				break;
			case "ScanForFeaturedID":
				break;
			case "SetFeaturedID":
				break;
			case "SetFeaturedIDWait":
				break;
			case "MatchFeaturedImage":
				break;
			case "MatchFeaturedImageWait":
				break;
			case "GameOver":
				break;
		}
	}	

	private IEnumerator LoadFeaturedTexture() {
		while (true) {

			yield return new WaitForSeconds(5.0f);

			Debug.Log (currentFeaturedURL);
			/*
			gameState = "SetFeaturedIDWait";
	        // Start a download of the given URL
			WWW www = new WWW(currentFeaturedURL);

	        // Wait for download to complete
	        yield return www;

	        // assign texture
	        Renderer renderer = GetComponent<Renderer>();
	        renderer.material.mainTexture = www.texture;*/

			Debug.Log ("TExTURE");

			StopCoroutine ("LoadFeaturedTexture");
		}
    }

	//--------------------------------------------------------------------------------------------------------------//
	//
	// Gameboard Object ID's and Gameboard Image URLs to load into Google Vision API
	//
	//--------------------------------------------------------------------------------------------------------------//
	void LoadGameboardData(){
		gameboardIDs.Add(50794);
		gameboardIDs.Add(51698);
		gameboardIDs.Add(59212);
		gameboardIDs.Add(59213);
		gameboardIDs.Add(59605);
		gameboardIDs.Add(101833);
		gameboardIDs.Add(102742);
		gameboardIDs.Add(102789);
		gameboardIDs.Add(102891);
		gameboardIDs.Add(102952);
		gameboardIDs.Add(102954);
		gameboardIDs.Add(102968);
		gameboardIDs.Add(103004);
		gameboardIDs.Add(103008);
		gameboardIDs.Add(103503);
		gameboardIDs.Add(103602);
		gameboardIDs.Add(103620);
		gameboardIDs.Add(103628);
		gameboardIDs.Add(103803);
		gameboardIDs.Add(103806);
		gameboardIDs.Add(104330);
		gameboardIDs.Add(104360);
		gameboardIDs.Add(104391);
		gameboardIDs.Add(104396);
		gameboardIDs.Add(104479);
		//
		gameboardImages.Add("gs://pma-image-bucket/1949-53-1-ov.jpg");
		gameboardImages.Add("gs://pma-image-bucket/1950-134-94-CX.jpg");
		gameboardImages.Add("gs://pma-image-bucket/1963-116-8after.jpg");
		gameboardImages.Add("gs://pma-image-bucket/1963-116-9-pma.jpg");
		gameboardImages.Add("gs://pma-image-bucket/1963-181-76-CX.jpg");
		gameboardImages.Add("gs://pma-image-bucket/Cat1173a.jpg");
		gameboardImages.Add("gs://pma-image-bucket/Cat908-pma.jpg");
		gameboardImages.Add("gs://pma-image-bucket/Cat949.jpg");
		gameboardImages.Add("gs://pma-image-bucket/E1924-3-32-CX.jpg");
		gameboardImages.Add("gs://pma-image-bucket/E1924-3-88.jpg");
		gameboardImages.Add("gs://pma-image-bucket/E1924-3-9-ov.jpg");
		gameboardImages.Add("gs://pma-image-bucket/E1924-4-13.jpg");
		gameboardImages.Add("gs://pma-image-bucket/E1972-2-1.jpg");
		gameboardImages.Add("gs://pma-image-bucket/E1975-1-1.jpg");
		gameboardImages.Add("gs://pma-image-bucket/F1938-1-2-pma.jpg");
		gameboardImages.Add("gs://pma-image-bucket/inv155.jpg");
		gameboardImages.Add("gs://pma-image-bucket/Inv203after-cons.jpg");
		gameboardImages.Add("gs://pma-image-bucket/Inv2095-pma2016.jpg");
		gameboardImages.Add("gs://pma-image-bucket/M1928-1-16.jpg");
		gameboardImages.Add("gs://pma-image-bucket/M1928-1-19-CX.jpg");
		gameboardImages.Add("gs://pma-image-bucket/W1893-1-106-CX.jpg");
		gameboardImages.Add("gs://pma-image-bucket/W1893-1-65-pma.jpg");
		gameboardImages.Add("gs://pma-image-bucket/W1900-1-5-nyd.jpg");
		gameboardImages.Add("gs://pma-image-bucket/W1901-1-5.jpg");
		gameboardImages.Add("gs://pma-image-bucket/ W1959-1-1-CX.jpg");
	}
	
	//--------------------------------------------------------------------------------------------------------------//
	//
	// Public functions to Locate Featured Gallery Pieces from iBeacon data array
	//
	//--------------------------------------------------------------------------------------------------------------//

	// Load CSV file imto data array
	void LoadBeaconCSVfile() {
		featureData = CSVReader.Read ("ibeacons");
		// Output data to console
		for(var i=0; i < featureData.Count; i++) {
			print ( "gallery " + featureData[i]["gallery"] + " " +
				"major " + featureData[i]["major"] + " " +
				"minor " + featureData[i]["minor"] + " " +
				"featureid " + featureData[i]["featureid"] + " " +
				"url " + featureData[i]["url"] );
		}

	}

	// Get Museum Gallery Number
	public string GetGalleryIDfromBeacon(string _major, string _minor){
		string _galleryID = "-1";
		for (var i = 0; i < featureData.Count; i++) {
			if (featureData[i]["major"].ToString() == _major && featureData[i]["minor"].ToString() == _minor ){
				_galleryID = featureData[i]["gallery"].ToString();
			}
		}
		return _galleryID;
	}

	// Get Featured Museum Piece ID
	public string GetFeaturedIDfromBeacon(string _major, string _minor){
		string _featureID = "-1";
		for (var i = 0; i < featureData.Count; i++) {
			if (featureData[i]["major"].ToString() == _major && featureData[i]["minor"].ToString() == _minor ){
				_featureID = featureData[i]["featureid"].ToString();
			}
		}
		return _featureID;
	}

	// Get Featured Museum Piece Image URL
	public string GetImageURL(string _featureid){
		string _imageURL = "-1";
		for (var i = 0; i < featureData.Count; i++) {
			if (featureData[i]["featureid"].ToString() == _featureid ){
				_imageURL = featureData[i]["url"].ToString();
			}
		}
		return _imageURL;
	}

	//--------------------------------------------------------------------------------------------------------------//

}
