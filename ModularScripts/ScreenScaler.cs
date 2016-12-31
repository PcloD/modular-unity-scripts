/* Script created by Lachlan McKay 2016 ©
 * This script allows you to scale a transform or text object based on the size of the viewport */

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenScaler : MonoBehaviour
{

	[Header("Main Options")]
	public Transform affectedObject;
	[Tooltip("Sets the type of scaling to perform.")]
	public mode targetType;
	public enum mode {transform, CanvasText, LegacyGUIText}
	[Tooltip("Enable this if you want this to only be activated by another script.")]
	public bool commandActivated = false;
	[Tooltip("Enable this if you want the script to execute slightly after other scripts.")]
	public bool useLateUpdate = false;

	[System.Serializable]
	public class ScreenScalerReferential
	{
		[Header("Reference Screen Size")]
		public Vector2 screenSize = new Vector2(1920f,1080f);

		[Header("Transform Reference Size")]
		[Tooltip("When the screen is at the above reference size, how big should the object's scale be?")]
		public Vector3 transformSize = Vector3.one;

		[Header("Font Reference Size")]
		[Tooltip("When the screen is at the above reference size, how big should the font be?")]
		public int fontSize = 26;
	}
	
	public ScreenScalerReferential Referential = new ScreenScalerReferential();

	[System.Serializable]
	public class ScreenScalerTransformSettings
	{
		[Header("Scaling Settings")]
		public bool scaleObjectOnXAxis = true;
		public bool scaleObjectOnYAxis = true;
		public bool scaleObjectOnZAxis = false;
		
		[Space(7)]
		[Tooltip("Should the object be scaled on the X axis based on screen width or height?")]
		public XAxis scaleXAxisBasedOnScreen;
		public enum XAxis {Width, Height}
		
		[Tooltip("Should the object be scaled on the Y axis based on screen width or height?")]
		public YAxis scaleYAxisBasedOnScreen;
		public enum YAxis {Width, Height}
		
		[Tooltip("Should the object be scaled on the Z axis based on screen width or height?")]
		public ZAxis scaleZAxisBasedOnScreen;
		public enum ZAxis {Width, Height}
	}
	
	public ScreenScalerTransformSettings TransformSettings = new ScreenScalerTransformSettings();


	[System.Serializable]
	public class ScreenScalerFontSettings
	{
		[Header("Font Settings")]
		public bool limitFontSize;
		[Tooltip("The minimum and maximum font size allowed")]
		public Vector2 fontSizeLimit = new Vector2(0,100);
		
		[Space(7)]
		[Tooltip("Should the font be scaled based on screen width or height?")]
		public fontAxis scaleBasedOnScreen;
		public enum fontAxis {Width, Height}
	}
	
	public ScreenScalerFontSettings FontSettings = new ScreenScalerFontSettings();

	[Header("Debug Options")]
	public bool debugConsole;
	public bool displayScreenSizeOnText;

	//Credits and description
	[Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script allows you to scale a transform, canvas text, or legacy GUIText based on the size of the viewport.";
    private string ScriptTags = "screen scaler scale size text viewport resize";
    private string ScriptCategory = "ui";

    //Private 
    private Vector2 screenScaleVector;	//Multiplier vector of how much the screen has grown / shrank since start

	private Vector3 currentScale;
	private int currentFontSize;

	private Text canvasText;
	private GUIText legacyGUIText;

	private bool active;

	void Start () {
		active = true;
		Setup();
	}

	void Setup() {

		if(!affectedObject) {
			affectedObject = transform;
		}

		switch(targetType) {

		case mode.transform:

			currentScale = affectedObject.localScale;

			break;

		case mode.CanvasText:

			canvasText = affectedObject.GetComponent<Text>();

			break;

		case mode.LegacyGUIText:

			legacyGUIText = affectedObject.GetComponent<GUIText>();

			break;

		}


	}

	void Update () {
	
		if(!useLateUpdate) {
			Master();
		}
	}

	void LateUpdate() {

		if(useLateUpdate) {
			Master();
		}
	}

	void Master() {

		if(active) {
			GetScreenSize();
			PrepVectors();
			ApplyVectors();
			Debugging();
		}

	}

	void GetScreenSize() {

		screenScaleVector.x = Screen.width / Referential.screenSize.x;
		screenScaleVector.y = Screen.height / Referential.screenSize.y;
	}

	void PrepVectors() {

		switch(targetType) {
			
		case mode.transform:

			if(TransformSettings.scaleObjectOnXAxis) {

				if(TransformSettings.scaleXAxisBasedOnScreen == ScreenScalerTransformSettings.XAxis.Width) {
					currentScale.x = screenScaleVector.x * Referential.transformSize.x;
				}

				if(TransformSettings.scaleXAxisBasedOnScreen == ScreenScalerTransformSettings.XAxis.Height) {
					currentScale.x = screenScaleVector.y * Referential.transformSize.x;
				}
			}

			if(TransformSettings.scaleObjectOnYAxis) {
				
				if(TransformSettings.scaleYAxisBasedOnScreen == ScreenScalerTransformSettings.YAxis.Width) {
					currentScale.y = screenScaleVector.x * Referential.transformSize.y;
				}
				
				if(TransformSettings.scaleYAxisBasedOnScreen == ScreenScalerTransformSettings.YAxis.Height) {
					currentScale.y = screenScaleVector.y * Referential.transformSize.y;
				}
			}

			if(TransformSettings.scaleObjectOnZAxis) {
				
				if(TransformSettings.scaleZAxisBasedOnScreen == ScreenScalerTransformSettings.ZAxis.Width) {
					currentScale.z = screenScaleVector.x * Referential.transformSize.z;
				}
				
				if(TransformSettings.scaleZAxisBasedOnScreen == ScreenScalerTransformSettings.ZAxis.Height) {
					currentScale.z = screenScaleVector.y * Referential.transformSize.z;
				}
			}

			
			break;
			
		case mode.CanvasText:
		case mode.LegacyGUIText:
			
			switch(FontSettings.scaleBasedOnScreen) {

			case ScreenScalerFontSettings.fontAxis.Width:

				currentFontSize = Mathf.RoundToInt((screenScaleVector.x * Referential.fontSize));

				break;

			case ScreenScalerFontSettings.fontAxis.Height:

				currentFontSize = Mathf.RoundToInt((screenScaleVector.y * Referential.fontSize));

				break;

			}
			
			break;
			
		}

	}

	void ApplyVectors() {

		if(FontSettings.limitFontSize) {
			currentFontSize = Mathf.RoundToInt(Mathf.Clamp(currentFontSize, FontSettings.fontSizeLimit.x, FontSettings.fontSizeLimit.y));
		}

		switch(targetType) {
			
		case mode.transform:
			
			affectedObject.localScale = currentScale;
			
			break;
			
		case mode.CanvasText:

			canvasText.fontSize = currentFontSize;

			break;

		case mode.LegacyGUIText:

			legacyGUIText.fontSize = currentFontSize;

			break;
			
		}

	}

	void Debugging() {

		if(displayScreenSizeOnText) {

			switch(targetType) {

			case mode.CanvasText:

				canvasText.text = "Width: " + Screen.width + " Height: " + Screen.height;

				break;

			case mode.LegacyGUIText:

				legacyGUIText.text = "Width: " + Screen.width + " Height: " + Screen.height;

				break;
			}

		}

	}

	//Enables or disables the script's update function
	public void ToggleScript(bool state) {
		active = state;
		if(debugConsole) {
			print ("Setting active state of ScreenScaler script to: " + state + " at time: " + Time.time);
		}
	}
}
