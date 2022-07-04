//#define BT_Commu

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IMURight : MonoBehaviour
{

    public Transform rightHand;
    private float[] imuData;
    Quaternion initialRotation;
    Quaternion gyroInitialRotation;

    private BleComm bleinput;

    // Start is called before the first frame update
    void Start()
    {
        bleinput = GetComponent<BleComm>();
    }


    float speedFactor = 9.999f;

    void Update()
    {

        Quaternion targetRotation = Quaternion.Euler(
            bleinput.roll, 
            -bleinput.pitch, 
            bleinput.heading
            );

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speedFactor);

    }


}
