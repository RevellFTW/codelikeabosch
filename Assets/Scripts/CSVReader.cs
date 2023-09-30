using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class CSVReader : MonoBehaviour
{
    static int frame_count = 0;

    public GameObject car;
    public GameObject lights;
    public GameObject honk;
    public GameObject brake;


    public GameObject firstObject;
    public GameObject secondObject;
    public GameObject thirdObject;
    public GameObject fourthObject;
    List<List<Obstacle>> obstacles = new List<List<Obstacle>>();

    List<float> timeStamps = new List<float>();
    List<float> yawRates = new List<float>();
    List<float> convertedYawRates = new List<float>();
    List<float> egoSpeed = new List<float>();

    void Start()
    {
        honk.SetActive(false);
        lights.SetActive(false);
        brake.SetActive(false);


        string path = "Assets/Scripts/Data/inputData.csv";

        using (var reader = new StreamReader(path))
        {
            var header = reader.ReadLine();
            string[] splittedHeader = header.Split(",");

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] splittedLine = line.Split(",");
                timeStamps.Add(float.Parse(splittedLine[splittedLine.Length - 1]));
                yawRates.Add(float.Parse(splittedLine[splittedLine.Length - 2]));
                egoSpeed.Add(float.Parse(splittedLine[9]) / 256);   // m/s
                List<Obstacle> obstacle_frames = new List<Obstacle>();
                for (int i = 0; i < 4; i++)
                {
                    obstacle_frames.Add(new Obstacle()
                    {
                        distance_x = float.Parse(splittedLine[1 + i]) / 128,    // m
                        distance_y = float.Parse(splittedLine[2 + i]) / 128,   // m
                        speed_x = Int32.Parse(splittedLine[10 + i]) / 256,     // m/s
                        speed_y = Int32.Parse(splittedLine[11 + i]) / 256     // m/s
                    });
                    obstacles.Add(obstacle_frames);
                };

            }
        }
        for (int i = 0; i < yawRates.Count; i++)
        {
            float yawRateInDegreesPerSecond = yawRates[i] * 180 / Mathf.PI;
            convertedYawRates.Add(yawRateInDegreesPerSecond);
        }
    }

    void LateUpdate()
    {
        if (frame_count < obstacles.Count)
        {
            if (frame_count < convertedYawRates.Count)
            {
                car.transform.rotation = Quaternion.Euler(0, 0, convertedYawRates[frame_count]);
            }
            firstObject.transform.position = new Vector2(obstacles[frame_count][0].distance_x, obstacles[frame_count][0].distance_y);
            if (firstObject.transform.position.x < 2 && firstObject.transform.position.y < 2)
            {
                firstObject.SetActive(false);

            }
            else
            {
                firstObject.SetActive(true);
            }
            secondObject.transform.position = new Vector2(obstacles[frame_count][1].distance_x, obstacles[frame_count][1].distance_y);
            if (secondObject.transform.position.x < 2 && secondObject.transform.position.y < 2)
            {
                secondObject.SetActive(false);

            }
            else
            {
                secondObject.SetActive(true);
            }
            thirdObject.transform.position = new Vector2(obstacles[frame_count][2].distance_x, obstacles[frame_count][2].distance_y);
            if (thirdObject.transform.position.x < 2 && secondObject.transform.position.y < 2)
            {
                thirdObject.SetActive(false);

            }
            else
            {
                thirdObject.SetActive(true);
            }
            fourthObject.transform.position = new Vector2(obstacles[frame_count][3].distance_x, obstacles[frame_count][3].distance_y);
            if (fourthObject.transform.position.x < 2 && secondObject.transform.position.y < 2)
            {
                fourthObject.SetActive(false);

            }
            else
            {
                fourthObject.SetActive(true);
            }
            // collision avoidance
            if (frame_count > 0 && frame_count < timeStamps.Count)
            {
                for (int i = 0; i < 1; i++)
                {
                    float egoAcceleration = (egoSpeed[frame_count] - egoSpeed[frame_count - 1]) / (timeStamps[frame_count] - timeStamps[frame_count - 1]);
                    float breakDistance = CalculateBreakDistance(vego: egoSpeed[frame_count], aego: egoAcceleration);
                    float object_i_distance = (float)Math.Sqrt(
                        Math.Pow(obstacles[frame_count][i].distance_x, 2f) +
                        Math.Pow(obstacles[frame_count][i].distance_y, 2f));
                    AvoidCollision(breakDistance, object_i_distance);


                }
            }
            frame_count++;

            if (IsColliding(firstObject.transform.position) || IsColliding(secondObject.transform.position) || IsColliding(thirdObject.transform.position) || IsColliding(fourthObject.transform.position))
            {
                // Collision detected, apply avoidance action
                AvoidCollision(0, 0);
            }
        }
    }

    bool IsColliding(Vector2 relativeObstaclePosition)
    {
        // Assuming you have a collider attached to the stationary car
        Collider2D carCollider = car.GetComponent<Collider2D>();

        // Check for collision using the car's collider
        return carCollider.OverlapPoint(relativeObstaclePosition);
    }

    public class Obstacle
    {
        public float distance_x { get; set; }
        public float distance_y { get; set; }
        public float speed_x { get; set; }
        public float speed_y { get; set; }
    }

    public static float CalculateBreakDistance(float vego, float aego, float maxJerk = -30f, float amax = -9f, float tlat = 0.2f, float v0 = 0f)
    {
        float break_distance = 0;
        float t2 = (amax - aego) / maxJerk;
        float deltav1 = maxJerk / 2 * t2 * t2 + aego * t2;
        float v1 = v0 + deltav1;
        float deltav2 = -v1;
        float t3 = deltav2 / amax;
        float distanceOneLatency = vego * tlat + aego / 2 * tlat * tlat;
        float distanceTwoBreaking = maxJerk / 6 * t2 * t2 * t2 + aego / 2 * t2 * t2 + vego * t2;
        float distannceThreeFullBreak = amax / 2 * t3 * t3 + v1 * t3;
        break_distance = distanceOneLatency + distanceTwoBreaking + distannceThreeFullBreak;

        return break_distance;
    }

    public float AvoidCollision(float break_distance, float distance_of_object)
    {
        if (break_distance-distance_of_object <=  1f)
        {
            honk.SetActive(true);
            lights.SetActive(true);
        } else
        {
            honk.SetActive(false);
            lights.SetActive(false);
        }
        if (break_distance - distance_of_object <=  0.5f)
        {
            brake.SetActive(true);

        } else { brake.SetActive(false); }
        return 1f;
    }
}
