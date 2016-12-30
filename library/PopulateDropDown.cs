/* Script created by Lachlan McKay 2016 ©
 * This script populates a target dropdown component with specified text/image values */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PopulateDropDown : MonoBehaviour {

    [Header("Main")]
    public Dropdown targetDropdown;
    public bool executeOnAwake = true;
    public bool clearExistingOptions = true;
    public enum mode { text, image };

    [Header("Options Arrays")]
    public mode OptionType = mode.text;
    public string[] textOptions;
    public Sprite[] imageOptions;

    [Header("Debug Options")]
    public bool debugConsole;

    //Credits and description
    [Header("_© Lachlan McKay 2016_")]
    [TextArea(2, 2)]
    public string ScriptDescription = "This script populates a target dropdown component with specified text/image values.";
    private string ScriptTags = "populate dropdown ui drop down control option options value change modify text image";
    private string ScriptCategory = "screen";

    void Start()
    {
        if (ErrorChecking()) {
            if (executeOnAwake)
            { Execute(); }
        }
    }

    bool ErrorChecking()
    {
        if (!targetDropdown)
        {
            if(GetComponent<Dropdown>())
            {
                targetDropdown = GetComponent<Dropdown>();
            } else
            {
                if(debugConsole)
                {
                    print("ERROR: No TargetDropdown specified on object: " + gameObject.name + ". Please check the inspector.");
                }
                return false;
            }
        }

        if (executeOnAwake)
        {
            if (OptionType == mode.text && textOptions.Length < 1)
            {
                if (debugConsole)
                {
                    print("ERROR: No text options specified in TextOptions array on object: " + gameObject.name + ". Please check the inspector.");
                }
                return false;
            }
            else if (OptionType == mode.image && imageOptions.Length < 1)
            {
                if (debugConsole)
                {
                    print("ERROR: No image options specified in ImageOptions array on object: " + gameObject.name + ". Please check the inspector.");
                }
                return false;
            }
        }
        return true;
    }

    //Executes all of this script's functionality
    void Execute()
    {
        if (clearExistingOptions)
        {
            targetDropdown.options.Clear();
        }
        Populate();
    }

    //Populate the target dropdown element based on inspector selections
    void Populate()
    {
        switch (OptionType)
        {
            case mode.text:
                PopulateWithText(textOptions);
                break;
            case mode.image:
                PopulateWithImages(imageOptions);
                break;
        }
    }

    //Populates the target dropdown element with a given string array
    public void PopulateWithText(string[] inputStringArr)
    {
        ErrorChecking();
        for (int i = 0; i < inputStringArr.Length; i++)
        {
            targetDropdown.options.Add(new Dropdown.OptionData(inputStringArr[i]));
        }
        Refresh();
    }

    //Populates the target dropdown element with a given image array
    public void PopulateWithImages(Sprite[] inputSpriteArr)
    {
        ErrorChecking();
        for (int i = 0; i < inputSpriteArr.Length; i++)
        {
            targetDropdown.options.Add(new Dropdown.OptionData(inputSpriteArr[i]));
        }
        Refresh();
    }

    //Refreshes the dropdown display
    public void Refresh()
    {
        targetDropdown.RefreshShownValue();
    }

    public void ClearOptions()
    {
        targetDropdown.options.Clear();
    }
}
