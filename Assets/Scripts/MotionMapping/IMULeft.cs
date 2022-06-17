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


    float speedFactor = 999.9f;
    bool flag_InitialRotation = true;

#if BT_Commu
    void Update()
    {
        if (BTCommu_Left.Instance.flag_RotationDataReady == false)
        {
            return;
        }
        if (flag_InitialRotation == true)
        {
            gyroInitialRotation = BTCommu_Left.Instance.rotation;
            flag_InitialRotation = false;
        }

        Quaternion offsetRotation = Quaternion.Inverse(gyroInitialRotation) * BTCommu_Left.Instance.rotation;
        //transform.localRotation = initialRotation * offsetRotation;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, initialRotation * offsetRotation, Time.deltaTime * speedFactor);

    }
#else
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
#endif

}
