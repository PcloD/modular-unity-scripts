/* Script created by Lachlan McKay 2016 ©
 * This script allows you to scroll an array of instantiated images across the screen in any direction */

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImageWipe : MonoBehaviour
{

	[Header("Serialized Elements")]
	[Tooltip("The image that will be repeated and scrolled across the screen.")]
	public Sprite imageToApply;
	[Tooltip("The canvas to display the images on.")]
	public Transform canvasParent;
	[Tooltip("Allow this object to be used as a container for instantiated image objects?")]
	public bool moveSelfToCanvas = true;

	[Header("Dimensions")]
	[Tooltip("Choose whether to specify all units as a percentage of the screen or in pixels.")]
	public dims dimensionsUnit = dims.percentage;
	public enum dims {pixels, percentage}

	[Header("Image Specs")]
	[Range(1,100)]
	[Tooltip("How many images to instantiate.")]
	public int imageCount = 5;
	[Space(5)]
	[Tooltip("The distance between images.")]
	public Vector2 imageGap = new Vector2(0.2f,0.2f);
	[Space(5)]
	[Tooltip("The width delta of the image components.")]
	public float imageWidth = 0.5f;
	[Tooltip("The height delta of the image components.")]
	public float imageHeight = 0.25f;

	[Header("Origin and Direction")]
	[Tooltip("The starting point of each image.")]
	public Vector2 originPoint = new Vector2(0, 1.25f);
	[Tooltip("How far the effect will scroll.")]
	public Vector2 scrollDistance = new Vector2(0, 1.95f);

	[Header("Time")]
	[Tooltip("How long the effect will take to complete (in seconds).")]
	public float effectDuration = 1.0f;

	[Header("Misc")]
	[Tooltip("When using percentage dimensions mode, updates the images when the screen resizes.")]
	public bool checkForScreenResize = true;
	[Tooltip("Handy for testing the effect simply by clicking the mouse at runtime.")]
	public bool testEffectWithMouseClick = false;

	//Non serialized
	[System.NonSerialized] public bool playing = false;

    //Debugging
    [Header("Debug Options")]
    public bool debugConsole = false;

    //Credits and description
    [Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script allows you to scroll an array of instantiated images across the screen in any direction.";
    private string ScriptTags = "scrolling image wipe 2d visual effects vfx texture color colour";
    private string ScriptCategory = "effect";

    //Private
    private GameObject[] imageObjects;
	private float timer;

	//Converted values
	private float imgWidth;
	private float imgHeight;
	private Vector2 orgPoint;
	private Vector2 scrollDist;
	private Vector2 imgGap;

	private bool setupComplete = false;
    private bool active;

    void Start () {
		Setup ();
	}

	void Setup() {

		if(ErrorCheck()) {	//If there are no problems with the inspector variables..

			timer = effectDuration;
			imageObjects = new GameObject[imageCount];
			GameObject containerObject = new GameObject(transform.name + " ImageContainer");	//Instantiate timers, containers and arrays

			if(moveSelfToCanvas) {
				Destroy(containerObject);														//Destroy the container if it is not needed
			} else {
				containerObject.transform.SetParent(canvasParent); 								//Else, parent it to the canvas
			}

			//Create the desired number of images using a for loop
			for(int i = 0; i < imageCount; i++) {
				imageObjects[i] = new GameObject(transform.name + " Image_" + i);		//Instantiate the GO
				imageObjects[i].AddComponent<Image>();									//Add the image component

				if(moveSelfToCanvas) {													//If we are allowed to move the object this script is attached to to the canvas, parent all image objects created under this object and then move it
					imageObjects[i].transform.SetParent(transform);
					transform.SetParent(canvasParent);
				} else {
					imageObjects[i].transform.SetParent(containerObject.transform);		//Else, move all created images under the container object we created earlier
				}
				imageObjects[i].GetComponent<Image>().sprite = imageToApply;			//Apply the image sprite to all created images
			}

			ConvertVars();																//Convert percentage values to pixels if dimension mode is set to screen percentages
			SetImageSpecs();															//Set the image objects' initial positions and sizes
			setupComplete = true;														//Tell the update function that the setup has been completed successfull and there were no errors
		}
	}

	//Makes sure that inspector variables have been set up correctly and prints to the console if they are not
	bool ErrorCheck() {

		if(!canvasParent) {
			print ("ERROR: Please specify a canvas parent to group images under. For information on what a UI canvas is refer to the Unity go here: http://docs.unity3d.com/Manual/UICanvas.html");
			return false;

		} else if(!imageToApply) {
			print ("ERROR: Please attach an image to apply on object: " + gameObject.name);
			return false;

		} else if(imageCount <= 0) {
			print ("ERROR: Image Count is too low");
			return false;

		} else {
			return true;
		}
	}

	//Converts from screen percentage inspector values to pixel values for use in transforms
	void ConvertVars() {

		switch(dimensionsUnit) {
		case dims.percentage:

			imgWidth = imageWidth * Screen.width;
			imgHeight = imageHeight * Screen.height;
			orgPoint = new Vector2(originPoint.x * Screen.width, originPoint.y * Screen.height);
			scrollDist = new Vector2(scrollDistance.x * Screen.width, scrollDistance.y * Screen.height);
			imgGap = new Vector2(imageGap.x * Screen.width, imageGap.y * Screen.height);
			break;
		}
	}

	void Update () {

        if (active)
        {
            Master();

            if (Input.GetMouseButtonDown(0) && testEffectWithMouseClick)
            {
                Play();
            }
        }
	}

	void Master() {

		if (playing && setupComplete) {		//If the effect is playing..
			timer -= Time.deltaTime;		//Tick down timer
			SetImageSpecs();				//and set the position and size of each image
		}

		if(timer <= 0) {				//If timer hits zero, we are no longer playing effect and the timer resets
			playing = false;
			timer = effectDuration;
		}

		if(checkForScreenResize && dimensionsUnit == dims.percentage) {						//If the screen changed size and we are checking for resize..
				ConvertVars();																//Update the percentage to pixel values
		}
	}

	//Set the position and size of each image
	void SetImageSpecs() {

		for(int i = 0; i < imageObjects.Length; i++) {
			imageObjects[i].transform.position = GetImagePos(i);
			imageObjects[i].GetComponent<RectTransform>().sizeDelta = new Vector2(imgWidth, imgHeight);
		}
	}

	//Returns what should be the position of any image object according to its index
	Vector3 GetImagePos(int ID) {

		Vector2 distDelta = new Vector2((-1f * scrollDist.x) * (timer/effectDuration), (-1f * scrollDist.y) * (timer/effectDuration));
		Vector3 currentPos = new Vector3(orgPoint.x + (imgGap.x * ID) + distDelta.x, orgPoint.y + (imgGap.y * ID) + distDelta.y, 0);
		return currentPos;

	}

	//Starts the effect
	public void Play () {
		playing = true;
	}

    //Enables or disables the script's update function
    public void ToggleScript(bool state)
    {
        active = state;
        this.enabled = state;
        if (debugConsole)
        {
            print("Setting active state of " + this.GetType().Name + " script to: " + state + " at time: " + Time.time);
        }
    }
}
