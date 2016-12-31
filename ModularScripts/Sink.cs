/* Script created by Lachlan McKay 2016 ©
 * This script sinks objects into the ground after a set time
 * Useful for sinking corpses or gibbed characters */

using UnityEngine;
using System.Collections;

public class Sink : MonoBehaviour {

    [Header("Time")]
    [Tooltip("How long to wait before sinking starts.")]
    public float lifeTime = 10f;
    [Tooltip("Length of time the objects will sink after lifetime has elapsed.")]
    public float sinkTime = 5f;
    [Tooltip("How fast the object(s) sink downwards.")]
    public float sinkSpeed = 1f;

    [Header("Component Removal")]
    [Tooltip("Destroy rigidbody components when sinking starts.")]
    public bool destroyRigidbodies = true;
    [Tooltip("Destroy collider components when sinking starts.")]
    public bool destroyColliders = true;
    [Tooltip("Destroy joint components when sinking starts.")]
    public bool destroyJoints = false;
    [Tooltip("Also destroy all children's components.")]
    public bool recursive = false;

    [Header("Targets")]
    [Tooltip("The transform to move downwards. (Leave blank for this object)")]
    public Transform moveThisTransform;
    [Tooltip("The object to destroy once sinking is complete. (Leave blank for this object)")]
    public GameObject destroyThisObj;

    //Credits and description
    [Header("_© Lachlan McKay 2016_")]
    [TextArea(2, 2)]
    public string ScriptDescription = "This script sinks objects into the ground after a set time.";
    private string ScriptTags = "sink die destroy hide fall through ground seep disappear corpse gib sinkhole";
    private string ScriptCategory = "spawning";

    //Private
    private bool active = true;
    private float timer = 0;
    private bool initialised = false;
    private bool componentsRemoved = false;

    void Setup()
    {
        if (!destroyThisObj) { destroyThisObj = gameObject; }
        if (!moveThisTransform) { moveThisTransform = transform; }

        initialised = true;
    }

    void Update()
    {
        if(active) { Master(); }
    }

    void Master()
    {
        if (!initialised) { Setup(); }

        timer += Time.deltaTime;

        if (timer > lifeTime + sinkTime)
        {
            Destroy(destroyThisObj);
        }
        else if (timer > lifeTime)
        {
            Sink();
        }
    }

    void RemoveComponents()
    {
        if (destroyJoints)
        {
            DestroyAllJoints(moveThisTransform.gameObject);
        }
        if (destroyRigidbodies)
        {
            DestroyAllRigidbodies(moveThisTransform.gameObject);
        }
        if (destroyColliders)
        {
            DestroyAllColliders(moveThisTransform.gameObject);
        }
        componentsRemoved = true;
    }

    //Sinks the gib into the ground
    void Sink()
    {
        if(!componentsRemoved) { RemoveComponents();  }
        Vector3 currentPos = moveThisTransform.position;
        currentPos.y -= sinkSpeed * Time.deltaTime;
        moveThisTransform.position = currentPos;

        sinkSpeed += 0.1f;
    }

    //Destroys all rigidbodies
    public void DestroyAllRigidbodies(GameObject obj)
    {
        if (!recursive)
        {
            foreach (Rigidbody rb in obj.GetComponents<Rigidbody>())
            {
                Destroy(rb);
            }
        } else {
            Component[] rigidbodies;
            rigidbodies = GetComponentsInChildren(typeof(Rigidbody));

            if (rigidbodies != null)
            {
                foreach (Rigidbody rb in rigidbodies)
                {
                    Destroy(rb);
                }
            }
        }
            
    }

    //Destroys any type of collider
    public void DestroyAllColliders(GameObject obj)
    {
        if(!recursive)
        {
            foreach (Collider c in obj.GetComponents<Collider>())
            {
                Destroy(c);
            }
        } else
        {
            Component[] colliders;
            colliders = GetComponentsInChildren(typeof(Collider));

            if (colliders != null)
            {
                foreach (Collider col in colliders)
                {
                    Destroy(col);
                }
            }
        }

    }

    //Destroys any type of joint
    public void DestroyAllJoints(GameObject obj)
    {
        if (!recursive)
        {
            foreach (Joint j in obj.GetComponents<Joint>())
            {
                Destroy(j);
            }
        } else
        {
            Component[] joints;
            joints = GetComponentsInChildren(typeof(Joint));

            if (joints != null)
            {
                foreach (Joint j in joints)
                {
                    Destroy(j);
                }
            }
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
