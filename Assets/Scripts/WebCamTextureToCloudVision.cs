using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WebCamTextureToCloudVision : MonoBehaviour {

	public string phoneNumber;

	private string gameState;

	private string mediaUploadURL;
	public GameObject imageUploader;

	public string url = "https://vision.googleapis.com/v1/images:annotate?key=";
	public string apiKey = "";
	public float captureIntervalSeconds = 15.0f;
	public int requestedWidth = 640;
	public int requestedHeight = 480;
	public FeatureType featureType = FeatureType.WEB_DETECTION;
	public int maxResults = 1;

	public GameObject startScreen;
	public GameObject imageMenu;
	public GameObject checkMark;

	private string imageURL;

	WebCamTexture webcamTexture;
	Texture2D texture2D;
	Dictionary<string, string> headers;

	[System.Serializable]
	public class AnnotateImageRequests {
		public List<AnnotateImageRequest> requests;
	}

	[System.Serializable]
	public class AnnotateImageRequest {
		public Image image;
		public List<Feature> features;
	}

	[System.Serializable]
	public class Image {
		public string content;
		public Source source;
	}

	[System.Serializable]
	public class Source {
		public string gcsImageUri;
	}

	[System.Serializable]
	public class Feature {
		public string type;
		public int maxResults;
	}

	[System.Serializable]
	public class ImageContext {
		public LatLongRect latLongRect;
		public List<string> languageHints;
	}

	[System.Serializable]
	public class LatLongRect {
		public LatLng minLatLng;
		public LatLng maxLatLng;
	}

	[System.Serializable]
	public class AnnotateImageResponses {
		public string access_token;
		//public List<AnnotateImageResponse> responses;
	}

	[System.Serializable]
	public class AnnotateImageResponse {
		//public List<FaceAnnotation> faceAnnotations;
	    //public List<EntityAnnotation> landmarkAnnotations;
		//public List<EntityAnnotation> logoAnnotations;
		//public List<EntityAnnotation> labelAnnotations;
		//public List<EntityAnnotation> textAnnotations;
		public string access_token;
	}

	[System.Serializable]
	public class WebDetection {
		public List<WebEntities> webEntities;
		public List<PartialMatchingImages> partialMatchingImages;
		public List<PagesWithMatchingImages> pagesWithMatchingImages;
	}

	[System.Serializable]
	public class WebEntities {
		public string entityId;
		public float score;
		public string description;
	}

	[System.Serializable]
	public class PartialMatchingImages {
		public string url;
		public float score;
	}

	[System.Serializable]
	public class PagesWithMatchingImages {
		public string url;
		public float score;
	}

	[System.Serializable]
	public class FaceAnnotation {
		public BoundingPoly boundingPoly;
		public BoundingPoly fdBoundingPoly;
		public List<Landmark> landmarks;
		public float rollAngle;
		public float panAngle;
		public float tiltAngle;
		public float detectionConfidence;
		public float landmarkingConfidence;
		public string joyLikelihood;
		public string sorrowLikelihood;
		public string angerLikelihood;
		public string surpriseLikelihood;
		public string underExposedLikelihood;
		public string blurredLikelihood;
		public string headwearLikelihood;
	}

	[System.Serializable]
	public class EntityAnnotation {
		public string mid;
		public string locale;
		public string description;
		public float score;
		public float confidence;
		public float topicality;
		public BoundingPoly boundingPoly;
		public List<LocationInfo> locations;
		public List<Property> properties;
	}

	[System.Serializable]
	public class BoundingPoly {
		public List<Vertex> vertices;
	}

	[System.Serializable]
	public class Landmark {
		public string type;
		public Position position;
	}

	[System.Serializable]
	public class Position {
		public float x;
		public float y;
		public float z;
	}

	[System.Serializable]
	public class Vertex {
		public float x;
		public float y;
	}

	[System.Serializable]
	public class LocationInfo {
		LatLng latLng;
	}

	[System.Serializable]
	public class LatLng {
		float latitude;
		float longitude;
	}

	[System.Serializable]
	public class Property {
		string name;
		string value;
	}

	public enum FeatureType {
		TYPE_UNSPECIFIED,
		FACE_DETECTION,
		LANDMARK_DETECTION,
		LOGO_DETECTION,
		LABEL_DETECTION,
		TEXT_DETECTION,
		SAFE_SEARCH_DETECTION,
		WEB_DETECTION,
		IMAGE_PROPERTIES
	}

	public enum LandmarkType {
		UNKNOWN_LANDMARK,
		LEFT_EYE,
		RIGHT_EYE,
		LEFT_OF_LEFT_EYEBROW,
		RIGHT_OF_LEFT_EYEBROW,
		LEFT_OF_RIGHT_EYEBROW,
		RIGHT_OF_RIGHT_EYEBROW,
		MIDPOINT_BETWEEN_EYES,
		NOSE_TIP,
		UPPER_LIP,
		LOWER_LIP,
		MOUTH_LEFT,
		MOUTH_RIGHT,
		MOUTH_CENTER,
		NOSE_BOTTOM_RIGHT,
		NOSE_BOTTOM_LEFT,
		NOSE_BOTTOM_CENTER,
		LEFT_EYE_TOP_BOUNDARY,
		LEFT_EYE_RIGHT_CORNER,
		LEFT_EYE_BOTTOM_BOUNDARY,
		LEFT_EYE_LEFT_CORNER,
		RIGHT_EYE_TOP_BOUNDARY,
		RIGHT_EYE_RIGHT_CORNER,
		RIGHT_EYE_BOTTOM_BOUNDARY,
		RIGHT_EYE_LEFT_CORNER,
		LEFT_EYEBROW_UPPER_MIDPOINT,
		RIGHT_EYEBROW_UPPER_MIDPOINT,
		LEFT_EAR_TRAGION,
		RIGHT_EAR_TRAGION,
		LEFT_EYE_PUPIL,
		RIGHT_EYE_PUPIL,
		FOREHEAD_GLABELLA,
		CHIN_GNATHION,
		CHIN_LEFT_GONION,
		CHIN_RIGHT_GONION
	};

	public enum Likelihood {
		UNKNOWN,
		VERY_UNLIKELY,
		UNLIKELY,
		POSSIBLE,
		LIKELY,
		VERY_LIKELY
	}

	// Use this for initialization
	void Start () {
		imageUploader = GameObject.FindGameObjectWithTag ("Player");
		//
		headers = new Dictionary<string, string>();
		headers.Add("Content-Type", "application/json; charset=UTF-8");

		//if (apiKey == null || apiKey == "")
			//Debug.LogError("No API key. Please set your API key into the \"Web Cam Texture To Cloud Vision(Script)\" component.");
		
		WebCamDevice[] devices = WebCamTexture.devices;
		for (var i = 0; i < devices.Length; i++) {
			Debug.Log (devices [i].name);
		}
		if (devices.Length > 0) {
			webcamTexture = new WebCamTexture(devices[0].name);
			Renderer r = GetComponent<Renderer> ();
			if (r != null) {
				Material m = r.material;
				if (m != null) {
					m.mainTexture = webcamTexture;
				}
			}

		}	
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void ShowCameraTexture(){
		startScreen.SetActive (false);
		imageMenu.SetActive (true);
		webcamTexture.Play();
	}

	/*
	// Start Image Search
	//--------------------------------------------------------------------------------------------------------------//
	public void SetImageSearch1() {
		imageURL = "gs://pma-image-bucket/1949-53-1-ov.jpg";
		StartCoroutine("Set");
	}
	public void SetImageSearch2() {
		imageURL = "gs://pma-image-bucket/1963-116-11-pma.jpg";
		StartCoroutine("Set");
	}
	public void SetImageSearch3() {
		imageURL = "gs://pma-image-bucket/1963-181-58-CX.jpg";
		StartCoroutine("Set");
	}

	private IEnumerator Set() {
		while (true) {

			yield return new WaitForSeconds(captureIntervalSeconds);

#if UNITY_WEBGL	
			//Application.ExternalCall("post", this.gameObject.name, "OnSuccessFromBrowser", "OnErrorFromBrowser", this.url + this.apiKey, base64, this.featureType.ToString(), this.maxResults);
#else

			AnnotateImageRequests requests = new AnnotateImageRequests();
			requests.requests = new List<AnnotateImageRequest>();

			AnnotateImageRequest request = new AnnotateImageRequest();
			//request.image.content = base64;
			request.image = new Image();
			request.image.source = new Source();
			request.image.source.gcsImageUri = imageURL;
			//
			request.features = new List<Feature>();
			Feature feature = new Feature();
			feature.type = this.featureType.ToString();
			feature.maxResults = this.maxResults;

			request.features.Add(feature); 
			requests.requests.Add(request);

			string jsonData = JsonUtility.ToJson(requests, false);
			if (jsonData != string.Empty) {
				string url = this.url + this.apiKey;
				byte[] postData = System.Text.Encoding.Default.GetBytes(jsonData);
				using(WWW www = new WWW(url, postData, headers)) {
					yield return www;
					if (www.error == null) {
						Debug.Log("SOURCE");
						Debug.Log(www.text.Replace("\n", "").Replace(" ", ""));
						AnnotateImageResponses responses = JsonUtility.FromJson<AnnotateImageResponses>(www.text);
						// SendMessage, BroadcastMessage or someting like that.
						Sample_OnAnnotateImageResponses(responses);
					} else {
						Debug.Log("Error: " + www.error);
					}
				}
			}
			StopCoroutine("Set");
#endif
		}
	}

	public void DoImageSearch() {
		StartCoroutine("Capture");
	}

	private IEnumerator Capture() {
		while (true) {
			//if (this.apiKey == null)
				//yield return null;

			yield return new WaitForSeconds(captureIntervalSeconds);

			Color[] pixels = webcamTexture.GetPixels();
			if (pixels.Length == 0)
				yield return null;
			if (texture2D == null || webcamTexture.width != texture2D.width || webcamTexture.height != texture2D.height) {
				texture2D = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGBA32, false);
			}

			texture2D.SetPixels(pixels);
			// texture2D.Apply(false); // Not required. Because we do not need to be uploaded it to GPU
			byte[] jpg = texture2D.EncodeToJPG();
			string base64 = System.Convert.ToBase64String(jpg);
#if UNITY_WEBGL	
			Application.ExternalCall("post", this.gameObject.name, "OnSuccessFromBrowser", "OnErrorFromBrowser", this.url + this.apiKey, base64, this.featureType.ToString(), this.maxResults);
#else
			
			AnnotateImageRequests requests = new AnnotateImageRequests();
			requests.requests = new List<AnnotateImageRequest>();

			AnnotateImageRequest request = new AnnotateImageRequest();
			request.image = new Image();
			request.image.content = base64;
			request.features = new List<Feature>();

			Feature feature = new Feature();
			feature.type = this.featureType.ToString();
			feature.maxResults = this.maxResults;

			request.features.Add(feature); 
		
			requests.requests.Add(request);

			string jsonData = JsonUtility.ToJson(requests, false);
			if (jsonData != string.Empty) {
				string url = this.url + this.apiKey;
				byte[] postData = System.Text.Encoding.Default.GetBytes(jsonData);
				using(WWW www = new WWW(url, postData, headers)) {
					yield return www;
					if (www.error == null) {
						Debug.Log("CAMERA");
						Debug.Log(www.text.Replace("\n", "").Replace(" ", ""));
						AnnotateImageResponses responses = JsonUtility.FromJson<AnnotateImageResponses>(www.text);
						// SendMessage, BroadcastMessage or someting like that.
						Sample_OnAnnotateImageResponses(responses);
					} else {
						Debug.Log("Error: " + www.error);
					}
				}
			}
#endif
		}
	}

#if UNITY_WEBGL
	void OnSuccessFromBrowser(string jsonString) {
		Debug.Log(jsonString);	
		AnnotateImageResponses responses = JsonUtility.FromJson<AnnotateImageResponses>(jsonString);
		Sample_OnAnnotateImageResponses(responses);
	}

	void OnErrorFromBrowser(string jsonString) {
		Debug.Log(jsonString);	
	}
#endif

	/// <summary>
	/// A sample implementation.
	/// </summary>
	void Sample_OnAnnotateImageResponses(AnnotateImageResponses responses) {
		Debug.Log ("IMAGE SEARCH RESULTS : " + responses.responses [0].webDetection.webEntities[0].entityId);
		Debug.Log ("IMAGE SEARCH RESULTS : " + responses.responses [0].webDetection.webEntities[1].entityId);
		Debug.Log ("IMAGE SEARCH RESULTS : " + responses.responses [0].webDetection.webEntities[2].entityId);

	}

	//---------------------------------------------------------------------------------//
	// End Image Search
	*/




	// Start Image Capture
	//---------------------------------------------------------------------------------//

	// Create Screenshot & upload to Imgur site
	public void CaptureScreenShot()
	{
		//clearScreens ();
		//takePhotoScreen.SetActive (false);
		//HideHeader ();
		gameState = "PhotoCaptureWait";
		imageUploader.GetComponent<ImageUploader>().MakeScreenshot ();
		Debug.Log ("MAKE SCREENSHOT");
		//ShowSharePhfoto();
	}

	public void PhotoUploaded() {
		Debug.Log ("HANDLE SCREENSHOT");
		//HandleOnUploadSucceed ();
		mediaUploadURL = imageUploader.GetComponent<ImageUploader>().LinkToScreenShot;
		GetClarifaiToken();
	}

	public void CaptureScreenShotWait(){
		gameState = "CaptureScreenShotWait";
	}

	// Record GIF & upload to Imgur site


	public void HandleOnUploadSucceed ()
	{
		//Set link to textbox to show
		mediaUploadURL = imageUploader.GetComponent<ImageUploader>().LinkToScreenShot;
		Debug.Log(mediaUploadURL);
		SendPhoto();
	}

	public void ShowPhotoTexture ()
	{
		
	}


	public void SendPhoto(){
		StartCoroutine(SendTwilioMMS());
	}

	IEnumerator SendTwilioMMS() {
		Debug.Log("A");
		// Twilio Phone #'s
		// (256)673-2215
		// (561)935-6732
		// (305)707-4541
		// (917)924-5326 
		// Twilio API Parameters
		WWWForm form = new WWWForm();
		form.AddField( "From", "+19179245326" );
		//form.AddField( "From", "+19179245326" );
		//form.AddField( "From", "+15619356732" );
		//form.AddField( "From", "+13057074541" );
		//form.AddField( "From", "+12566732215" );
		form.AddField( "To", "+1" + phoneNumber );
		form.AddField( "MediaUrl", mediaUploadURL );
		form.AddField( "Body", "Wound Type: Venous ulcer - Recommended Treatment: Skin grafts and compressions dressings");
		byte[] rawData = form.data;

		Debug.Log("B");
		
		// Twilio Account URL
		string url = "https://api.twilio.com/2010-04-01/Accounts/AC1474ff453d4368bfa8f3633db1551263/Messages";

		// Add a custom header to the request.
		// In this case a basic authentication to access a password protected resource.
		Dictionary<string, string> dict_api = new Dictionary<string, string>();
		string auth = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("AC1474ff453d4368bfa8f3633db1551263:aa6c4006c5cca82c3162f03bec41a033"));
		dict_api.Add("Authorization", auth);

		// Post a request to an URL with our custom headers
		WWW www = new WWW(url, rawData, dict_api);
		yield return www;

		Debug.Log("PHOTO SENT");
		//
		//FileUtil.DeleteFileOrDirectory( recorder.SavedFilePath );
		//
		if (System.IO.File.Exists(UnityEngine.Application.dataPath + "/StreamingAssets/ARscreenshot.png")){
			System.IO.File.Delete (UnityEngine.Application.dataPath + "/StreamingAssets/ARscreenshot.png");
		}
		Debug.Log("C");
		//ShowExitScreen ();
		//

	}

	//---------------------------------------------------------------------------------//
	// End Image Capture

	// Start Clarifai
	//---------------------------------------------------------------------------------//
	private void GetClarifaiToken()
	{
		StartCoroutine(SendClarifaiClientID());
	}
	
	IEnumerator SendClarifaiClientID() {
		
		// Clarifai Token Parameters
		WWWForm form = new WWWForm();
		form.AddField( "client_id", "c_2Z_Q3QUuXqEQi19KhRkrX970fiYYxUOrvuJbWh" );
		form.AddField( "client_secret", "RP0wgwWdU0RpTAAdb-KTWM3zvJs4zJG3ID--S7JI" );
		form.AddField( "grant_type", "client_credentials" );
		byte[] rawData = form.data;
		
		// Clarifai Account URL
		string url = "https://api.clarifai.com/v1/token/";
		
		// In this case a basic authentication to access a password protected resource.
		Dictionary<string, string> dict_api = new Dictionary<string, string>();
		
		// Post a request to an URL with our custom headers
		WWW www = new WWW(url, rawData, dict_api);
		yield return www;
		
		Debug.Log(www.text);
		//if (www.error == null) {
			AnnotateImageResponses responses = JsonUtility.FromJson<AnnotateImageResponses>(www.text);
			// SendMessage, BroadcastMessage or someting like that.
			Sample_OnAnnotateImageResponses(responses);
		//}
		
		Debug.Log("A"+mediaUploadURL);
		// Clarifai Image URL Parameters
		WWWForm form2 = new WWWForm();
		form.AddField( "url", "mediaUploadURL" );
		//form.AddField( "model", "wound" );
		//form.AddField( "id", "e802feb160ec45de8e58ecddbc628801" );
		byte[] rawData2 = form2.data;
		
		// Clarifai Account URL
		string url2 = "https://api.clarifai.com/v1/tag/";
		
		// In this case a basic authentication to access a password protected resource.
		Dictionary<string, string> dict_api2 = new Dictionary<string, string>();
		string auth2 = "Bearer " + responses.access_token;
		dict_api2.Add("Authorization", auth2);
		
		// Post a request to an URL with our custom headers
		WWW www2 = new WWW(url2, rawData2, dict_api2);
		yield return www2;
		
		Debug.Log(www2.text);
		//
		SendPhoto();
		
	}
	
	void Sample_OnAnnotateImageResponses(AnnotateImageResponses responses) {
		Debug.Log ("Token RESULTS : " + responses.access_token);

	}


}
