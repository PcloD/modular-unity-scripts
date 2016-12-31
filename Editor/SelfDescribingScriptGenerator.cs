using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public static class SelfDescribingScriptGenerator
{
    [MenuItem("Edit/Generate Self-Describing C# Script")]
    public static void Generate()
    {
        // Try to find an existing file in the project called "NewScript.cs"
        string filePath = string.Empty;
        foreach (var file in Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories))
        {
            if (Path.GetFileNameWithoutExtension(file) == "NewScript")
            {
                filePath = file;
                break;
            }
        }

        // If no such file exists already, use the save panel to get a folder in which the file will be placed.
        if (string.IsNullOrEmpty(filePath))
        {
            string directory = EditorUtility.OpenFolderPanel("Choose location for NewScript.cs", Application.dataPath, "");

            // Canceled choose? Do nothing.
            if (string.IsNullOrEmpty(directory))
            {
                return;
            }

            filePath = Path.Combine(directory, "NewScript.cs");
        }

        // Write out our file
        using (var writer = new StreamWriter(filePath))
        {
            //Info
            writer.WriteLine("/* Script created by Author Name Year ©");
            writer.WriteLine(" * Script description goes here */");
            writer.WriteLine();

            //Dependencies
            writer.WriteLine("using UnityEngine;");
            writer.WriteLine("using System.Collections;");
            writer.WriteLine();

            // Class
            writer.WriteLine("public class NewScript : MonoBehaviour");
            writer.WriteLine("{");
            writer.WriteLine();

            //Main options
            writer.WriteLine("    [Header(\"Main Options\")]");
            writer.WriteLine("    [Tooltip(\"Foo string says foo.\")]");
            writer.WriteLine("    public string foo;");
            writer.WriteLine();

            //Debugging
            writer.WriteLine("    //Debugging");
            writer.WriteLine("    [Header(\"Debug Options\")]");
            writer.WriteLine("    [Tooltip(\"Display errors in the console.\")]");
            writer.WriteLine("    public bool debugConsole = false;");
            writer.WriteLine();

            //Credits and description
            writer.WriteLine("    //Credits and description");
            writer.WriteLine("    [Header(\"_© Author Name Year_\")]");
            writer.WriteLine("    [TextArea(2, 2)]");
            writer.WriteLine("    public string ScriptDescription = \"Script description.\";");
            writer.WriteLine("    private string ScriptTags = \"script name search tags space delimited\";");
            writer.WriteLine("    private string ScriptCategory = \"category\";");
            writer.WriteLine();

            //Active bool
            writer.WriteLine("    //Private");
            writer.WriteLine("    private bool active = false;");
            writer.WriteLine();

            //Start
            writer.WriteLine("    void Start () {");
            writer.WriteLine("	      if (ErrorChecking()) { Init(); } //Initialize the script only if we pass ErrorChecking");
            writer.WriteLine("    }");
            writer.WriteLine();

            //ErrorChecking
            writer.WriteLine("    //Make assertions about initial user defined values (via inspector) here.");
            writer.WriteLine("    //Print a helpful error message if debugConsole is enabled and if there is incorrect user setup.");
            writer.WriteLine("    bool ErrorChecking() {");
            writer.WriteLine("	      return true;");
            writer.WriteLine("    }");
            writer.WriteLine();

            //Init
            writer.WriteLine("    void Init() {");
            writer.WriteLine("	      //Initialization here");
            writer.WriteLine("	      active = true;");
            writer.WriteLine("    }");
            writer.WriteLine();

            //Update
            writer.WriteLine("    void Update() {");
            writer.WriteLine("	      if(active) { Master(); }");
            writer.WriteLine("    }");
            writer.WriteLine();

            //Master
            writer.WriteLine("    void Master() {");
            writer.WriteLine("	      //Script body here");
            writer.WriteLine("    }");
            writer.WriteLine();

            //ToggleScript
            writer.WriteLine("    //Enables or disables the script's update function");
            writer.WriteLine("    public void ToggleScript(bool state) {");
            writer.WriteLine("	      active = state;");
            writer.WriteLine("	      this.enabled = state;");
            writer.WriteLine("	      if (debugConsole) { print(\"Setting active state of \" + this.GetType().Name + \" script to: \" + state + \" at time: \" + Time.time); }");
            writer.WriteLine("    }");

            writer.WriteLine("}");
        }

        // Refresh
        AssetDatabase.Refresh();
    }
}