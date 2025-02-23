using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SceneRandomizer : MonoBehaviour
{
    [System.Serializable]
    public struct HDRIConfig
    {
        public Cubemap HDRIMap;
        public float rotation;
    }


    /// <summary>
    /// Configuration for one scene group
    /// All the scenes in the group will share the same volume and lighting configuration
    /// But the HDRI will be different
    /// </summary>
    [System.Serializable]
    public struct SceneGroupConfig
    {
        public string name;
        public bool enabled;
        public HDRIConfig[] HDRIMapConfigs;
        public GameObject volumeSettingObject;
        public GameObject lightSettingObject;
    }

    public SceneGroupConfig[] sceneConfigs;

    // store all light setting objects
    public GameObject lightsGroup;

    // store all volume setting objects
    public GameObject volumesGroup;

    // Start is called before the first frame update
    void Start()
    {
        // disable all children in lights and volumes
        foreach (Transform child in lightsGroup.transform)
        {
            child.gameObject.SetActive(false);
        }
        foreach (Transform child in volumesGroup.transform)
        {
            child.gameObject.SetActive(false);
        }

        // retrieve all enabled scene configs
        List<SceneGroupConfig> enabledSceneConfigs = new List<SceneGroupConfig>();
        foreach (SceneGroupConfig configItem in sceneConfigs)
        {
            if (configItem.enabled)
            {
                enabledSceneConfigs.Add(configItem);
            }
        }

        // pick a random scene config
        int randomSceneConfigIndex = Random.Range(0, enabledSceneConfigs.Count);
        SceneGroupConfig config = enabledSceneConfigs[randomSceneConfigIndex];

        // pick a random HDRI
        int randomHDRIIndex = Random.Range(0, config.HDRIMapConfigs.Length);
        HDRIConfig HDRIConfig = config.HDRIMapConfigs[randomHDRIIndex];

        // get the volume
        Volume globalVolume = config.volumeSettingObject.GetComponent<Volume>();

        // set the HDRI as the skybox of the volume profile
        if (globalVolume.profile.TryGet<HDRISky>(out var hdriSky))
        {
            // Initially set to the first HDRI sky in the array
            hdriSky.hdriSky.value = HDRIConfig.HDRIMap;

            // Rotate the skybox
            hdriSky.rotation.value = HDRIConfig.rotation;
        }

        // enable the volume and light setting objects
        config.volumeSettingObject.SetActive(true);
        config.lightSettingObject.SetActive(true);

    }

}
