# Azure IOT Hub API
This API is a wrapper over the official C# Azure IOT Hub API to make it easier to call from other programs. It is very basic and is designed to be run as an Azure Function with a HTTP/HTTPs trigger.

This was used to get over the current limitations of the official Python API (v1 and v2), to allow easier direct method C2D calls.

It's a HTTP API with two endpoints, ```/api/GetDevices/``` and ```/api/DirectCall/```.

GET ```/api/GetDevices/``` Returns a list of devices currently associated with the IOT hub and their status (1 == online, 0 == offline)
```json
[{"device_name":"a","device_status":1},{"device_name":"b","device_status":0}]
```
POST ```/api/DirectCall/``` takes two headers ```method_name``` and ```target_device```, with a payload of whatever you want to send to your device.

It will then return a [JSON response](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-direct-methods#invoke-a-direct-method-from-a-back-end-app) that will depend on your implementation on the Azure Sphere.

Your Azure Sphere should have a callback function that looks something like this
```C 
static int DirectMethodCall(const char *methodName, const char *payload, size_t payloadSize, char **responsePayload, size_t *responsePayloadSize)
```
This function should be set as your direct method callback with
```C 
AzureIoT_SetDirectMethodCallback(&DirectMethodCall);
```



