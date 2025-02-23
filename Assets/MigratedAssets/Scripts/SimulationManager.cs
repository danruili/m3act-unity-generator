using System.Collections;
using System.Collections.Generic;


using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Perception.Randomization.Scenarios;
using UnityEngine.SceneManagement;

public class SimulationManager : MonoBehaviour
{
    public int NumOfSimulation = 10;
    public GameObject SimulationScenario;//envs,active_env;
    //public GameObject GroupManager;
    //public GameObject Envs;
    public static int _NumOfSimulation { get; set; }
    private int IterationCount;
   // public List<GameObject> Children;

    // Todo:
    // 1. Get the "Iteration Count" variable from Simulation Scenario object
    // 2. Create a coroutine that reloads the scene every "Iteration Count" frame

    // To reload the scene, use the following:
    // SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(_NumOfSimulation);
        //Debug.Log(NumOfSimulation);
        /*foreach (Transform child in envs.transform)
        {

            Children.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }
        index = Random.Range(0, Children.Count);

        active_env = Children[index];
        active_env.SetActive(true);

        */



        IterationCount = SimulationScenario.GetComponent<FixedLengthScenario>().constants.iterationCount;
        if (_NumOfSimulation < NumOfSimulation - 1)
        {
            StartCoroutine(ReloadScene());
        }
        Debug.Log(_NumOfSimulation);
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ReloadScene()
    {
        while (true)
        {
            //Debug.Log(SimulationScenario.GetComponent<FixedLengthScenario>().currentIteration);
            //Debug.Log(IterationCount);
            if (SimulationScenario.GetComponent<FixedLengthScenario>().currentIteration == IterationCount - 1)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                _NumOfSimulation += 1;
            }
            yield return null;
        }
    }
}
