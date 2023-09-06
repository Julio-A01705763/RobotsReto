// TC2008B Modelación de Sistemas Multiagentes con gráficas computacionales
// C# client to interact with Python server via POST
// Sergio Ruiz-Loza, Ph.D. March 2021

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class WebClient : MonoBehaviour
{
    List<GameObject> instantiatedObjects = new List<GameObject>();

    IEnumerator FetchAndProcessSimulation()
    {
        string url = "http://localhost:8585/get-simulation";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Received: " + www.downloadHandler.text);
                ProcessAndGenerateObjects(www.downloadHandler.text);
            }
        }
    }

    public GameObject prefabToInstantiateX2;
    public GameObject prefabToInstantiateP; // Para "P"
    public GameObject prefabToInstantiateS; // Para robots
    public GameObject prefabToInstantiate1; // Para basura1
    public GameObject prefabToInstantiate2; // Para basura2
    public GameObject prefabToInstantiate3; // Para basura3
    public GameObject prefabToInstantiate4; // Para basura4

    public void DestroyInstantiatedObjects()
    {
        GameObject[] generatedObjects = GameObject.FindGameObjectsWithTag("GeneratedObject");
        foreach(GameObject obj in generatedObjects)
        {
            Destroy(obj);
        }
    }

    void ProcessAndGenerateObjects(string jsonPositions) 
    {

        ItemList itemList = JsonUtility.FromJson<ItemList>("{\"items\":" + jsonPositions + "}");
        foreach(Item item in itemList.items) 
        {
            GameObject createdObject = null;
            if(item.type == "X")
            {
                createdObject = Instantiate(prefabToInstantiateX2, item.position, Quaternion.identity);
            }
            else if(item.type == "P")
            {
                createdObject = Instantiate(prefabToInstantiateP, item.position, Quaternion.identity);
            }
            else if(item.type == "S")
            {
                createdObject = Instantiate(prefabToInstantiateS, item.position, Quaternion.identity);
            }
            else if(item.type == "1")
            {
                createdObject = Instantiate(prefabToInstantiate1, item.position, Quaternion.identity);
            }
            else if(item.type == "2")
            {
                createdObject = Instantiate(prefabToInstantiate1, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate2, item.position, Quaternion.identity);
            }
            else if(item.type == "3")
            {
                createdObject = Instantiate(prefabToInstantiate1, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate2, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate3, item.position, Quaternion.identity);
            }
            else if(item.type == "4")
            {
                createdObject = Instantiate(prefabToInstantiate1, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate2, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate3, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate4, item.position, Quaternion.identity);
            }
            else if(item.type == "5")
            {
                createdObject = Instantiate(prefabToInstantiate1, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate2, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate3, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate3, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate4, item.position, Quaternion.identity);
            }
            else if(item.type == "6")
            {
                createdObject = Instantiate(prefabToInstantiate1, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate2, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate3, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate4, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate2, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate3, item.position, Quaternion.identity);
            }
            else if(item.type == "7")
            {
                createdObject = Instantiate(prefabToInstantiate1, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate2, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate3, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate1, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate2, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate3, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate4, item.position, Quaternion.identity);
            }
            else if(item.type == "8")
            {
                createdObject = Instantiate(prefabToInstantiate1, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate2, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate3, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate4, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate1, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate2, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate3, item.position, Quaternion.identity);
                createdObject = Instantiate(prefabToInstantiate4, item.position, Quaternion.identity);
            }
            if (createdObject != null)
            {
                instantiatedObjects.Add(createdObject);
            }
            
        }
        
    }

    [System.Serializable]
    public class Item
    {
        public Vector3 position;
        public string type;
    }
    [System.Serializable]
    public class ItemList 
    {
        public List<Item> items;
    }

    IEnumerator FetchAndProcessSimulationPeriodically(float intervalInSeconds) 
    {
        while (true) // Bucle infinito
        {
            DestroyInstantiatedObjects();

            yield return StartCoroutine(FetchAndProcessSimulation());
            yield return new WaitForSeconds(intervalInSeconds);

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        float timeInterval = 1.2f;
        StartCoroutine(FetchAndProcessSimulationPeriodically(timeInterval));
    }

}