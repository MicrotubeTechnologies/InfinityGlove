using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using BleInputdll;


//NOTE: Upon building for windows standalone, make sure architecture is x86_64 (64 bits) for BLE functionality to work
public class BleComm : MonoBehaviour
{
    float m_Timer = 10.0f;
    private float deltat; 

    public MTPL MTPL_PCB;

    // global parameters for 9 DoF fusion and AHRS (Attitude and Heading Reference System)
    public float heading, pitch, roll, heading_neutral = 0, pitch_neutral = 0, roll_neutral = 0;

    public float[] sensorArray = new float[16];
    public float[] ExtendSensorArray = new float[] { 9999, 9999, 9999, 9999, 9999 };
    public float[] FlexSensorArray = new float[] { 0, 0, 0, 0, 0 };
    public float[] sensors;

    BLE ble;
    BLE.BLEScan scan;
    public bool isScanning = false, _connected = false, isTimerRunning = false, isCalibration = false;
    string deviceId = null;
    IDictionary<string, string> discoveredDevices = new Dictionary<string, string>();
    int devicesCount = 0;

    // BLE Threads 
    Thread scanningThread, connectionThread, readingThread, serialthread, calibrationThread;

    // GUI elements
    public Text TextThread, CalibrationText; string screentext, m_text;
    public Button ButtonCalibrate, ButtonNeutral;

    public float x, y, z;


    void Start()
    {
        ble = new BLE();
        MTPL_PCB = new MTPL();
        sensors = new float[] { 50, 50, 50, 50, 50, 50, 50, 50, 50 };
        readingThread = new Thread(ReadBleData);
        ButtonCalibrate.gameObject.SetActive(false);
        ButtonNeutral.gameObject.SetActive(false);

    }

    void Update()
    {
        deltat = Time.deltaTime/100;

        //Scan BLE devices 
        if (isScanning)
        {

            if (discoveredDevices.Count > devicesCount)
            {
                foreach (KeyValuePair<string, string> entry in discoveredDevices)
                {
                    Debug.Log("Added device: " + entry.Key);
                }
                devicesCount = discoveredDevices.Count;
            }
        }


        // The target device was found.
        if (deviceId != null && deviceId != "-1")
        {
            // Target device is connected and GUI knows.
            if (ble.isConnected && _connected)
            {
                ButtonCalibrate.gameObject.SetActive(true);
                ButtonNeutral.gameObject.SetActive(true);
                if (!readingThread.IsAlive)
                {
                    readingThread = new Thread(ReadBleData);
                    readingThread.Start();
                }
            }
            // Target device is connected, but GUI hasn't updated yet.
            else if (ble.isConnected && !_connected)
            {
                //ButtonEstablishConnection.enabled = false;
                _connected = true;
                TextThread.text = "Connected to target device!\n";
                Debug.Log("Connected to target device!\n");
            }
            else if (!_connected)
            {
                Debug.Log("Found target device but not connected.\n");
            }
        }

        // Display unto UI
        TextThread.text = screentext;

        //Timer for calibration
        if (isTimerRunning)
        {
            if (ButtonCalibrate.enabled)
            {
                ButtonCalibrate.enabled = false;
                ExtendSensorArray = new float[] { 9999, 9999, 9999, 9999, 9999 };
                FlexSensorArray = new float[] { 0, 0, 0, 0, 0 };
            }

            if (m_Timer < 0)
            {
                isTimerRunning = false;
                ButtonCalibrate.enabled = true;
                m_Timer = 10;
            }

            m_Timer -= Time.deltaTime;
            CalibrationText.text = m_text;
            calibrationThread = new Thread(CalibrateFingers);
            calibrationThread.Start();
        }
    }


    /* Functions to handle BLE */

    public void setScan()
    {
        StartScanHandler();
    }

    //Start BLE Scan
    public void StartScanHandler()
    {
        devicesCount = 0;
        isScanning = true;
        discoveredDevices.Clear();
        deviceId = null;
        Debug.Log("Scanning...");
        screentext = "Scanning.. ";
        scanningThread = new Thread(ScanBleDevices);
        scanningThread.Start();

    }

    // Start establish BLE connection with
    // target device in dedicated thread.
    public void StartConHandler()
    {
        connectionThread = new Thread(ConnectBleDevice);
        connectionThread.Start();
    }

    // Scan BLE devices
    private void ScanBleDevices()
    {
        scan = BLE.ScanDevices();
        Debug.Log("BLE.ScanDevices() started.");
        screentext = "BLE.ScanDevices() started.";
        scan.Found = (_deviceId, deviceName) =>
        {

            discoveredDevices.Add(_deviceId, deviceName);

            //if found the target device, immediately stop scan and attempt to connect
            if (deviceId == null && deviceName.Contains(MTPL_PCB.targetDevice()))
            {
                Debug.Log("Found device!");
                screentext = "Found device!";
                deviceId = _deviceId;
                StartConHandler();
            }
        };

        scan.Finished = () =>
        {
            isScanning = false;
            screentext = "scan finished";
            Debug.Log("scan finished");
            if (deviceId == null)
                deviceId = "-1";
        };
        while (deviceId == null)
            Thread.Sleep(500);
        scan.Cancel();
        scanningThread = null;
        isScanning = false;

        if (deviceId == "-1")
        {
            screentext = "no device found!";
            Debug.Log("no device found!");
            return;
        }
    }

    // Connect BLE device
    private void ConnectBleDevice()
    {
        if (deviceId != null)
        {
            try
            {
                ble.Connect(deviceId,
                MTPL_PCB.targetUuid(),
                MTPL_PCB.targetCharacteristics().ToArray());
            }
            catch (Exception e)
            {
                Debug.Log("Could not establish connection to device with ID " + deviceId + "\n" + e);
            }
        }
        if (ble.isConnected)
            Debug.Log("Connected to Device! " );
            screentext = "Connected to Device! " ;
    }

    // Read BLE Data
    private void ReadBleData(object obj)
    {
        byte[] bytes = BLE.ReadBytes(248); //data input via bytes
        screentext = "Reading Data\n";
        Debug.Log("time: " + deltat);   
        MTPL.ProcessByteData(in bytes, in deltat, out sensors);

        foreach (float sensor in sensors)
        {
            screentext += sensor.ToString() + ",";
            //Debug.Log(sensor + ",");
        }

        sensorArray = sensors;
        roll = sensors[5] - roll_neutral;
        pitch = sensors[6] - pitch_neutral;
        heading = sensors[7] - heading_neutral;   
    }



    // Reset BLE handler
    public void ResetHandler()
    {
        // Reset previous discovered devices
        discoveredDevices.Clear();
        deviceId = null;
        CleanUp();

    }
    


    public void NeutralButton()
    {
        roll_neutral = SetNeutral(roll_neutral, roll);
        pitch_neutral = SetNeutral(pitch_neutral, pitch);
        heading_neutral = SetNeutral(heading_neutral, heading);
    }

    private float SetNeutral(float neutral, float x)
    {
        if (neutral + x > 180) { return neutral + x - 360; }
        if (neutral + x < -180) { return neutral + x + 360; }
        return neutral + x;
    }


    /* Functions to initiate calibration */

    // Begin calibration of fingers
    public void StartCalibrationHandler()
    {

        isTimerRunning = true;
    }

    // Store calibrated values
    public void CalibrateFingers()
    {

        if (m_Timer > 5)
        {

            for (int i = 0; i < ExtendSensorArray.Length; i++)
            {
                ExtendSensorArray[i] = (sensorArray[i] + ExtendSensorArray[i]) / 2; //obtain a moving average
            }

            m_text = "Starting calibration: please open your fingers fully for " + (m_Timer - 5f).ToString("00") + " seconds.\n" + "Extend fingers: " + ExtendSensorArray[0] + "," + ExtendSensorArray[1] + "," + ExtendSensorArray[2] + "," + ExtendSensorArray[3];
        }

        else
        {
            if (m_Timer > 0)
            {
                for (int i = 0; i < FlexSensorArray.Length; i++)
                {
                    FlexSensorArray[i] = (sensorArray[i] + FlexSensorArray[i]) / 2; //obtain a moving average
                }
                m_text = "Please close your fingers fully for " + m_Timer.ToString("00") + " seconds.\n" + "Flex fingers: " + FlexSensorArray[0] + "," + FlexSensorArray[1] + "," + FlexSensorArray[2] + "," + FlexSensorArray[3];
            }

            else
            {
                m_text = "Calibration is done!\n" + "Extend fingers: " + ExtendSensorArray[0] + "," + ExtendSensorArray[1] + "," + ExtendSensorArray[2] + "," + ExtendSensorArray[3] + "\n" + "Flex fingers: " + FlexSensorArray[0] + "," + FlexSensorArray[1] + "," + FlexSensorArray[2] + "," + FlexSensorArray[3];

                isCalibration = true;
            }
        }
    }




    // Handle GameObject destroy
    private void OnDestroy()
    {
        ResetHandler();

    }

    // Handle Quit Game
    private void OnApplicationQuit()
    {
        ResetHandler();

    }

    // Prevent threading issues and free BLE stack.
    // Can cause Unity to freeze and lead
    // to errors when omitted.
    private void CleanUp()
    {
        try
        {
            scan.Cancel();
        }
        catch (NullReferenceException e)
        {
            Debug.Log("Scan never initialized.\n" + e);
        }


        try
        {
            ble.Close();
        }
        catch (NullReferenceException e)
        {
            Debug.Log("ble never initialized.\n" + e);
        }

        try
        {
            scanningThread.Abort();
        }
        catch (NullReferenceException e)
        {
            Debug.Log("Scan thread never initialized.\n" + e);
        }

        try
        {
            connectionThread.Abort();
        }
        catch (NullReferenceException e)
        {
            Debug.Log("Connection thread never initialized.\n" + e);
        }
    }


}