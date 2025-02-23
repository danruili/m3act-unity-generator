using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool enable = true;

    // receive external modification
    public Vector3 center = Vector3.zero;

    // positional configurations
    public float min_camera_height = 1.5f;
    public float max_camera_height = 1.5f;
    public float min_rotation_in_degree = 0f;
    public float max_rotation_in_degree = 360f;
    public float min_radius = 12f;
    public float max_radius = 15f;

    // camera configurations
    public GameObject cameraObj;
    public float min_fov = 30f;
    public float max_fov = 60f;

    // public float ambient_intensity = 2.0f;

    void Start()
    {        
        // RenderSettings.ambientIntensity = Random.Range(ambient_intensity, ambient_intensity * 10);
        
        updateCameraViews();
    }




    public void updateCameraViews()
    {
        if (!enable)
        {
            return;
        }

        GameObject camera = cameraObj;

        // randomly choose a fov
        float fov = Random.Range(min_fov, max_fov);
        camera.GetComponent<Camera>().fieldOfView = fov;

        // randomly choose a position
        float radius = Random.Range(min_radius, max_radius);
        float theta0 = Random.Range(min_rotation_in_degree, max_rotation_in_degree);
        Vector3 positionalOffset = new(Mathf.Sin(theta0 * Mathf.Deg2Rad) * radius, 0f, Mathf.Cos(theta0 * Mathf.Deg2Rad) * radius);

        // set camera position and orientation
        float camera_height = Random.Range(min_camera_height, max_camera_height);
        camera.transform.position = new Vector3(positionalOffset.x + center.x, camera_height + center.y, positionalOffset.z + center.z);
        camera.transform.LookAt(new Vector3(center.x, 1.5f + center.y, center.z));

    }

}

