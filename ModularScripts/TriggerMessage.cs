/* Script created by Lachlan McKay 2016 ©
* This script sends a message to a target object when a trigger is activated on this object */

using UnityEngine;
using System.Collections;

public class TriggerMessage : MonoBehaviour {

    [Header("Main")]
    public GameObject targetObject;
    public string message;
    public string parameters;

    [Header("Collision Type")]
    public bool trigger;
    public bool collision;

    [Header("Events")]
    public bool onEnter;
    public bool onStay;
    public bool onExit;

    [Header("Tag Comparison")]
    public bool requireTag;
    public string requiredTag;

    //Debugging
    [Header("Debug Options")]
    public bool debugConsole = false;

    //Credits and description
    [Header("_© Lachlan McKay 2016_")]
    [TextArea(2, 2)]
    public string ScriptDescription = "This script sends a message to a target object when a trigger is activated on this object.";
    private string ScriptTags = "trigger message send msg target box collider collision trig enter onenter exit stay event";
    private string ScriptCategory = "messaging";

    //Executes script functionality
    void Execute(string inTag)
    {
        if (inTag == requiredTag || !requireTag)
        {
            if (debugConsole) { print("InTag: " + inTag + " equals: " + requiredTag); }
            targetObject.SendMessage(message, parameters);
        }
    }

    //On event functions-----------------

    void OnTriggerEnter(Collider col)
    {
        if (onEnter)
        {
            if(debugConsole) { print("OnTriggerEnter triggered"); }
            Execute(col.tag);
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (onStay)
        {
            if (debugConsole) { print("OnTriggerStay triggered"); }
            Execute(col.tag);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (onExit)
        {
            if (debugConsole) { print("OnTriggerExit triggered"); }
            Execute(col.tag);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (onEnter)
        {
            if (debugConsole) { print("OnCollisionEnter triggered"); }
            Execute(col.collider.tag);
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (onStay)
        {
            if (debugConsole) { print("OnCollisionStay triggered"); }
            Execute(col.collider.tag);
        }
    }
    void OnCollisionExit(Collision col)
    {
        if (onExit)
        {
            if (debugConsole) { print("OnCollisionExit triggered"); }
            Execute(col.collider.tag);
        }
    }
}
