## modular-unity-scripts
*A collection of simple 'drag-n-drop' style unity C# scripts that satisfy common game development requirements.*

### Catalogue


| name | category | description |
| --- | --- | --- |
| [Boundary](./library/Boundary.cs)								|	motion		|	Allows you to clamp the transform of an object
| [Follow](./library/Follow.cs)									|	motion		|	Maintains the distance between two objects at all times
| [CharacterGibber](./library/CharacterGibber.cs)				|	effect		|	Explodes a transform heirarchy into gibs
| [CloneEmitter](./library/CloneEmitter.cs)						|	effect		|	Clones an object and emits it as if it were a particle
| [DestroyAfterTime](./library/DestroyAfterTime.cs)				|	spawning	|	Destroy an object after a period of time has elapsed
| [GetDropDownValue](./library/GetDropDownValue.cs)				|	space		|	Outputs the value of a dropdown component to a text UI component
| [GetFileOrDirList](./library/GetFileOrDirList.cs)				|	management	|	Sends a list of file or directory names to a target object as a message
| [ImageWipe](./library/ImageWipe.cs)							|	effect		|	Scroll images across the screen in any direction
| [LightColorModifier](./library/LightColorModifier.cs)			|	effect		|	Modifies a light's color value based on a gradient over time
| [LightIntensityModifier](./library/LightIntensityModifier.cs)	|	effect		|	Modifies a light's intensity over time based on a curve
| [LookAt](./library/LookAt.cs)									|	motion		|	Make an object look at a target or manually specified position
| [MaterialColorModifier](./library/MaterialColorModifier.cs)	|	effect		|	Modifies a material's color value based on gradient over time
| [MouseFollow](./library/MouseFollow.cs)						|	motion		|	This script moves an object in tandem with the mouse's viewport coordinates
| [MouseTrigger](./library/MouseTrigger.cs)						|	messaging	|	Sends a message to a target object on mouse click
| [MoveTowards](./library/MoveTowards.cs)						|	motion		|	Moves an object towards a target
| [ParticleCollision](./library/ParticleCollision.cs)			|	effect		|	Spawns an object when this object's particle system collides with a surface
| [ParticleManager](./library/ParticleManager.cs)				|	effect		|	Controls all values of multiple shuriken particle systems at runtime
| [PixelScrambler](./library/PixelScrambler.cs)					|	effect		|	Scrambles the pixels of a texture randomly
| [PopulateDropDown](./library/PopulateDropDown.cs)				|	space		|	Populates a target dropdown component with specified text/image values
| [PrefabSpawner](./library/PrefabSpawner.cs)					|	spawning	|	Spawns an object at the location of a target transform
| [RandomInterpolator](./library/RandomInterpolator.cs)			|	motion		|	Interpolates an object between 2 vectors randomly
| [RayProjector](./library/RayProjector.cs)						|	spawning	|	Randomly fires raycasts and spawns new objects on ray collision
| [RotateToVelocity](./library/RotateToVelocity.cs)				|	motion		|	Rotates an object to look in the direction of its movement
| [ScreenScaler](./library/ScreenScaler.cs)						|	space		|	Scales a transform or text object based on the size of the viewport
| [ScriptMessenger](./library/ScriptMessenger.cs)				|	messaging	|	Send messages to other scripts or toggle them based on a timer
| [SpriteCycler](./library/SpriteCycler.cs)						|	effect		|	Cycles through a sprite sheet (atlas) to give the appearance of animation
| [Toggle](./library/Toggle.cs)									|	management	|	Toggles an object's active state when told by another script / button
| [TrailEmitter](./library/TrailEmitter.cs)						|	effect		|	Generates a trail - has multiple options for representing said trail
| [TransformConverter](./library/TransformConverter.cs)			|	space		|	Copy an object's transform, but in a different space
| [WaveArray](./library/WaveArray.cs)							|	effect		|	Generates an array of vectors - must be accessed via another script
| [WeldObject](./library/WeldObject.cs)							|	motion		|	Sticks an object to another's world position at all times
	