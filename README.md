# modular-unity-scripts
A collection of simple 'drag-n-drop' style unity C# scripts that satisfy common game development requirements.

Preface
===============

The idea is to speed up unity developer workflow by having ready-made scripts that can be quickly dropped on an
object and then set up *exclusively* through easy to understand public variables.
If nothing else these scripts may serve as a good starting point or reference for more complex / coarse grained
scripts.


Catalogue
===============

View the CATALOGUE.md file for a list of all script names, categories and descriptions


Categories
===============

name			description
----------------------------
motion			gameobject/character movement whether it be transform or physics based
messaging		event/script triggering based on a condition - send messaging when entered a collider etc.
screen			everything related to 2d screen space - screen space conversions / UI etc.
effect			visual effects - aesthetic stuff that looks pretty
management		object/file management - toggling / renaming of objects etc.
spawning		instantiation and destruction of objects


Conventions
===============

All scripts must have a:
	1. PascalCase name - e.g. ScriptName
	2. Self-describing comment at the beginning of the class detailing the author, year and function of the script
	3. Neatly formatted public variables with logical groupings and helpful tooltips
	4. Credits and description block with the author and year as the header
	5. Public ScriptDescription string that describes the function of the script (same as 2. comment)
	6. Private ScriptTags string that contains search terms related to the script
	7. Private ScriptCategory string that loosely categorizes the script - see above for available categories
	8. ToggleScript() function with single bool input that enables/disables script functionality (where applicable)
	