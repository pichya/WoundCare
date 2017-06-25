using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Demo : MonoBehaviour {

	public ImageUploader imageUploader;
	public Text text;
	public UnityEngine.UI.Image image;
	// Use this for initialization
	void Start () 
	{
		//Find a prefab of Image Uploader plugin
		imageUploader = FindObjectOfType<ImageUploader>();
		//Sign to event that would be raised after image is uploaded.
		imageUploader.OnUploadSucceed += HandleOnUploadSucceed;
	}

	void HandleOnUploadSucceed ()
	{
		//Set link to textbox to show
		text.text = imageUploader.LinkToScreenShot;
		//Set screenshot into image to show
		Sprite spriteFromTexture = Sprite.Create(imageUploader.Screenshot, new Rect(0, 0, imageUploader.Screenshot.width, imageUploader.Screenshot.height), new Vector2(0.5f,0.5f));
		image.sprite = spriteFromTexture;
    }
    
}
