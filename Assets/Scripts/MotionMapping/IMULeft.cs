//#define BT_Commu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IMULeft : MonoBehaviour
{

    public Transform leftHand;
    Quaternion initialRotation;
    Quaternion gyroInitialRotation;

    private BleComm bleinput;

    // Start is called before the first frame update
    void Start()
    {
        initialRotation = transform.localRotation;
        bleinput = GetComponent<BleComm>();
    }


    float speedFactor = 9.999f;
    bool flag_InitialRotation = true;

    void Update()
    {

        if (flag_InitialRotation == true)
        {
            gyroInitialRotation = Quaternion.Euler(-bleinput.roll, bleinput.pitch, bleinput.heading);
            flag_InitialRotation = false;
        }


        Quaternion offsetRotation = Quaternion.Inverse(gyroInitialRotation) * Quaternion.Euler(bleinput.roll, bleinput.pitch, bleinput.heading);

        transform.localRotation = Quaternion.Lerp(transform.localRotation, initialRotation * offsetRotation, Time.deltaTime * speedFactor);

    }

}
