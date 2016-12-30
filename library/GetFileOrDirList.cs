/* Script created by Lachlan McKay 2016 ©
 * This script sends a list of file or directory names under a given path to a target object as a message */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class GetFileOrDirList : MonoBehaviour {

    [Header("Main")]
    public bool executeOnAwake = true;

    [Header("Get")]
    [Tooltip("The directory to search (underneath the Assets folder). E.g. '/Scenes' would search (PathtoProjectFolder)/Assets/Scenes")]
    public string path;
    public enum mode { file, directory };
    [Tooltip("Get either files or directories.")]
    public mode getMode = mode.file;

    [Header("Send")]
    [Tooltip("Send the resulting array of names to another script?")]
    public bool sendResult = false;
    [Tooltip("Specify an object to send name array to. (Leave blank for this object)")]
    public GameObject targetObject;
    [Tooltip("The name of the script function that will receive the name array.")]
    public string[] receiveFunctionNames;

    [Header("Matching")]
    public bool matchResults = false;
    public enum match { excludeGiven, includeGiven };
    [Tooltip("Either exclude or ONLY include any found directory/file names that contain any of these given strings.")]
    public match matchMode = match.excludeGiven;
    [Tooltip("A list of strings to test found directory/file names for matches.")]
    public string[] givenList;

    [Header("Debug Options")]
    [Tooltip("Display error information in the console.")]
    public bool debugConsole;
    [Tooltip("Print name array in the console.")]
    public bool printGetResults;
    [Tooltip("Print matching information in the console.")]
    public bool printMatchResults;

    //Credits and description
    [Header("_© Lachlan McKay 2016_")]
    [TextArea(2, 2)]
    public string ScriptDescription = "This script sends a list of file or directory names under a given path to a target object as a message.";
    private string ScriptTags = "get files or directories file name list array folder structure heirarchy resources names resource assets asset";
    private string ScriptCategory = "management";

    [System.NonSerialized] public string[] names;
    private List<string> nameList;

    void Start () {
        if (executeOnAwake) { Execute(); }
    }

    //Checks for user inspector errors before executing
    bool ErrorChecking()
    {
        if(matchResults)
        {
            if(givenList.Length < 1)
            {
                if(debugConsole)
                {
                    print("ERROR: Please include a string to match against when using include/exclude on object: " + gameObject.name);
                }
                return false;
            }
        }

        if(sendResult)
        {
            if(!targetObject)
            {
                targetObject = gameObject;
            }
            if(receiveFunctionNames.Length < 1)
            {
                if(debugConsole)
                {
                    print("ERROR: No receive function names specified on object: " + gameObject.name + ". Please specify a script function name to output results to in the inspector.");
                    return false;
                }
            }
        }
        return true;
    }

    //Executes script functionality
    public void Execute()
    {
        if (ErrorChecking())
        {
            GetNames(path);
            if (sendResult) {
                for(int i = 0; i < receiveFunctionNames.Length; i++)
                {
                    SendNames(targetObject, receiveFunctionNames[i]);
                }
            }
        }
    }

    //Gets the list of files or directories
    void GetNames(string inputPath)
    {
        string completePath = Application.dataPath + inputPath;

        switch(getMode)
        {
            case mode.file:
            default:
                nameList = new List<string>(Directory.GetFiles(completePath));    //Returns files
                break;
            case mode.directory:
                nameList = new List<string>(Directory.GetDirectories(completePath));            //Returns directories
                break;
        }

        if (matchResults) { nameList.RemoveAll(MatchString); }

        names = new string[nameList.Count];

        for (int i = 0; i < names.Length; i++)
        {
            names[i] = nameList[i].Substring(nameList[i].LastIndexOf("\\") + 1);  //Store cleaned string names in array
            if (printGetResults) { print(names[i]); }
        }
    }

    void SendNames(GameObject targetObj, string functionName)
    {
        targetObj.SendMessage(functionName, names);
    }

    //Changes the path to get results
    public void ModifyPath(string inputPath)
    {
        path = inputPath;
    }

    //Checks if a given string matches any if the givenList array strings
    private bool MatchString(string inputString)
    {
        bool returnBool = false;
        string offendingMatch = "";

        for (int i = 0; i < givenList.Length; i++) {
            returnBool = inputString.Contains(givenList[i]);
            offendingMatch = givenList[i];
        }

        switch (matchMode)
        {
            case match.excludeGiven:
            default:
                if (printMatchResults)
                {
                    if (returnBool) {
                        print("Removing result: '" + inputString + "' because it matches: '" + offendingMatch + "'");
                    }
                    else {
                        print("Including result: '" + inputString + "' because it did not match any given list options");
                    }
                }
                return returnBool;
            case match.includeGiven:
                if(printMatchResults)
                {
                    if(returnBool) {
                        print("Including result: '" + inputString + "' because it matches: '" + offendingMatch + "'");
                    } else {
                        print("Removing result: '" + inputString + "' because it did not match any given list options");
                    }
                }
                return !returnBool;
        }
    }
}