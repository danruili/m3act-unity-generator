using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGenerator : MonoBehaviour
{
    public GameObject cameraPrefab;
    public int numOfCameras;
    public int radius;
    public float camera_height;
    public List<GameObject> cameras;
    // Start is called before the first frame update
    void Start()
    {
        int distance = 6 / (numOfCameras + 1);
        for (int i = 0; i < numOfCameras-1; i++)
        {
            float theta = (float)(i+1) * 2.0f * Mathf.PI / (float)numOfCameras;
            Debug.Log(theta);
            float x = Mathf.Sin(theta) * radius;
            float z = Mathf.Cos(theta) * radius;
            float tx = Mathf.Sin(theta) * (float)(radius);
            float tz = Mathf.Cos(theta) * (float)(radius);
            GameObject camera = Instantiate(cameraPrefab);
            camera.transform.position = new Vector3(tx, camera_height, tz);
            camera.transform.LookAt(new Vector3(0f, 0f, 0f));
            cameras.Add(camera);
        }
    }

    
}
