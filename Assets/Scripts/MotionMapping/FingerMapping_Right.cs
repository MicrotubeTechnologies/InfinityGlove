using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerMapping_Right : MonoBehaviour
{
    public Transform[,] Joints_left = new Transform[5, 3];

    private Vector3[,] roFinger_left = new Vector3[5, 3];

    private Vector3[,] roStraightFinger_left = new Vector3[5, 3]
    {
        {new Vector3(60, -70, -40), new Vector3(0, 0, 0), new Vector3(0, 0, 0)},
        {new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0)},
        {new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0)},
        {new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0)},
        {new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0)}
    };
    private Vector3[,] roGraspFinger_left = new Vector3[5, 3]
    {
        {new Vector3(43, -137, -104), new Vector3(0, 0, -12), new Vector3(0, 0, -30)},
        //{new Vector3(39, -134, -86), new Vector3(0, 0, -12), new Vector3(0, 0, -30)},
        {new Vector3(0, 0, -50), new Vector3(0, 0, -30), new Vector3(0, 0, -10)},
        {new Vector3(0, 0, -50), new Vector3(0, 0, -40), new Vector3(0, 0, -10)},
        {new Vector3(0, 0, -50), new Vector3(0, 0, -45), new Vector3(0, 0, -10)},
        {new Vector3(0, 0, -50), new Vector3(0, 0, -40), new Vector3(0, 0, -10)}
    };
    private Vector3[,] roCurlFinger_left = new Vector3[5, 3]
    {
        {new Vector3(23, -160, -125), new Vector3(0, 0, -45), new Vector3(0, 0, -80)},
        //{new Vector3(39, -140, -90), new Vector3(0, 0, -45), new Vector3(0, 0, -60)},
        {new Vector3(0, 0, -70), new Vector3(0, 0, -100), new Vector3(0, 0, -55)},
        {new Vector3(0, 0, -90), new Vector3(0, 0, -95), new Vector3(0, 0, -70)},
        {new Vector3(0, 0, -85), new Vector3(0, 0, -95), new Vector3(0, 0, -70)},
        {new Vector3(0, 0, -80), new Vector3(0, 0, -90), new Vector3(0, 0, -60)}
    };


    private float timeCount = 0.05f;

    private double[] fingerMin = new double[] {10000, 10000, 10000, 10000, 10000};
    private double[] fingerMax = new double[5];

    private double[] microtubeData = new double[5];

    private BleComm bleinput;

    void Start()
    {
        bleinput = GetComponent<BleComm>();
        string side = "Left";
        string letter = "L";

        //Objects mapping
        Joints_left[0, 0] = transform.Find("baseMeshHand_" + side + "_GRP/" + letter + "_Wrist/" + letter + "_Palm/" + letter + "_thumb_meta");
        Joints_left[0, 1] = transform.Find("baseMeshHand_" + side + "_GRP/" + letter + "_Wrist/" + letter + "_Palm/" + letter + "_thumb_meta/" + letter + "_thumb_a");
        Joints_left[0, 2] = transform.Find("baseMeshHand_" + side + "_GRP/" + letter + "_Wrist/" + letter + "_Palm/" + letter + "_thumb_meta/" + letter + "_thumb_a/" + letter + "_thumb_b");
        Joints_left[1, 0] = transform.Find("baseMeshHand_" + side + "_GRP/" + letter + "_Wrist/" + letter + "_Palm/" + letter + "_index_meta/" + letter + "_index_a");
        Joints_left[1, 1] = transform.Find("baseMeshHand_" + side + "_GRP/" + letter + "_Wrist/" + letter + "_Palm/" + letter + "_index_meta/" + letter + "_index_a/" + letter + "_index_b");
        Joints_left[1, 2] = transform.Find("baseMeshHand_" + side + "_GRP/" + letter + "_Wrist/" + letter + "_Palm/" + letter + "_index_meta/" + letter + "_index_a/" + letter + "_index_b/" + letter + "_index_c");
        Joints_left[2, 0] = transform.Find("baseMeshHand_" + side + "_GRP/" + letter + "_Wrist/" + letter + "_Palm/" + letter + "_middle_meta/" + letter + "_middle_a");
        Joints_left[2, 1] = transform.Find("baseMeshHand_" + side + "_GRP/" + letter + "_Wrist/" + letter + "_Palm/" + letter + "_middle_meta/" + letter + "_middle_a/" + letter + "_middle_b");
        Joints_left[2, 2] = transform.Find("baseMeshHand_" + side + "_GRP/" + letter + "_Wrist/" + letter + "_Palm/" + letter + "_middle_meta/" + letter + "_middle_a/" + letter + "_middle_b/" + letter + "_middle_c");
        Joints_left[3, 0] = transform.Find("baseMeshHand_" + side + "_GRP/" + letter + "_Wrist/" + letter + "_Palm/" + letter + "_ring_meta/" + letter + "_ring_a");
        Joints_left[3, 1] = transform.Find("baseMeshHand_" + side + "_GRP/" + letter + "_Wrist/" + letter + "_Palm/" + letter + "_ring_meta/" + letter + "_ring_a/" + letter + "_ring_b");
        Joints_left[3, 2] = transform.Find("baseMeshHand_" + side + "_GRP/" + letter + "_Wrist/" + letter + "_Palm/" + letter + "_ring_meta/" + letter + "_ring_a/" + letter + "_ring_b/" + letter + "_ring_c");
        Joints_left[4, 0] = transform.Find("baseMeshHand_" + side + "_GRP/" + letter + "_Wrist/" + letter + "_Palm/" + letter + "_pinky_meta/" + letter + "_pinky_a");
        Joints_left[4, 1] = transform.Find("baseMeshHand_" + side + "_GRP/" + letter + "_Wrist/" + letter + "_Palm/" + letter + "_pinky_meta/" + letter + "_pinky_a/" + letter + "_pinky_b");
        Joints_left[4, 2] = transform.Find("baseMeshHand_" + side + "_GRP/" + letter + "_Wrist/" + letter + "_Palm/" + letter + "_pinky_meta/" + letter + "_pinky_a/" + letter + "_pinky_b/" + letter + "_pinky_c");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(ToStraightFingerPose());
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(ToGraspFingerPose());
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(ToCurlFingerPose());
        }

        if(bleinput._connected)
        {
            Array.Copy(bleinput.sensorArray, microtubeData, 5);
            UpdateMicrotubeRange();
            UpdateFingerPos();
        }



    }

    private static bool flag_FirstData = true;
    void UpdateMicrotubeRange()
    {
        if (flag_FirstData)
        {
            //Array.Copy(microtubeData, fingerMin, 5);
            Array.Copy(microtubeData, fingerMax, 5);
            flag_FirstData = false;
            return;
        }

        if (bleinput.isCalibration)
        {
            for (int i = 0; i < 5; i++)
            {
                fingerMin[i] = bleinput.ExtendSensorArray[i];
                fingerMax[i] = bleinput.FlexSensorArray[i];
            }
                
        }

        for (int i = 0; i < 5; i++)
        {
            if (microtubeData[i] < fingerMin[i] && microtubeData[i] > 500)
            {
                fingerMin[i] = microtubeData[i];
            }

            if (microtubeData[i] > fingerMax[i] && microtubeData[i] < 10000)
            {
                fingerMax[i] = microtubeData[i];
            }
        }

        Debug.Log("fingerMin = [" + fingerMin[0] + "\t" + fingerMin[1] + "\t" + fingerMin[2]
            + "\t" + fingerMin[3] + "\t" + fingerMin[4] + "]");
        Debug.Log("fingerMax = [" + fingerMax[0] + "\t" + fingerMax[1] + "\t" + fingerMax[2]
            + "\t" + fingerMax[3] + "\t" + fingerMax[4] + "]");
        Debug.Log("Data = [" + microtubeData[0] + "\t" + microtubeData[1] + "\t" + microtubeData[2]
            + "\t" + microtubeData[3] + "\t" + microtubeData[4] + "]");
    }

    private float[] k = new float[5];
    public float[] r = new float[5] { 0.4f, 0.4f, 0.5f, 0.5f, 0.5f };
    void UpdateFingerPos()
    {
        for (int i = 0; i < 5; i++)
        {

            k[i] = (float)((microtubeData[i] - fingerMin[i]) / (fingerMax[i] - fingerMin[i]));
            if (fingerMin[i] == fingerMax[i]) { k[i] = 1; }
        }

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (k[i] < r[i])
                {
                    roFinger_left[i, j] = Vector3.Lerp(roFinger_left[i, j], k[i] / r[i]
                        * (roGraspFinger_left[i, j] - roStraightFinger_left[i, j])
                        + roStraightFinger_left[i, j], timeCount);
                    Joints_left[i, j].localRotation = Quaternion.Euler(roFinger_left[i, j]);
                }
                else
                {
                    roFinger_left[i, j] = Vector3.Lerp(roFinger_left[i, j], (k[i] - r[i]) / r[i]
                        * (roCurlFinger_left[i, j] - roGraspFinger_left[i, j])
                        + roGraspFinger_left[i, j], timeCount);
                    Joints_left[i, j].localRotation = Quaternion.Euler(roFinger_left[i, j]);
                }
            }
        }

    }

    IEnumerator ToStraightFingerPose()
    {
        float t = 0;
        while (t < timeCount)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Joints_left[i, j].localRotation = Quaternion.Slerp(Joints_left[i, j].localRotation,
                        Quaternion.Euler(roStraightFinger_left[i, j]), t / timeCount);
                }
            }
            t += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ToGraspFingerPose()
    {
        float t = 0;
        while (t < timeCount)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Joints_left[i, j].localRotation = Quaternion.Slerp(Joints_left[i, j].localRotation,
                        Quaternion.Euler(roGraspFinger_left[i, j]), t / timeCount);
                }
            }
            t += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ToCurlFingerPose()
    {
        float t = 0;
        while (t < timeCount)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Joints_left[i, j].localRotation = Quaternion.Slerp(Joints_left[i, j].localRotation,
                        Quaternion.Euler(roCurlFinger_left[i, j]), t / timeCount);
                }
            }
            t += Time.deltaTime;
            yield return null;
        }
    }
}
