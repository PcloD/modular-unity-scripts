/* Script created by Lachlan McKay 2016 ©
 * This script outputs the currently selected option value of a dropdown component to a text UI component */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetDropDownValue : MonoBehaviour {

    [Header("Get")]
    [Tooltip("The dropdown component to get info from. (Leave blank for self)")]
    public Dropdown sourceDropdown;
    [Tooltip("Execute the script immediately?")]
    public bool executeOnAwake = true;
    [Tooltip("Add +1 to index for display purposes. (First entry will be 1 rather than 0)")]
    public bool addOneToIndex = true;

    [Header("Output")]
    [Tooltip("Output ID information to a text component?")]
    public bool outputToText;
    public enum form { INDEX, COUNT, INDEX_COUNT, TEXT, INDEX_TEXT, TEXT_INDEX_COUNT, INDEX_COUNT_TEXT};
    public form format;

    [Tooltip("The target text component to output to. (Leave blank for self)")]
    public Text textTarget;
    [Tooltip("Optional text to display before information.")]
    public string prefix;
    [Tooltip("Optional text to display between.")]
    public string separator;
    [Tooltip("Optional text to display after information.")]
    public string postfix;

    [Header("Debug Options")]
    public bool debugConsole;

    //Credits and description
    [Header("_© Lachlan McKay 2016_")]
    [TextArea(2, 2)]
    public string ScriptDescription = "This script outputs the currently selected option value of a dropdown component to a text UI component.";
    private string ScriptTags = "get drop down info dropdown option value to text ui display selected select";
    private string ScriptCategory = "screen";

    [System.NonSerialized] public int currentIndex;
    [System.NonSerialized] public int currentCount;
    [System.NonSerialized] public string currentText;
    private bool init = false;
    private bool active = false;

    void Start()
    {
        active = executeOnAwake;
    }

    //Checks for inspector errors
    bool ErrorChecking()
    {
        if(!sourceDropdown)
        {
            if(GetComponent<Dropdown>())
            {
                sourceDropdown = GetComponent<Dropdown>();
            } else
            {
                if(debugConsole) { print("ERROR: No Dropdown component specified on object: " + gameObject.name + ". Please check the inspector."); }
                return false;
            }
        }

        if(outputToText && !textTarget)
        {
            if (GetComponent<Text>())
            {
                textTarget = GetComponent<Text>();
            }
            else
            {
                if (debugConsole) { print("ERROR: No Text component specified on object: " + gameObject.name + ". Please check the inspector."); }
                return false;
            }
        }
        return true;
    }
	
    //Script runs in update in order to wait for other start functions
	void Update () {
	    if(!init && ErrorChecking() && active)
        {
            Execute();
            init = true;
        }
	}

    //Executes the script's functionality
    public void Execute()
    {
        StoreInfo();
        if(outputToText)
        {
            textTarget.text = GetFormattedText();
        }
    }

    //Stores dropdown info in exposed variables
    void StoreInfo()
    {
        switch (format)
        {
            default:
            case form.INDEX:
                currentIndex = GetIndex();
                break;
            case form.COUNT:
                currentCount = GetCount();
                break;
            case form.INDEX_COUNT:
                currentIndex = GetIndex();
                currentCount = GetCount();
                break;
            case form.INDEX_COUNT_TEXT:
            case form.TEXT_INDEX_COUNT:
                currentIndex = GetIndex();
                currentCount = GetCount();
                currentText = GetText();
                break;
            case form.INDEX_TEXT:
                currentIndex = GetIndex();
                currentText = GetText();
                break;
            case form.TEXT:
                currentText = GetText();
                break;
        }
    }

    //Gets a string formatted according to inspector values
    string GetFormattedText()
    {
        switch(format)
        {
            case form.INDEX:
            default:
                return textTarget.text = prefix + currentIndex + postfix;
            case form.COUNT:
                return textTarget.text = prefix + currentCount + postfix;
            case form.INDEX_COUNT:
                return textTarget.text = prefix + currentIndex + separator + currentCount + postfix;
            case form.INDEX_COUNT_TEXT:
                return textTarget.text = prefix + currentIndex + separator + currentCount + currentText + postfix;
            case form.INDEX_TEXT:
                return textTarget.text = prefix + currentIndex + separator + currentText + postfix;
            case form.TEXT:
                return textTarget.text = prefix + currentText + postfix;
            case form.TEXT_INDEX_COUNT:
                return textTarget.text = prefix + currentText + separator + currentIndex + separator + currentCount + postfix;
        }
    }

    //Returns the index value of the sourceDropdown
    int GetIndex()
    {
        int returnVal = sourceDropdown.value;
        if(addOneToIndex) { returnVal++; }
        return returnVal;
    }

    //Returns number of dropdown options
    int GetCount()
    {
        return sourceDropdown.options.Count;
    }

    //Returns the text of the current dropdown option
    string GetText()
    {
        if (sourceDropdown.options.Count > 0)
        {
            return sourceDropdown.options[sourceDropdown.value].text;
        } else
        {
            return "";
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
