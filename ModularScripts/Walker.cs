/* Script created by Lachlan McKay 2016 ©
* This script moves an object linearly in one direction */

using UnityEngine;
using System.Collections;

public class Walker : MonoBehaviour {

    [Header("Main")]
    [Tooltip("The target transform to move. (Leave blank for self)")]
    public Transform targetTransform;
    public bool active = true;
    [Tooltip("")]
    [Range(-9999,9999)]
    public float speed = 10f;
    public Vector3 direction = Vector3.forward;

    //Debugging
    [Header("Debug Options")]
    public bool debugConsole = false;
    public bool showVelocityRay = false;
    public float rayLength = 100f;
    public Color rayColour = Color.red;

    //Credits and description
    [Header("_© Lachlan McKay 2016_")]
    [TextArea(2, 2)]
    public string ScriptDescription = "This script moves an object linearly in one direction.";
    private string ScriptTags = "walker move motion movement speed engine simple linear direction path go travel";
    private string ScriptCategory = "motion";

    [System.NonSerialized] Vector3 velocity;

    void Start () {
	
        if(!ErrorChecking()) { active = false; }
	}

    bool ErrorChecking()
    {
        if(!targetTransform)
        {
            targetTransform = transform;
        }
        if(direction == Vector3.zero)
        {
            if(debugConsole) { print("Direction is null on object: " + gameObject.name + ". Please specify a direction in the inspector."); };
            return false;
        }
        return true;
    }

    void Update()
    {
        if (active) {
            Engine();
        }
	}

    void Engine()
    {
        Vector3 dir = transform.forward;
        velocity = dir * Time.deltaTime * speed;

        transform.position += velocity;

        if (showVelocityRay) {
            Debug.DrawRay(targetTransform.position, velocity * rayLength, rayColour);
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
