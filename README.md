# InfinityGlove
InfinityGlove is virtual reality glove with 5 fingers mapping and wrist motion capturing. 
It is a simple avatar that utilizes Microtube Technology's plugin to measure the finger bending and hand rotational based on Microtube Technology's sensor device.

Instructions for use:
1) Open Handtracking scene.
2) Press play on Unity Editor.
3) Switch on the InfinityGlove sensor device.
4) Click on BT Comm to establish communication with the device.
5) If the scan is not found, stop Unity Editor, restart the device and try again. 
6) If the scan is successful, the screen will display "Found target device!"
7) Wait for 1-2 seconds for the characteristics to be subscribed and data communication to be handled.
8) Once data is sent, you may bend your fingers which shows some movement on the hand avatar. 
9) To enable more accurate finger movements, click on Calibrate. Follow the instructions: Keep your hands open for 5 seconds, and close your hands for 5 seconds.
10) There is slight drift and inaccuracies in the IMU. Click on Set Neutral to return the hand avatar to the neutral hand position.


For developers:
1) More sensor data can be extracted from BleComm.cs in SensorArray:
    i) sensorArray[0] to sensorArray[4] ==> Sensor 1 to Sensor 5 
    ii) sensorArray[5] to sensorArray[7] ==> Roll, pitch, yaw
    iii) sensorArray[8] to sensorArray[10] ==> AccX, AccY, AccZ
    iv) sensorArray[11] to sensorArray[13] ==> GyroX, GyroY, GyroZ
    v) sensorArray[14] to sensorArray[16] ==> MagX, MagY, MagZ
    




