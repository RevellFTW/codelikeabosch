# We simulated an the visualisation from the classified .CSV data in Unity, using C#

The scene is about a car turning out from a paralel parking spot then it drives forward. 
In the end of the video a pedestrian crosses the street in front of the vehicle.

## Simulation & Scene
The scene is a CPNCO (Car to Pedestrian Nearside Child Obscured) scene. 
![scene](https://github.com/RevellFTW/codelikeabosch/blob/main/Animation.gif "Scene")

## Our solution
We also created an innovative AEB solution with Forward Collision Warning (Honk and Flash).
The car will activate the FCW if the break distance is only one meter longer than the distance of the object in the driving corridoor.
If the additional breaking distance is even smaller (0.5m) we activate the Automatic Emergency Braking System.
