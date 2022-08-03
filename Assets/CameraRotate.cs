using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    float rx = 0;
    float ry = 0;
    float mx0;
    float my0;
    public float rotSpeed = 400;
    // Start is called before the first frame update
    void Start()
    {
        mx0 = Input.GetAxis("Mouse X");
        my0 = Input.GetAxis("Mouse Y");
    }

    // Update is called once per frame
    void Update()
    {
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        rx += rotSpeed * (mx - mx0) * Time.deltaTime;
        ry += rotSpeed * (my - my0) * Time.deltaTime;

        ry = Mathf.Clamp(ry, -90, 90);
        transform.eulerAngles = new Vector3(-ry, rx, 0);
    }
}
