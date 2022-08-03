using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 5;
    public float gravity = -9.81f;
    public float jumpPower = 10;
    float yVelocity = 0;
    float speedMul = 1;
    bool noGravity = false;
    CharacterController controller;
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //날 때와 날지 않을 때 차이 만들기
        if (Input.GetButtonDown("Fly"))
        {
            noGravity = !noGravity;
        }
        if (!noGravity)
        {
            yVelocity += gravity * Time.deltaTime;
            if (Input.GetButtonDown("Jump"))
            {
                yVelocity = jumpPower;
            }
        }
        else
        {
            if (Input.GetButton("Jump"))
            {
                if(Input.GetButton("LShift"))
                {
                    yVelocity = -jumpPower;
                }
                else
                {
                    yVelocity = jumpPower;
                }
                
            }
            else
            {
                yVelocity = 0;
            }
            
        }
        if (Input.GetButton("Faster"))
        {
            speedMul = 2;
        }
        else
        {
            speedMul = 1;
        }


        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        


        Vector3 dir = Vector3.right * h + Vector3.forward * v;
        dir = Camera.main.transform.TransformDirection(dir);
        dir.y = 0;
        dir.Normalize();
        dir *= speed * speedMul;
        dir.y = yVelocity;

        controller.Move(dir * Time.deltaTime);
    }
}
