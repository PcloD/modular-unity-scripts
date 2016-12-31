## modular-unity-scripts
*A collection of simple 'drag-n-drop' style unity C# scripts that satisfy common game development requirements.*

### Catalogue

Back to [README.md](./README.md)

| name | category | description |
| --- | --- | --- |
| [Boundary](./ModularScripts/Boundary.cs)                             |   motion      |   Allows you to clamp the transform of an object
| [Follow](./ModularScripts/Follow.cs)                                 |   motion      |   Maintains the distance between two objects at all times
| [CharacterGibber](./ModularScripts/CharacterGibber.cs)               |   effect      |   Explodes a transform heirarchy into gibs
| [CloneEmitter](./ModularScripts/CloneEmitter.cs)                     |   effect      |   Clones an object and emits it as if it were a particle
| [DestroyAfterTime](./ModularScripts/DestroyAfterTime.cs)             |   spawning    |   Destroy an object after a period of time has elapsed
| [GetDropDownValue](./ModularScripts/GetDropDownValue.cs)             |   ui          |   Outputs the value of a dropdown component to a text UI component
| [GetFileOrDirList](./ModularScripts/GetFileOrDirList.cs)             |   management  |   Sends a list of file or directory names to a target object as a message
| [ImageWipe](./ModularScripts/ImageWipe.cs)                           |   effect      |   Scroll images across the screen in any direction
| [LightColorModifier](./ModularScripts/LightColorModifier.cs)         |   effect      |   Modifies a light's color value based on a gradient over time
| [LightIntensityModifier](./ModularScripts/LightIntensityModifier.cs) |   effect      |   Modifies a light's intensity over time based on a curve
| [LookAt](./ModularScripts/LookAt.cs)                                 |   motion      |   Make an object look at a target or manually specified position
| [MaterialColorModifier](./ModularScripts/MaterialColorModifier.cs)   |   effect      |   Modifies a material's color value based on gradient over time
| [MouseFollow](./ModularScripts/MouseFollow.cs)                       |   motion      |   This script moves an object in tandem with the mouse's viewport coordinates
| [MouseTrigger](./ModularScripts/MouseTrigger.cs)                     |   messaging   |   Sends a message to a target object on mouse click
| [MoveTowards](./ModularScripts/MoveTowards.cs)                       |   motion      |   Moves an object towards a target
| [ParticleCollision](./ModularScripts/ParticleCollision.cs)           |   effect      |   Spawns an object when this object's particle system collides with a surface
| [ParticleManager](./ModularScripts/ParticleManager.cs)               |   effect      |   Controls all values of multiple shuriken particle systems at runtime
| [PixelScrambler](./ModularScripts/PixelScrambler.cs)                 |   effect      |   Scrambles the pixels of a texture randomly
| [PopulateDropDown](./ModularScripts/PopulateDropDown.cs)             |   ui          |   Populates a target dropdown component with specified text/image values
| [PrefabSpawner](./ModularScripts/PrefabSpawner.cs)                   |   spawning    |   Spawns an object at the location of a target transform
| [RandomInterpolator](./ModularScripts/RandomInterpolator.cs)         |   motion      |   Interpolates an object between 2 vectors randomly
| [RayProjector](./ModularScripts/RayProjector.cs)                     |   spawning    |   Randomly fires raycasts and spawns new objects on ray collision
| [RotateToVelocity](./ModularScripts/RotateToVelocity.cs)             |   motion      |   Rotates an object to look in the direction of its movement
| [Scoop](./ModularScripts/Scoop.cs)                                   |   motion      |   Attaches two rigidbodies via fixed joint on collision or trigger
| [ScreenScaler](./ModularScripts/ScreenScaler.cs)                     |   ui          |   Scales a transform or text object based on the size of the viewport
| [ScriptMessenger](./ModularScripts/ScriptMessenger.cs)               |   messaging   |   Send messages to other scripts or toggle them based on a timer
| [SpriteCycler](./ModularScripts/SpriteCycler.cs)                     |   effect      |   Cycles through a sprite sheet (atlas) to give the appearance of animation
| [Toggle](./ModularScripts/Toggle.cs)                                 |   management  |   Toggles an object's active state when told by another script / button
| [TrailEmitter](./ModularScripts/TrailEmitter.cs)                     |   effect      |   Generates a trail - has multiple options for representing said trail
| [TransformConverter](./ModularScripts/TransformConverter.cs)         |   tools       |   Copy an object's transform, but in a different space
| [TriggerMessage](./ModularScripts/TriggerMessage.cs)                 |   messaging   |   Sends a message to a target when a trigger is activated
| [Walker](./ModularScripts/Walker.cs)                                 |   motion      |   Moves an object linearly in one direction
| [WaveArray](./ModularScripts/WaveArray.cs)                           |   effect      |   Generates an array of vectors - must be accessed via another script
| [WeldObject](./ModularScripts/WeldObject.cs)                         |   motion      |   Sticks an object to another's world position at all times
    