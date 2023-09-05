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
    public GameObject prefabToInstantiateX; // Para "X"
    public GameObject prefabToInstantiateP; // Para "P"
    public GameObject prefabToInstantiateS; // Para robots
    public GameObject prefabToInstantiate1; // Para basura1
    public GameObject prefabToInstantiate2; // Para basura2
    public GameObject prefabToInstantiate3; // Para basura3
    public GameObject prefabToInstantiate4; // Para basura4

    void ProcessAndGenerateObjects(string jsonPositions) 
    {
        
        MapData mapData = JsonUtility.FromJson<MapData>(jsonPositions);
        foreach(Item item in mapData.positions) 
        {
            if(item.type == "X")
            {
                Instantiate(prefabToInstantiateX, item.position, Quaternion.identity);
            }
            else if(item.type == "P")
            {
                Instantiate(prefabToInstantiateP, item.position, Quaternion.identity);
            }
            else if(item.type == "S")
            {
                Instantiate(prefabToInstantiateS, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiateS, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiateS, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiateS, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiateS, item.position, Quaternion.identity);
            }
            else if(item.type == "1")
            {
                Instantiate(prefabToInstantiate1, item.position, Quaternion.identity);
            }
            else if(item.type == "2")
            {
                Instantiate(prefabToInstantiate1, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate2, item.position, Quaternion.identity);
            }
            else if(item.type == "3")
            {
                Instantiate(prefabToInstantiate1, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate2, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate3, item.position, Quaternion.identity);
            }
            else if(item.type == "4")
            {
                Instantiate(prefabToInstantiate1, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate2, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate3, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate4, item.position, Quaternion.identity);
            }
            else if(item.type == "5")
            {
                Instantiate(prefabToInstantiate1, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate2, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate3, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate3, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate4, item.position, Quaternion.identity);
            }
            else if(item.type == "6")
            {
                Instantiate(prefabToInstantiate1, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate2, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate3, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate4, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate2, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate3, item.position, Quaternion.identity);
            }
            else if(item.type == "7")
            {
                Instantiate(prefabToInstantiate1, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate2, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate3, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate1, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate2, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate3, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate4, item.position, Quaternion.identity);
            }
            else if(item.type == "8")
            {
                Instantiate(prefabToInstantiate1, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate2, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate3, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate4, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate1, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate2, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate3, item.position, Quaternion.identity);
                Instantiate(prefabToInstantiate4, item.position, Quaternion.identity);
            }
            
        }
        
        AdjustCameraToMap(mapData.map_dimensions.width, mapData.map_dimensions.depth);


    }

    public void AdjustCameraToMap(int mapWidth, int mapDepth)
    {

        float marginFactor = 1.1f;  // 10% adicional

        // Define la posición de la cámara basada en el ancho y la profundidad del mapa
        float cameraXPosition = mapWidth * (2f / 3f);
        float cameraZPosition = mapDepth * (2f / 3f);

        float cameraYPosition = mapWidth * (38.7f / 30f) * marginFactor;

        Camera.main.transform.position = new Vector3(cameraXPosition, cameraYPosition, cameraZPosition);
        Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0);
        
        // Opcional: Si quieres ajustar el FOV, descomenta la siguiente línea y ajusta el valor según lo necesites.
        Camera.main.fieldOfView = 50;  // Ajusta el valor 60 según lo necesites
    }



    [System.Serializable]
    public class Item
    {
        public Vector3 position;
        public string type;
    }

    [System.Serializable]
    public class PositionsList
    {
        public List<Item> data;
    }

    [System.Serializable]
    public class MapData
    {
        public MapDimensions map_dimensions;
        public List<Item> positions;
    }

    [System.Serializable]
    public class MapDimensions
    {
        public int width;
        public int depth;
    }


    // IEnumerator - yield return
    IEnumerator SendData(string data)
    {
        WWWForm form = new WWWForm();
        form.AddField("bundle", "the data");
        string url = "http://localhost:8585";
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(data);
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            //www.SetRequestHeader("Content-Type", "text/html");
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();          // Talk to Python
            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
             {
                Debug.Log(www.downloadHandler.text);    // Answer from Python
                ProcessAndGenerateObjects(www.downloadHandler.text);
                //Debug.Log("Form upload complete!");
            }
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        //string call = "What's up?";
        //StartCoroutine(SendData(call));
        StartCoroutine(SendData(""));
        // transform.localPosition
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}