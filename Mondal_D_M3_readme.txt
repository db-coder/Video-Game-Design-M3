Name: Dibyendu Mondal
email: dibyendu@gatech.edu
TSquare account name: dmondal6

Completed Requirements:

Added my name to the 2D HUD.
Created a NPC with 4 AI states: Path Planning, Reach Vantage Point, Throw, Chase GameObject.
NPC uses Unity's NavMesh and built-in A* support to navigate to its desired destinations.
On each Update() call, the AI transitions to one of the states randomly.
The player visits stationary waypoints.
On pressing the Space bar, it chases the moving game object.(Forced transition)
On pressing the Left Control key, it goes to a vantage point and throws a projectile towards the moving gameobject.(Forced transition)
Displayed the current AI state on the HUD canvas.
Added informative sound effects to denote various events like hitting moving gameobject, throwing projectile, reaching waypoints etc.

Resources used:
1. Footstep, jump, bear, gas leak etc sounds from Unity Asset Store.
2. Tutorials for footstep sounds:
	i. https://www.youtube.com/watch?v=ISoBKFxQLic
	ii. https://www.youtube.com/watch?v=ih8gyGeC7xs
3. Tutorial for Configurable Joints:
	i. https://www.youtube.com/watch?v=4X18N3g0mSk
4. Prefabs like bear statue, log from asset store.
5. Particle system prefab from asset store.
6. Predictive Aim tutorial:
	i. https://www.gamasutra.com/blogs/KainShin/20090515/83954/Predictive_Aim_Mathematics_for_AI_Targeting.php
7. Various Mathf and Vector3 function documentations in Unity documentation website.

Main scene file: demo.unity
Build target: Windows