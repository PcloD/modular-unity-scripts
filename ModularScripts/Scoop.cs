/* Script created by Lachlan McKay 2016 ©
 * This script attaches two rigidbodies via fixed joint on collision or trigger
 * Useful for charging enemies that pin the player etc. */

using UnityEngine;
using System.Collections;

public class Scoop : MonoBehaviour {

    [Header("Objects")]
    public Rigidbody rb;
    private FixedJoint scoopJoint;

    [Header("CollisionType")]
    [Tooltip("Automatically trigger scoop on trigger/collision if conditions are met.")]
    public bool scoopOnEnter;
    [Tooltip("Scoop on collision entry.")]
    public bool collision;
    [Tooltip("Scoop on trigger entry.")]
    public bool trigger;

    [Header("Conditions")]
    [Tooltip("If set, the collider's name must equal this string to trigger the scoop.")]
    public string matchName;
    [Tooltip("If set, the collider's tag must equal this string to trigger the scoop.")]
    public string matchTag;
    [Tooltip("If set, the collider's layer name must equal this string to trigger the scoop.")]
    public string matchLayer;

    [Header("JointConfig")]
    [Tooltip("If enabled, fixed joint is placed on this object and other object is attached. Vice versa if disabled.")]
    public bool selfOrCol = true;
    [Tooltip("If enabled, do not apply a new joint if one already exists.")]
    public bool checkExisting = true;

    //Credits and description
    [Header("_© Lachlan McKay 2016_")]
    [TextArea(2, 2)]
    public string ScriptDescription = "This script attaches two rigidbodies via fixed joint on collision or trigger.";
    private string ScriptTags = "scoop fixed joint attach weld join pin charge magnet stick grab hold clasp";
    private string ScriptCategory = "motion";

    //Private
    private bool active = true;

    void OnCollisionEnter(Collision col)
    {
        if (collision) { Execute(col.collider); }
    }

    void OnTriggerEnter (Collider col) {
        if (trigger) { Execute(col); }
    }

    void Execute(Collider col)
    {
        if (MatchCheck(col.gameObject.name, matchName) && MatchCheck(col.tag, matchTag) && MatchCheck(LayerMask.LayerToName(col.gameObject.layer), matchLayer))   //Check all relevant options match
        {
            ScoopRigidbody(true, col.gameObject.GetComponent<Rigidbody>());   //Create the fixed joint
        }
    }

    //Checks if the collider name matches
    bool MatchCheck(string testee, string mustEqual)
    {
        if(mustEqual != "" && mustEqual != null)
        {
            if(mustEqual == testee) { return true; } else { return false; }
        } else
        {
            return true;
        }
    }

    //Attaches or releases the player to/from the bull
    public void ScoopRigidbody(bool onOff, Rigidbody inRb)
    {
        if (onOff)
        {
            if (!scoopJoint)
            {
                if(selfOrCol)
                {
                    if ((checkExisting && !gameObject.GetComponent<FixedJoint>()) || !checkExisting)    //check if a fixed joint already exists
                    {
                        scoopJoint = gameObject.AddComponent<FixedJoint>(); //Create a fixed joint and attach the player
                        scoopJoint.connectedBody = inRb;    //Connect the inputted rigidbody to this object's rb
                    }
                } else
                {
                    if ((checkExisting && !inRb.gameObject.GetComponent<FixedJoint>()) || !checkExisting)    //check if a fixed joint already exists
                    {
                        scoopJoint = inRb.gameObject.AddComponent<FixedJoint>(); //Create a fixed joint and attach the player
                        scoopJoint.connectedBody = rb;    //Connect the inputted rigidbody to this object's rb
                    }
                }
            }
        }
        else
        {
            if (scoopJoint) { Destroy(scoopJoint); }   //Destroy the scoop joint
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
