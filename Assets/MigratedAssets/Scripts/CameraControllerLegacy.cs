using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerLegacy : MonoBehaviour
{

    public GameObject cameraPrefab;
    public int numOfCameras;
    public int radius;
    public float min_camera_height;
    public float max_camera_height;
    private float camera_height;
    public List<GameObject> cameras;
    public float ambient_intensity = 2.0f;
    public Vector3 center = Vector3.zero;
    public Vector3[] positions;
    // Start is called before the first frame update
    void Awake()
    {
        camera_height = Random.Range(min_camera_height, max_camera_height);
        RenderSettings.ambientIntensity = Random.Range(ambient_intensity, ambient_intensity * 10);
        generateCameras();
        // controlCameras();
        updateCameraViews();
    }
    private void generateCameras()
    {
        cameras.Add(transform.gameObject);
        for (int i = 0; i < numOfCameras - 1; i++)
        {
            GameObject camera = Instantiate(cameraPrefab);
            cameras.Add(camera);
        }

    }

    public void controlCameras()
    {
        // Control main camera view 
        //int index = Random.Range(0, 5);
        //int num = Random.Range(1, 10);
        /*float theta0 = Random.Range(0f, 1f);
        float theta = theta0 * 2 * Mathf.PI;
        float x = Mathf.Sin(theta) * radius;
        float z = Mathf.Cos(theta) * radius;
        while (System.Single.IsNaN(x) || System.Single.IsNaN(z))
        {
            //index = Random.Range(0, 5);
            //num = Random.Range(0, 10);
            theta = theta0 * 2.0f * Mathf.PI;
            x = Mathf.Sin(theta) * radius;
            z = Mathf.Cos(theta) * radius;
        }*/
      
        // control other cameras
        int distance = 6 / (numOfCameras + 1);
        for (int i = 0; i < numOfCameras - 1; i++)
        {
            /*theta = (float)(i + 1) * 2.0f * Mathf.PI / (float)numOfCameras + theta0 * 2f * Mathf.PI;
            //Debug.Log(theta);
            //x = Mathf.Sin(theta) * radius;
            //z = Mathf.Cos(theta) * radius;
            float tx = Mathf.Sin(theta) * (float)(radius);
            float tz = Mathf.Cos(theta) * (float)(radius);*/
            GameObject camera = Instantiate(cameraPrefab);
            camera.transform.LookAt(new Vector3(center.x, -1.05f, center.z));
            cameras.Add(camera);
        }

        transform.LookAt(new Vector3(center.x, -1.05f, center.z));
    }

    public void updateCameraViews()
    {
        Vector3 cameraCenter = new Vector3(center.x, center.y, center.z - radius);
        int i = 0;
        HashSet<int> used = new HashSet<int>();
        int index = Random.Range(0, positions.Length);
        /*float theta0 = Random.Range(0f, 1f);*/
        foreach (GameObject camera in cameras)
        {

            index = Random.Range(0, positions.Length);
            while (used.Contains(index))
            {
                index = Random.Range(0, positions.Length);
            }
            used.Add(index);
            /*float theta = (float)(i + 1) * 2.0f * Mathf.PI / (float)numOfCameras + theta0 * 2f * Mathf.PI;
            float tx = Mathf.Sin(theta) * (float)(radius);
            float tz = Mathf.Cos(theta) * (float)(radius);
            camera.transform.position = new Vector3(tx + center.x, camera_height, tz + center.z);*/
            camera.transform.position = new Vector3(positions[index].x + Random.Range(-1, 1), camera_height, positions[index].z + Random.Range(-1, 1));
            camera.transform.LookAt(new Vector3(center.x, 0f, center.z));
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

