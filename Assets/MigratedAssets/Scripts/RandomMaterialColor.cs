using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMaterialColor : MonoBehaviour
{
    // Update is called once per frame
    public void GenerateMaterialColor()
    {
        foreach(Transform child in transform)
        {
            if(!child.gameObject.activeSelf)
            {
                continue;
            }
            foreach(Transform part in child)
            {
                if (part.gameObject.GetComponent<Renderer>())
                {
                    List<Material> myMaterials = new List<Material>(part.gameObject.GetComponent<Renderer>().materials);
                    if (part.name.Contains("Body"))
                    {
                        foreach (Material mat in myMaterials)
                        {
                            mat.color = new Color(Random.Range(0.4f, 1.0f), Random.Range(0.4f, 1.0f), Random.Range(0.4f, 1.0f), Random.Range(0.6f, 1.0f));
                        }
                        part.gameObject.GetComponent<Renderer>().materials = myMaterials.ToArray();
                    }
                    else
                    {
                        // Other Parts
                        foreach (Material mat in myMaterials)
                        {

                            mat.color = Random.ColorHSV(0f, 1f, 0f, 1f, 0.4f, 1f);//new Color(Random.Range(0.2f, 1.0f), Random.Range(0.2f, 1.0f), Random.Range(0.2f, 1.0f));
                        }
                        part.gameObject.GetComponent<Renderer>().materials = myMaterials.ToArray();
                    }

                }
            }
        }
    }
}
