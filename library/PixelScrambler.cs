/* Script created by Lachlan McKay 2016 ©
 * This script allows you to physically modify the individual pixels of a texture randomly, similar to a TV static effect
 * Textures must be set to "Advanced", with Read/Write enabled and the format mode set to either: ARGB32, RGBA32 RGB24, Alpha8 or another float format
 * For best "pixelly" effects, use the "Point" filter mode on the texture */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PixelScrambler : MonoBehaviour
{

	public enum texType {Rendered, LegacyGUI};
	[Header ("Component")]
	[Tooltip("Is the object's material part of a Mesh Renderer or a GUITexture?")]
	public texType textureType = texType.Rendered;

	[Header ("Effect")]
	[Tooltip("Whether or not the effect will continue after the whole image has been filled with a target colour.")]
	public bool loopForever = false;
	[Range(1f, 1000f)]
	[Tooltip("How quickly a new round of pixels will be coloured. Time between executions in seconds = 1/effectSpeed.")]
	public float effectSpeed = 1.0f;
	[Range(1f, 5000f)]
	[Tooltip("How many pixels will be coloured per execution.")]
	public int instances;
	[Tooltip("Force every pixel to be coloured as the target colour at the end of the effect if not using Loop Forever.")]
	public bool useEndFill = false;

	[Header ("Colour")]
	[Tooltip("Target Colour will be a random greyscale value.")]
	public bool useRandomGreyScale;
	[Tooltip("Target Colour will be ANY random colour.")]
	public bool useRandomColours;
	[Tooltip("Target Colour will be any random colour that has a Red, Green, or Blue value of 100%.")]
	public bool useBrightRandomColours;
	[Tooltip("Each individual pixel will have a new colour value, including ones generated on the same frame.")]
	public bool individualColouredPixels = false;

	[Range(-1, 1)]
	[Tooltip("Increases or decreases the brightness of randomly generated colours.")]
	public float randomColBoost = 0;

	[Range(0.001f, 100f)]
	[Tooltip("How quickly a new target colour is generated randomly.")]
	public float randomColSpeed;
	[Tooltip("The colour that pixels will be generated as. If randomization is enabled, this will change.")]
	public Color targetCol;
	[Tooltip("If Use Random Grey Scale is enabled, use this to add a tint to generated pixels.")]
	public Color greyScaleTint;

    //Debugging
    [Header("Debug Options")]
    public bool debugConsole = false;

    //Credits and description
    [Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script allows you to scramble the pixels of a texture randomly, similar to a TV static effect. Make a backup of the target texture before use with this script. Textures must be set to 'Advanced', with Read/Write enabled and the format mode set to either: ARGB32, RGBA32 RGB24, Alpha8 or another float format. For best visual effect, use the 'Point' filter mode on the texture";
    private string scriptTags = "pixel scrambler 2d visual effects vfx texture color colour";

    //Private
    private Material mat;
	private Texture2D tex;
	
	private int randomIndex;
	
	private List<Vector2> availablePixels = new List<Vector2>();
	
	private int pixelsRemaining = -1;
	private int totalPixels;

	private bool finished = false;
	private bool dissolving = false;

	private float effectInterval;
	private float colourInterval;

	private float effectTimer = 0;
	private float colourTimer = 0;

    private bool active;


	void Start () {
	
        if(active)
        {
            Setup();
        }
	}

    void Setup()
    {
        if (textureType == texType.Rendered)
        {

            mat = gameObject.GetComponent<Renderer>().material;
            tex = mat.mainTexture as Texture2D;

        }
        else if (textureType == texType.LegacyGUI)
        {

            tex = gameObject.GetComponent<GUITexture>().texture as Texture2D;

        }

        pixelsRemaining = 0;

        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {

                Vector2 storedPos = new Vector2(i, j);

                availablePixels.Add(storedPos);
                pixelsRemaining++;
                totalPixels++;
            }
        }

        DissolveToCol(targetCol, loopForever, effectSpeed, useRandomColours, randomColSpeed, useBrightRandomColours);
    }

	void Update () {

		if(dissolving && active) {
			Dissolve();
		}

	}

	void DissolveToCol(Color inputTargetCol, bool inputLoop, float inputSpeed, bool useRandCol, float inputColSpeed, bool useBrightRandCol) {

		dissolving = true;
		targetCol = inputTargetCol;

		loopForever = inputLoop;

		useRandomColours = useRandCol;
		useBrightRandomColours = useBrightRandCol;

		effectInterval = 1.0f/inputSpeed;
		colourInterval = 1.0f/inputColSpeed;
		effectTimer = effectInterval;
	}

	void Dissolve() {

		HandleColour();
		HandleEffect();

	}

	void HandleColour() {

		if(colourTimer > 0) {

			colourTimer -= Time.deltaTime;

		} else {

			if(useRandomColours || useRandomGreyScale) {
				GenerateNewTargetCol();
			}
			colourTimer = colourInterval;

		}

	}

	void GenerateNewTargetCol() {

		float redRandomSplit = 0;
		float blueRandomSplit = 0;
		float greenRandomSplit = 0;

		if(useBrightRandomColours && !useRandomGreyScale) {

			redRandomSplit = Mathf.Sign(Random.Range(-1f,1f) - (greenRandomSplit/1.5f + blueRandomSplit/1.5f));
			blueRandomSplit = Mathf.Sign(Random.Range(-1f,1f) - (redRandomSplit/1.5f + greenRandomSplit/1.5f));
			greenRandomSplit = Mathf.Sign(Random.Range(-1f,1f) - (redRandomSplit/1.5f + blueRandomSplit/1.5f));

		} else if(!useRandomGreyScale) {

			redRandomSplit = Random.Range(0f,1f);
			blueRandomSplit = Random.Range(0f,1f);
			greenRandomSplit = Random.Range(0f,1f);

		} else if(useRandomGreyScale) {

			float randomAll = Random.Range(0f,1f);

			redRandomSplit = randomAll * greyScaleTint.r;
			blueRandomSplit = randomAll * greyScaleTint.b;
			greenRandomSplit = randomAll * greyScaleTint.g;

		}

		redRandomSplit = Mathf.Clamp(redRandomSplit + randomColBoost, -1f, 1f);
		blueRandomSplit = Mathf.Clamp(blueRandomSplit + randomColBoost, -1f, 1f);
		greenRandomSplit = Mathf.Clamp(greenRandomSplit + randomColBoost, -1f, 1f);
		
		targetCol = new Color(redRandomSplit, blueRandomSplit, greenRandomSplit);

	}

	
	void HandleEffect() {
		
		if(effectTimer > 0) {
			
			effectTimer -= Time.deltaTime;
			
		} else {
			
			if(pixelsRemaining > instances || loopForever) {
				for(int i = 0; i < instances; i++) {
					
					randomIndex = Random.Range(0, pixelsRemaining);
					
					//Execute the set colour
					int selectionCoordX = (int) availablePixels[randomIndex].x;
					int selectionCoordY	= (int) availablePixels[randomIndex].y;

					if(individualColouredPixels) {
						GenerateNewTargetCol();
					}

					tex.SetPixel(selectionCoordX, selectionCoordY, targetCol);

					if(individualColouredPixels) {
						tex.Apply();
					}

					if(!loopForever) {
						availablePixels.RemoveAt(randomIndex);
						pixelsRemaining--;
					}

				}

				if(!individualColouredPixels) {
					tex.Apply();
				}
				
			} else if(!finished && useEndFill){	//Fills with target colour on completion for smooth fill effect
					
				Color[] allTargetCol = new Color[totalPixels];
				
				for(int i = 0; i < totalPixels; i++) {
					allTargetCol[i] = targetCol;
				}
				
				tex.SetPixels(0,0,tex.width,tex.height, allTargetCol);
				tex.Apply();
				finished = true;
			}
			effectTimer = effectInterval;
		}
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
