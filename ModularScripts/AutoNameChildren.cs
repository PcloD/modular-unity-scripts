/* Script created by Lachlan McKay 2016 �
 * This script numerically names all children objects */

using UnityEngine;
using System.Collections;

public class AutoNameChildren : MonoBehaviour {

    [Header("Main Options")]
    [Tooltip("Do not include this object.")]
    public bool excludeSelf = true;
    [Tooltip("Disable children after they have been named.")]
    public bool disableAfter = false;

    [Header("Naming")]
    [Tooltip("String to add before numeric index. (optional)")]
    public string prefix;
    [Tooltip("String to add after numeric index. (optional)")]
    public string suffix;

    //Credits and description
    [Header("_© Lachlan McKay 2016_")]
    [TextArea(2, 2)]
    public string ScriptDescription = "This script numerically names all children objects.";
    private string ScriptTags = "auto name rename children hierarchy array number id child index sort order";
    private string ScriptCategory = "management";

    [System.NonSerialized] public Transform[] children;

    void Awake()
    {
        RenameChildren();
    }

    //Initialises arrays
    void Init()
    {
        Transform[] loadChildren = transform.GetComponentsInChildren<Transform>();
        
        int offset = excludeSelf ? 1 : 0;
        children = new Transform[loadChildren.Length - offset];

        for (int i = 0; i < loadChildren.Length - offset; i++)
        {
            children[i] = loadChildren[i + offset];
        }
    }

    //Renames children
    void Execute()
    {
        for (int i = 0; i < children.Length; i++)
        {
            if(!excludeSelf || children[i].name != gameObject.name )
            {
                string totalString = prefix + i + suffix;
                children[i].name = totalString;

                if(disableAfter) { children[i].gameObject.SetActive(false); }
            }
        }
    }

    //Renames all children
    public void RenameChildren()
    {
        Init();
        Execute();
    }
}
