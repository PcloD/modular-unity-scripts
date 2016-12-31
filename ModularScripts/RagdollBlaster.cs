/* Script created by Lachlan McKay 2016 ©
* This script recursively applies force to child rigidbodies */

using UnityEngine;
using System.Collections;

public class RagdollBlaster : MonoBehaviour {

    [Header("Universal")]
    [Tooltip("Apply forces straight away.")]
    public bool executeImmediately = true;
    [Tooltip("Choose a rigidbody force mode (Impulse is usually the best choice).")]
    public ForceMode forceMode = ForceMode.Impulse;
    [Tooltip("All vectors relative to world or object forward")]
    public bool relativeToObjForward = true;

    [Header("Linear Force")]
    [Tooltip("Apply a linear force to child rigidbodies")]
    public bool linearForce = false;
    public Vector3 forceVector = new Vector3(0, 0, 1200f);

    [Header("Relative Force")]
    [Tooltip("Apply a relative force to child rigidbodies")]
    public bool relForce = false;
    public Vector3 relForceVector = new Vector3(0, 0, 1200f);

    [Header("Explosion Force")]
    [Tooltip("Apply an explosion force to child rigidbodies")]
    public bool explode = false;
    public float explosionForce = 1200f;
    public float explosionRadius = 50f;
    public float upwardsModifier = 0;
    public Vector3 positionOffset;

    [Header("Force At Position")]
    [Tooltip("Apply a force at a particular position to child rigidbodies")]
    public bool forceAtPos = false;
    public Vector3 forceAtPosVector = new Vector3(0, 0, 1200f);
    public Vector3 positionVector;

    [Header("Torque")]
    [Tooltip("Apply torque (spin) to child rigidbodies")]
    public bool torque = false;
    public Vector3 torqueVector = new Vector3(0, 0, 1200f);

    [Header("Relative Torque")]
    [Tooltip("Apply relative torque (spin) to child rigidbodies")]
    public bool relTorque = false;
    public Vector3 relTorqueVector = new Vector3(0, 0, 1200f);

    //Credits and description
    [Header("_© Lachlan McKay 2016_")]
    [TextArea(2, 2)]
    public string ScriptDescription = "This script recursively applies force to child rigidbodies.";
    private string ScriptTags = "ragdoll blaster force rigid body rigidbody rb blast rag doll flop die death explode fly";
    private string ScriptCategory = "motion";

    void Start () {
        if(executeImmediately) { Execute(); }
	}
	
    //Executes script functionality
	public void Execute() {

        Component[] rigidbodies;
        rigidbodies = GetComponentsInChildren(typeof(Rigidbody));

        if (rigidbodies != null)
        {
            foreach (Rigidbody rb in rigidbodies)
            {
                EffectSwitch(rb);
            }
        }
    }

    //Applies relevant effects to an inputted rigidbody
    void EffectSwitch(Rigidbody rb)
    {

        if (explode)
        {
            rb.AddExplosionForce(explosionForce, transform.position + GetVector(positionOffset), explosionRadius, upwardsModifier);
        }

        if(linearForce)
        {
            rb.AddForce(GetVector(forceVector), forceMode);
            
        }

        if(relForce)
        {
            rb.AddRelativeForce(GetVector(relForceVector), forceMode);
        }

        if(forceAtPos)
        {
            rb.AddForceAtPosition(GetVector(forceAtPosVector), positionVector, forceMode);
        }

        if(torque)
        {
            rb.AddTorque(GetVector(torqueVector), forceMode);
        }

        if (relTorque)
        {
            rb.AddRelativeTorque(GetVector(relTorqueVector), forceMode);
        }
    }

    //Converts to local space if necessary
    Vector3 GetVector(Vector3 inputVec)
    {
        Vector3 returnVec = Vector3.zero;
        if (relativeToObjForward)
        {
            returnVec = transform.TransformDirection(inputVec);
        }
        else
        {
            returnVec = inputVec;
        }
        return returnVec;
    }
}
