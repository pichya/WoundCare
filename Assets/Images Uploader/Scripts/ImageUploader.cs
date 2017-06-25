using System;
using System.IO;
using System.Xml;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ImageUploader : MonoBehaviour 
{
	#region Public members
	/// <summary>
	/// Event that is occured if upload was succeed.
	/// </summary>
	public event UploadDelegate OnUploadSucceed;
	public delegate void UploadDelegate();

	public GameObject gameEngine;
    
	[HideInInspector]
	/// <summary>
	/// Path for screenshot on the device
	/// </summary>
	public string PathToScreenShot = string.Empty;
	[HideInInspector]
	/// <summary>
	/// Link for uploaded screenshot
	/// </summary>
	public string LinkToScreenShot = string.Empty;

	/// <summary>
	/// Last captured screenshot
	/// </summary>
	[HideInInspector]
	public Texture2D Screenshot = null;

	/// <summary>
	/// Register and get it on imgur, for using the func.
	/// Look on Readme, for proper instructions
	/// </summary>
	public string imgurClientID = string.Empty;
	// imgurClientID's
	// 9f9f1df17a47aea
	// 3bc17045e20d041
	// 5f9277a94f97040
	// b79c94ae050ef25

	/// <summary>
	/// Name for the screnshot
	/// </summary>
	public string ScreenShotName = "ARimage";
	#endregion

	#region Private Fields
	private string imgurURL = "https://api.imgur.com/3/upload.xml";
	//A hint variable, to raise an event when is uploaded, in main thread
	public bool isUploaded = false;
	#endregion

	#region Private methods	

	/// <summary>
	//IMPORTANT FOR WINDOWS & ANDROID PLATFORMS. DONT TOUCH LINE IN START().
	//Register a dummy callback for certification validation, that allow plugin to use SSL and proper port to use uploading functionality
	/// </summary>
	void Start()
	{
		gameEngine = GameObject.FindGameObjectWithTag ("GameController");
		ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
		//
	}

	/// <summary>
	/// As because of Unity is thread safety, we cannot work with it from another thread.
	/// We do here a simple hint, to avoid that problem.
	/// Set the flag when screenshot is uploaded, and raise an event.
	/// </summary>
	void Update()
	{
		if(isUploaded)
		{
				Debug.Log ("TEST:");
				gameEngine.GetComponent<WebCamTextureToCloudVision>().PhotoUploaded ();
				//gameEngine.GetComponent<WebCamTextureToCloudVision>().PhotoUploaded ();
				isUploaded = false;
        }
	}

	/// <summary>
	/// Asyncly uploads image stored in PathToScreenshot var to Imgur. 
	/// </summary>
	private void UploadToImgur()
	{
		//If app has been registered on Imgur site as it is described in readme
		if(imgurClientID != null)
		{
			using (var webClient = new WebClient())
			{
				//Get client ID from XML
				imgurClientID = "5f9277a94f97040";
				Debug.Log ("imgurClientID:"+imgurClientID);

				//Add client ID to request's header for autorisation
				webClient.Headers.Add("Authorization", "Client-ID " + imgurClientID);
				//Prepare the content to upload to server
				var values = new NameValueCollection
				{
					{ "image", Convert.ToBase64String(File.ReadAllBytes(this.PathToScreenShot)) }
				};
				//Register callback
				webClient.UploadValuesCompleted += UploadedCallback;
				//Make async operation
				try
				{
					webClient.UploadValuesAsync(new Uri(this.imgurURL), values);
				}
				catch(WebException webException)
				{
					Debug.Log(webException.Message);
				}
			}
		}
		else
		{
			Debug.Log("Imgur client ID is not assigned! Please register your app according to readme");
		}
	}
        
	/// <summary>
	/// Callback of upload method
	/// </summary>
	private void UploadedCallback(object sender, UploadValuesCompletedEventArgs e)
	{
		//If uploading has not cancelled and error has not accured
		if(!e.Cancelled && e.Error == null)
		{
			//Get the response
			byte[] response = e.Result;

			//Open it in XML document, to reach an uploaded link
			XmlDocument doc = new XmlDocument();
			doc.Load(new MemoryStream(response));
			//Find link element in XML and store it in a field
			XmlNode nodeImageLink = doc.GetElementsByTagName("link")[0];
			this.LinkToScreenShot = nodeImageLink.InnerText;
			this.isUploaded = true;
            Debug.Log("Uploaded successfully! Here is your link - " + LinkToScreenShot);
			//
			
		}
		else
		{
			LinkToScreenShot = string.Empty;
			//
			if(e.Cancelled)
			{
				Debug.Log("Uploading was cancelled");
			}
			else
			{
				Debug.Log(e.Error.Message);
			}
		}
	}

	/// <summary>
	/// Captures the screen shot and store a link to it in property
	/// </summary>
	private IEnumerator CaptureScreenShot()
	{
		//Wait untill the frame is ended, important to create a proper screenshot
		yield return new WaitForEndOfFrame();
		Capture();
    }

	/// <summary>
	/// A set of of actions that captures a screenshot and uploads it to Imgur.
	/// </summary>
	/// <returns>The and upload screenshot.</returns>
	private IEnumerator CaptureAndUploadScreenshot()
	{
		yield return new WaitForEndOfFrame();
		this.Capture();
		this.UploadToImgur();
    }

	/// <summary>
	/// Captures the screen shot and store a link to it in property
	/// </summary>
    private void Capture()
	{
		//Get a screenshot and store it in a field
		Screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		Screenshot.ReadPixels(new Rect(0,0, Screen.width, Screen.height), 0,0);
		Screenshot.name = this.ScreenShotName + ".png";
		Screenshot.Apply ();

		//Save screenshot
		byte[] allBytes = Screenshot.EncodeToPNG();
		
		//On mobile platforms - store in persistentDataPath
		if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			PathToScreenShot = Application.persistentDataPath + "/" + Screenshot.name;
			try
			{
				File.WriteAllBytes(PathToScreenShot, allBytes);
			}
			catch(IOException exc)
			{
				Debug.Log(exc.Message);
			}
		}
		//On other store to regular
		else
		{
			PathToScreenShot = UnityEngine.Application.dataPath + "/StreamingAssets/" + Screenshot.name;
			try
			{
           		File.WriteAllBytes(PathToScreenShot, allBytes);
			}
			catch(IOException exc)
			{
				Debug.Log(exc.Message);
			}
        }
    }
    

	    
	#endregion
	
	#region Public API methods

	/// <summary>
	/// Take a screenshot and upload
	/// </summary>
	public void Upload()
	{
		//Using coroutine because of need to wait for end of frame to make a screenshot
        StartCoroutine(this.CaptureAndUploadScreenshot());
    }

	/// <summary>
	/// Upload a set up image, by path to image
	/// </summary>
	/// <param name="pathToImage">Path to image.</param>ShowPreviewPhoto
	public void Upload(string pathToImage)
	{
		this.PathToScreenShot = pathToImage;
		this.UploadToImgur();
	}

	/// <summary>
	/// Make a screenshot, store it in a field and save on the device
	/// </summary>
	public void MakeScreenshot()
	{
		Debug.Log ("Capture");
		//Using coroutine because of need to wait for end of frame to make a screenshot
		StartCoroutine (this.CaptureAndUploadScreenshot ());
		gameEngine.GetComponent<WebCamTextureToCloudVision>().CaptureScreenShotWait ();
		Debug.Log("Wait");
	}

	#endregion	
}
