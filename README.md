# Automatic Emergency Braking To Save Lives
We simulated an the visualisation from the classified .CSV data in Unity, using C#.

## Simulation & Scene
The scene is about a car turning out from a paralel parking spot then it drives forward. 
In the end of the video a pedestrian crosses the street in front of the vehicle. The relevant object regarding the task is the crossing pedestrian in the end marked as the "yellow banana men".

### Classification
The scene is a CPNCO (Car to Pedestrian Nearside Child Obscured) scene. 
![scene](https://github.com/RevellFTW/codelikeabosch/blob/main/Animation.gif "Scene")

## Our solution
We also created an innovative AEB solution with Forward Collision Warning (Honk and Flash).
The car will activate the FCW if the brake distance is only one meter longer than the distance of the object in the driving corridoor.
If the additional braking distance is even smaller (0.5m) we activate the Automatic Emergency Braking System.

## Code and Technical solution

### braking distance

To calculate the distance required to stop the car we implemented the following calculation:
```c#
public static float CalculatebrakeDistance(float vego, float aego, float maxJerk = -30f, float amax = -9f, float tlat = 0.2f, float v0 = 0f)
    {
        float brake_distance = 0;
        float t2 = (amax - aego) / maxJerk;
        float deltav1 = maxJerk / 2 * t2 * t2 + aego * t2;
        float v1 = v0 + deltav1;
        float deltav2 = -v1;
        float t3 = deltav2 / amax;
        float distanceOneLatency = vego * tlat + aego / 2 * tlat * tlat;
        float distanceTwobraking = maxJerk / 6 * t2 * t2 * t2 + aego / 2 * t2 * t2 + vego * t2;
        float distannceThreeFullbrake = amax / 2 * t3 * t3 + v1 * t3;
        brake_distance = distanceOneLatency + distanceTwobraking + distannceThreeFullbrake;

        return brake_distance;
    }
```
The final `brake_distance` consists of the distance that the car travels between the object detection and the braking. This is a so-called latency distance. 
After we calculate the second part of the braking distance which consist of the travelled distance untill the full braking preasure is built up. 
The last part of the braking distance calculation is calculated with full braking. 

### Collision avoidance
There are multiple techniques to avoid collision once the braking distance is calculated. 
We can steer the car if the collision can be avoided. In the same time we also can start the braking and warning the other participants in the traffic, allowing them to start their avoiding manouver.

We only implemented a simple solution with Warning and Braking as a proof of concept, but the optimal solution can be much more complex.

```c#
 public float AvoidCollision(float brake_distance, float distance_of_object)
    {
        if (brake_distance-distance_of_object <=  1f)
        {
            honk.SetActive(true);
            lights.SetActive(true);
        } else
        {
            honk.SetActive(false);
            lights.SetActive(false);
        }
        if (brake_distance - distance_of_object <=  0.5f)
        {
            brake.SetActive(true);

        } else { brake.SetActive(false); }
        return 1f;
    }
```
