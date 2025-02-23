using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SceneRandomizerPlus : MonoBehaviour
{

    // Light settings
    public Light randomLight;
    // values for light intensity
    public float minLightIntensity = 300f;
    public float maxLightIntensity = 45000f;
    // values for light temperature
    public float minLightTemperature = 2000f;
    public float maxLightTemperature = 12000f;
    // values for light rotation
    public float minLightRotation = 20f;
    public float maxLightRotation = 90f;


    // insert a divider in unity inspector
    [Space(10)]

    public Volume randomVolume;

    // insert a divider in unity inspector
    [Space(10)]

    // system file folders that contains all the hdri maps
    public string[] hdriFolder;


    // Start is called before the first frame update
    void Start()
    {
        
        UpdateSetting();
    }

    public void UpdateSetting()
    {
        // randomize light intensity in Lux
        var intensity = Random.Range(minLightIntensity, maxLightIntensity);
        Debug.Log("Random light intensity: " + intensity);
        // get the hdadditionallightdata component from the light
        randomLight.GetComponent<HDAdditionalLightData>().SetIntensity(intensity);

        // randomize light temperature
        randomLight.colorTemperature = Random.Range(minLightTemperature, maxLightTemperature);
        Debug.Log("Random light temperature: " + randomLight.colorTemperature);

        // randomize light rotation on x axis
        randomLight.transform.rotation = Quaternion.Euler(Random.Range(minLightRotation, maxLightRotation), 0, 0);
        //Debug.Log("Random light rotation: " + randomLight.transform.rotation.eulerAngles.x);

        // extract all hdri map file path from the system file folders
        List<string> hdriMapPath = new();
        foreach (string folder in hdriFolder)
        {
            string[] hdriMap = System.IO.Directory.GetFiles(folder, "*.exr", System.IO.SearchOption.AllDirectories);
            foreach (string map in hdriMap)
            {
                hdriMapPath.Add(map);
            }
            hdriMap = System.IO.Directory.GetFiles(folder, "*.hdr", System.IO.SearchOption.AllDirectories);
            foreach (string map in hdriMap)
            {
                hdriMapPath.Add(map);
            }
        }
        Debug.Log("Total HDRI maps: " + hdriMapPath.Count);

        // randomize hdri map
        randomVolume.profile.TryGet(out HDRISky hdriSky);
        int randomHdriIndex = Random.Range(0, hdriMapPath.Count);
        //Debug.Log("Random HDRI index: " + randomHdriIndex);
        string randomHdriMapPath = hdriMapPath[randomHdriIndex];
        // remove the file path before "Resources" folder
        randomHdriMapPath = randomHdriMapPath.Substring(randomHdriMapPath.IndexOf("Resources") + 10);
        // remove the file extension
        randomHdriMapPath = randomHdriMapPath.Substring(0, randomHdriMapPath.LastIndexOf("."));
        Debug.Log("Random HDRI map: " + randomHdriMapPath);

        // load the cube map from the file path
        Cubemap randomHdriMap = Resources.Load<Cubemap>(randomHdriMapPath);
        hdriSky.hdriSky.value = randomHdriMap;

        // randomize hdri rotation
        hdriSky.rotation.value = Random.Range(0, 360);
        //Debug.Log("Random HDRI rotation: " + hdriSky.rotation.value);

        // randomize hdri intensity in desired lux value
        hdriSky.skyIntensityMode.value = SkyIntensityMode.Lux;
        hdriSky.desiredLuxValue.value = intensity;


    }

}
