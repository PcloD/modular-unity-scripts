/* Script created by Lachlan McKay 2016 ©
 * This script generates an array of vectors that can be arranged in many formations including: Wave/Spiral/Circle/Cone/Sphere etc */

using UnityEngine;
using System.Collections;

public class WaveArray : MonoBehaviour
{

	[Header("Main Options")]
	[Tooltip("The transform around which the wave will center.")]
	public Transform centerTransform;
	[Tooltip("Enable this if you want the wave to update it's position to the center transform's position at runtime.")]
	public bool followContinuously = false;
	[Tooltip("Enable this if you want this to only be activated by another script.")]
	public bool commandActivated = false;
	
	[Header("Controls")]
	[Tooltip("How far the vector will oscillate.")]
	public Vector3 amplitude = Vector3.one;
	[Tooltip("How quickly the vector will oscillate.")]
	public float speed = 0.1f;
	[Tooltip("How quickly the amplitude values will oscillate.")]
	public float amplitudeOcillationSpeed = 0.05f;
	[Tooltip("How many vectors are in the array.")]
	public int totalPoints = 100;
	[Tooltip("The distance between each vector down the chain.")]
	public Vector3 distanceBetweenPoints = Vector3.one;

	[Header("Oscillation Method")]
	[Tooltip("What type movement will the wave have on the X axis?")]
	public oscTypeX oscillationMethodX;
	public enum oscTypeX {None, ArrangeByID, Sine, Cos, PerlinX, PerlinY, PerlinDouble}
	[Tooltip("What type movement will the wave have on the Y axis?")]
	public oscTypeY oscillationMethodY;
	public enum oscTypeY {None, ArrangeByID, Sine, Cos, PerlinX, PerlinY, PerlinDouble}
	[Tooltip("What type movement will the wave have on the Z axis?")]
	public oscTypeZ oscillationMethodZ;
	public enum oscTypeZ {None, ArrangeByID, Sine, Cos, PerlinX, PerlinY, PerlinDouble}

	[Header("No Oscillation Options")]
	[Tooltip("When oscillation method is set to None, uses this value.")]
	public Vector3 defaultConstantValues = Vector3.zero;

	[Header("Oscillate Amplitude")]
	[Tooltip("What type movement will the wave have on the amplitudeX axis?")]
	public ampOscX amplitudeOscillationX;
	public enum ampOscX {None, Sine, Cos, PerlinX, PerlinY, PerlinDouble}
	[Tooltip("What type movement will the wave have on the amplitudeY axis?")]
	public ampOscY amplitudeOscillationY;
	public enum ampOscY {None, Sine, Cos, PerlinX, PerlinY, PerlinDouble}
	[Tooltip("What type movement will the wave have on the amplitudeZ axis?")]
	public ampOscZ amplitudeOscillationZ;
	public enum ampOscZ {None, Sine, Cos, PerlinX, PerlinY, PerlinDouble}
	[Space(7)]
	[Tooltip("Should the X oscillation of amplitude be affected by index?")]
	public bool useIndexDistributionX = false;
	[Tooltip("Should the Y oscillation of amplitude be affected by index?")]
	public bool useIndexDistributionY = false;
	[Tooltip("Should the Z oscillation of amplitude be affected by index?")]
	public bool useIndexDistributionZ = false;
	

	[Header("Perlin Options")]
	[Tooltip("When oscillation method is set to PerlinX, uses Y value and vice versa.")]
	public Vector2 perlinValuesX = Vector2.zero;
	[Tooltip("When oscillation method is set to PerlinX, uses Y value and vice versa.")]
	public Vector2 perlinValuesY = Vector2.zero;
	[Tooltip("When oscillation method is set to PerlinX, uses Y value and vice versa.")]
	public Vector2 perlinValuesZ = Vector2.zero;

	[Header("Spiral Settings")]
	[Tooltip("Arranges amplitude by ID, causing a spiral effect.")]
	public bool spiralMode = false;
	[Tooltip("The radius of the spiral effect.")]
	public float radius = 1f;

	//Debugging
	[Header("Debug Options")]
	[Tooltip("Display errors in the console.")]
	public bool debugConsole = false;
	[Tooltip("Visually display vectors as cubes that take on the vectors positionally.")]
	public bool displayPoints = false;
	[Tooltip("Scale of the cubes representing points.")]
	public float pointScale = 1f;

	//Credits and description
	[Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "This script generates an array of vectors that can be arranged in many formations including: Wave/Spiral/Circle/Cone/Sphere etc. Access the vector array via other scripts by calling the function: GetVector(int ID)";
    private string ScriptTags = "wave array point generator oscillation oscillate oscillator motion movement effect vfx noise";
    private string ScriptCategory = "effect";

    //Output
    [System.NonSerialized] public Vector3[] vectors;

	//Private
	private bool active;					//Enables or disables script functionality

	private Vector3 centerPosition;			//The center of the vector array
	private Vector3 pvtAmplitude;			//Privately stored amplitude vector

	private float phase;					//The current time multiplied by speed
	private float theta;					//The current phase with amplitude accounted for

	private float overridePhase;			//The overriden phase input by another script
	private bool receivingPhase = false;	//Whether or not the phase is being overriden
	private Vector3 overrideCenter;			//The overriden center input by another script
	private bool receivingCenter = false;	//Whether or not the center is being overriden

	private float xCoord;					//The X coordinate to assign to the next vector within the for loop
	private float yCoord;					//The Y coordinate to assign to the next vector within the for loop
	private float zCoord;					//The Z coordinate to assign to the next vector within the for loop

	//Debugging
	private GameObject[] cubes;				//Array of cube primitives to represent vector points

	void Start () {
		Setup();
	}

	void Setup() {

		if(!commandActivated) {
			active = true;
		}

		if(!centerTransform) {
			centerTransform = transform;
		}

		vectors = new Vector3[totalPoints];

		if(displayPoints) {

			cubes = new GameObject[totalPoints];
			GameObject cubesParent = new GameObject(gameObject.name + " - Debugging cubes container");
			
			for(int i = 0; i < cubes.Length; i++) {
				cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cubes[i].transform.SetParent(cubesParent.transform);
				cubes[i].name = i.ToString();
				cubes[i].transform.localScale = new Vector3(pointScale, pointScale, pointScale);
			}
		}

		if(!receivingCenter) {
			centerPosition = centerTransform.position;
		} else {
			centerPosition = overrideCenter;
		}
		//totalLength = distanceBetweenPoints * totalPoints;
	}

	void Update () {

		if(active) {
			GetInfo();
			SetVectors();
			DebuggingInfo();
		}
	}

	//Update relevant private variables to inspector ones
	void GetInfo() {

		if(followContinuously && !receivingCenter) {

			centerPosition = centerTransform.position;

		} else if(followContinuously && receivingCenter) {

			centerPosition = overrideCenter;
		}
	}

	//Sets XYZ coords for insertion into vectors[] array
	void SetVectors() {

		for(int i = 0; i < vectors.Length; i++) {

			//Grab amplitude oscillation values
			pvtAmplitude.x = GetCoords("AX", i);
			pvtAmplitude.y = GetCoords("AY", i);
			pvtAmplitude.z = GetCoords("AZ", i);

			if(spiralMode) {
				pvtAmplitude.x = radius + (i * distanceBetweenPoints.x);
				pvtAmplitude.y = radius + (i * distanceBetweenPoints.y);
				pvtAmplitude.z = radius + (i * distanceBetweenPoints.z);
			}

			//Grab vector oscillation values
			xCoord = GetCoords("X", i);
			yCoord = GetCoords("Y", i);
			zCoord = GetCoords("Z", i);
			
			vectors[i] = new Vector3(centerPosition.x + xCoord, centerPosition.y + yCoord, centerPosition.z + zCoord);
			
			phase += speed * Time.deltaTime;
			theta = GetTheta(i);
		}
	}

	//Shows cubes at vector locations
	void DebuggingInfo() {

		if(displayPoints) {
			for(int i = 0; i < vectors.Length; i++) {
				cubes[i].transform.position = vectors[i];
			}
		}
	}

	//Grabs coordinates
	float GetCoords(string axis, int ID) {

		switch(axis) {

		case "X":

			switch(oscillationMethodX) {

			case oscTypeX.None:
				return defaultConstantValues.x;

			case oscTypeX.ArrangeByID:
				return ID * distanceBetweenPoints.x;
				
			case oscTypeX.Sine:
				return pvtAmplitude.x * Mathf.Sin(theta);
				
			case oscTypeX.Cos:
				return pvtAmplitude.x * Mathf.Cos(theta);
				
			case oscTypeX.PerlinX:
				return pvtAmplitude.x * Mathf.PerlinNoise(theta, perlinValuesX.y);
				
			case oscTypeX.PerlinY:
				return pvtAmplitude.x * Mathf.PerlinNoise(perlinValuesX.x, theta);
				
			case oscTypeX.PerlinDouble:
				return pvtAmplitude.x * Mathf.PerlinNoise(theta, theta);
				
			}
			break;

		case "Y":

			switch(oscillationMethodY) {

			case oscTypeY.None:
				return defaultConstantValues.y;
				
			case oscTypeY.ArrangeByID:
				return ID * distanceBetweenPoints.y;
				
			case oscTypeY.Sine:
				return pvtAmplitude.y * Mathf.Sin(theta);
				
			case oscTypeY.Cos:
				return pvtAmplitude.y * Mathf.Cos(theta);
				
			case oscTypeY.PerlinX:
				return pvtAmplitude.y * Mathf.PerlinNoise(theta, perlinValuesY.y);

			case oscTypeY.PerlinY:
				return pvtAmplitude.y * Mathf.PerlinNoise(perlinValuesY.x, theta);
				
			case oscTypeY.PerlinDouble:
				return pvtAmplitude.y * Mathf.PerlinNoise(theta, theta);
				
			}
			break;

		case "Z":
			switch(oscillationMethodZ) {

			case oscTypeZ.None:
				return defaultConstantValues.z;
				
			case oscTypeZ.ArrangeByID:
				return ID * distanceBetweenPoints.z;
				
			case oscTypeZ.Sine:
				return pvtAmplitude.z * Mathf.Sin(theta);
				
			case oscTypeZ.Cos:
				return pvtAmplitude.z * Mathf.Cos(theta);
				
			case oscTypeZ.PerlinX:
				return pvtAmplitude.z * Mathf.PerlinNoise(theta, perlinValuesZ.y);
				
			case oscTypeZ.PerlinY:
				return pvtAmplitude.z * Mathf.PerlinNoise(perlinValuesZ.x, theta);
				
			case oscTypeZ.PerlinDouble:
				return pvtAmplitude.z * Mathf.PerlinNoise(theta, theta);
				
			}
			break;

			//Amplitude Oscillation-------------------------------------
		case "AX":

			switch(amplitudeOscillationX) {

			case ampOscX.None:
				return amplitude.x;
				
			case ampOscX.Sine:
				if(useIndexDistributionX) {
					return amplitude.x * Mathf.Sin(theta * amplitudeOcillationSpeed);
				} else {
					return amplitude.x * Mathf.Sin(phase * amplitudeOcillationSpeed);
				}
				
			case ampOscX.Cos:
				if(useIndexDistributionX) {
					return amplitude.x * Mathf.Cos(theta * amplitudeOcillationSpeed);
				} else {
					return amplitude.x * Mathf.Cos(phase * amplitudeOcillationSpeed);
				}
				
			case ampOscX.PerlinX:
				if(useIndexDistributionX) {
					return amplitude.x * Mathf.PerlinNoise(theta * amplitudeOcillationSpeed, perlinValuesX.y * amplitudeOcillationSpeed);
				} else {
					return amplitude.x * Mathf.PerlinNoise(phase * amplitudeOcillationSpeed, perlinValuesX.y * amplitudeOcillationSpeed);
				}

				
			case ampOscX.PerlinY:
				if(useIndexDistributionX) {
					return amplitude.x * Mathf.PerlinNoise(perlinValuesX.x * amplitudeOcillationSpeed, theta * amplitudeOcillationSpeed);
				} else {
					return amplitude.x * Mathf.PerlinNoise(perlinValuesX.x * amplitudeOcillationSpeed, phase * amplitudeOcillationSpeed);
				}

				
			case ampOscX.PerlinDouble:
				if(useIndexDistributionX) {
					return amplitude.x * Mathf.PerlinNoise(theta * amplitudeOcillationSpeed, theta * amplitudeOcillationSpeed);
				} else {
					return amplitude.x * Mathf.PerlinNoise(phase * amplitudeOcillationSpeed, phase * amplitudeOcillationSpeed);
				}

				
			}
			break;

		case "AY":
			
			switch(amplitudeOscillationY) {
				
			case ampOscY.None:
				return amplitude.y;
				
			case ampOscY.Sine:
				if(useIndexDistributionY) {
					return amplitude.y * Mathf.Sin(theta * amplitudeOcillationSpeed);
				} else {
					return amplitude.y * Mathf.Sin(phase * amplitudeOcillationSpeed);
				}
				
			case ampOscY.Cos:
				if(useIndexDistributionY) {
					return amplitude.y * Mathf.Cos(theta * amplitudeOcillationSpeed);
				} else {
					return amplitude.y * Mathf.Cos(phase * amplitudeOcillationSpeed);
				}
				
			case ampOscY.PerlinX:
				if(useIndexDistributionY) {
					return amplitude.y * Mathf.PerlinNoise(theta * amplitudeOcillationSpeed, perlinValuesY.y * amplitudeOcillationSpeed);
				} else {
					return amplitude.y * Mathf.PerlinNoise(phase * amplitudeOcillationSpeed, perlinValuesY.y * amplitudeOcillationSpeed);	
				}
				
			case ampOscY.PerlinY:
				if(useIndexDistributionY) {
					return amplitude.y * Mathf.PerlinNoise(perlinValuesY.x * amplitudeOcillationSpeed, theta * amplitudeOcillationSpeed);
				} else {
					return amplitude.y * Mathf.PerlinNoise(perlinValuesY.x * amplitudeOcillationSpeed, phase * amplitudeOcillationSpeed);
				}
				
			case ampOscY.PerlinDouble:
				if(useIndexDistributionY) {
					return amplitude.y * Mathf.PerlinNoise(theta * amplitudeOcillationSpeed, theta * amplitudeOcillationSpeed);
				} else {
					return amplitude.y * Mathf.PerlinNoise(phase * amplitudeOcillationSpeed, phase * amplitudeOcillationSpeed);
				}
				
			}
			break;

		case "AZ":
			
			switch(amplitudeOscillationZ) {
				
			case ampOscZ.None:
				return amplitude.z;
				
			case ampOscZ.Sine:
				if(useIndexDistributionZ) {
					return amplitude.z * Mathf.Sin(theta * amplitudeOcillationSpeed);
				} else {
					return amplitude.z * Mathf.Sin(phase * amplitudeOcillationSpeed);
				}
				
			case ampOscZ.Cos:
				if(useIndexDistributionZ) {
					return amplitude.z * Mathf.Cos(theta * amplitudeOcillationSpeed);
				} else {
					return amplitude.z * Mathf.Cos(phase * amplitudeOcillationSpeed);
				}
				
			case ampOscZ.PerlinX:
				if(useIndexDistributionZ) {
					return amplitude.z * Mathf.PerlinNoise(theta * amplitudeOcillationSpeed, perlinValuesZ.y * amplitudeOcillationSpeed);
				} else {
					return amplitude.z * Mathf.PerlinNoise(phase * amplitudeOcillationSpeed, perlinValuesZ.y * amplitudeOcillationSpeed);
				}
				
			case ampOscZ.PerlinY:
				if(useIndexDistributionZ) {
					return amplitude.z * Mathf.PerlinNoise(perlinValuesZ.x * amplitudeOcillationSpeed, theta * amplitudeOcillationSpeed);
				} else {
					return amplitude.z * Mathf.PerlinNoise(perlinValuesZ.x * amplitudeOcillationSpeed, phase * amplitudeOcillationSpeed);
				}
				
			case ampOscZ.PerlinDouble:
				if(useIndexDistributionZ) {
					return amplitude.z * Mathf.PerlinNoise(theta * amplitudeOcillationSpeed, theta * amplitudeOcillationSpeed);
				} else {
					return amplitude.z * Mathf.PerlinNoise(phase * amplitudeOcillationSpeed, phase * amplitudeOcillationSpeed);
				}
				
			}
			break;

		}

		if(debugConsole) {
			print ("ERROR: Could not get coordinates for axis: " + axis + " in VectorArrayOscillator script on object: " + gameObject.name);
		}
		return 0;
	}

	//Gets the theta value based on whether the phase is being overriden
	public float GetTheta(int ID) {

		if(receivingPhase) {

			return ID + overridePhase;

		} else {

			return ID + phase;
		}

	}

	//Gets the phase value based on whether the phase is being overriden
	public float GetPhase() {

		if(receivingPhase) {
			
			return overridePhase;
			
		} else {
			
			return phase;
		}
	}

	//Allows other scripts to pass in a custom phase
	public void ReceivePhase(float inputPhase) {

		overridePhase = inputPhase;
	}

	//Enables / disables the passing in of other phases
	public void ToggleOverridePhase(bool state) {

		receivingPhase = state;
	}

	//Allows other scripts to pass in a custom center
	public void ReceiveCenter(Vector3 inputCenter) {
		
		overrideCenter = inputCenter;
	}

	//Enables / disables the passing in of other center vectors
	public void ToggleOverridCenter(bool state) {

		receivingCenter = state;
	}
	
	//Public output function
	public Vector3 GetVector(int ID) {
		
		return vectors[ID];
	}

	//Public output function
	public int GetLength() {
		
		return totalPoints;
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
