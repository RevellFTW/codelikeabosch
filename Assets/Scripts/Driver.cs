using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour
{
    [SerializeField] float steerSpeed = 0.1f;
    [SerializeField] public static float moveSpeed;
    public static Vector2 currentPosition;
    Vector2 oldposition = Vector2.zero;

    public static float speedLimit = 15f;
    public static int cargoLimit = 20;

    public static bool canMove = true;

    public static float upgradedSpeed { get; internal set; }
    public static int upgradedStorage { get; internal set; }

    // Start is called before the first frame update
    void Start()
    {
        oldposition = new Vector2(transform.position.x, transform.position.y);
        moveSpeed = 9f;
    }

    // Update is called once per frame
    void Update()
    {

        if (canMove)
        {
            float steerAmount = Input.GetAxis("Horizontal") * steerSpeed * Time.deltaTime;
            float moveAmount = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
            if (moveAmount != 0)
            {
                if (moveAmount < 0)
                {
                    steerAmount = -steerAmount;
                }

                transform.Rotate(0, 0, -steerAmount);
            }
            transform.Translate(0, moveAmount, 0);
        }
        currentPosition = new Vector2(transform.position.x, transform.position.y);
        if (currentPosition != oldposition)
        {
            oldposition = currentPosition;
        }
    }



    public static void setMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

}
