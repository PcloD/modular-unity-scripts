## modular-unity-scripts
*A collection of simple 'drag-n-drop' style unity C# scripts that satisfy common game development requirements.*

### Preface

> The idea is to speed up unity developer workflow by providing ready-made scripts that can be quickly dropped on an object and then set up *exclusively* through easy to understand public variables.
The target audience of these scripts includes unity developers of all skill levels - tooltips should explain public variable functionality as if the user is a complete beginner.
If nothing else these scripts may serve as a good starting point or reference for more complex / coarse grained scripts.

### Catalogue

View the [CATALOGUE.md](./CATALOGUE.md) file for a list of all script names, categories and descriptions.

### Usage

1. Clone this repository and place the `ModularScripts` folder inside your Unity Project's `Assets` directory
2. Drag-n-drop a script onto a Unity gameobject
3. Configure the script's public variables to your liking (hover over any particular property for a description of what effect it will have)

### DIY Template

If you want to quickly generate a template that follows the script conventions outlined at the bottom of this readme, follow these steps:

1. Clone this repository and place the `Editor` folder inside the **root** of your Unity Project's `Assets` directory
2. Choose `Edit->Generate Self-Describing C# Script`
3. Choose a directory for your new script
4. Rename both the `NewScript` filename as well as the `public class **NewScript**` (note: they must be the same)

---

### Categories

| name | description |
| --- | --- |
| `effect`      | visual effects - aesthetic stuff that looks pretty |
| `management`  | object/file management - toggling / renaming of objects etc. |
| `messaging`   | event/script triggering based on a condition - send messaging when entered a collider etc. |
| `motion`      | gameobject/character movement whether it be transform or physics based |
| `spawning`    | instantiation and destruction of objects |
| `tools`       | handy miscellaneous scripts |
| `ui`          | 2D legacy GUI and unity canvas scripts |

---

### Conventions

All scripts must have a:

1. PascalCase name - e.g. `ScriptName`
2. Self-describing comment at the beginning of the class detailing the author, year and function of the script

   ~~~csharp
   /* Script created by Lachlan McKay 2016 ©
    * This script allows you to clamp the transform of an object */
   ~~~
   
3. Neatly formatted public variables with logical groupings and helpful tooltips
   
   ~~~csharp
   [Header("Main Options")]
   [Tooltip("Foo string says foo.")]
   public string foo;
   ~~~
   
4. Credits and description block as shown:

   ~~~csharp
   	//Credits and description
	[Header("_© Lachlan McKay 2016_")]
	[TextArea(2,2)]
	public string ScriptDescription = "Script description.";
    private string ScriptTags = "script name search tags space delimited";
    private string ScriptCategory = "category";
   ~~~
   
5. Public `ScriptDescription` string that describes the function of the script
6. Private `ScriptTags` string that contains search terms related to the script
7. Private `ScriptCategory` string that loosely categorizes the script - see above for available categories
8. `ToggleScript()` function with single bool input that enables/disables script functionality (where applicable)

   ~~~csharp
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
   ~~~