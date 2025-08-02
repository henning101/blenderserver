using System;
using System.Collections;
using System.Diagnostics.Tracing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using Dummiesman;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Networking;

public class BlenderServer : MonoBehaviour
{
    [SerializeField] TextMeshPro text;

    [SerializeField] Material material;

    private GameObject waveformObj;

    int logNum = 0;

    void Start()
    {
        Log("Entering coroutine ...");
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            Log(ip.ToString());
        }
        StartCoroutine(GetRequest("http://192.168.1.135:8000"));
    }

    private IEnumerator GetRequest(string uri)
    {
        while (true)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                Log("Coroutine entered");

                yield return webRequest.SendWebRequest();

                Log("Web request sent");

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        Log("Connection Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        Log("Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Log("HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Log("Mesh received");
                        UpdateMesh(webRequest.downloadHandler.text);
                        break;
                }
            }
            yield return new WaitForSeconds(1);
        }
    }

    public void UpdateMesh(string data)
    {
        try
        {
            string obj = data.Split("#####")[0];
            string mtl = data.Split("#####")[1];

            Debug.Log(obj);

            Debug.Log(mtl);

            byte[] objByteArray = Encoding.UTF8.GetBytes(obj);
            MemoryStream objStream = new MemoryStream(objByteArray);

            byte[] mtlByteArray = Encoding.UTF8.GetBytes(mtl);
            MemoryStream mtlStream = new MemoryStream(mtlByteArray);

            if (waveformObj)
            {
                Destroy(waveformObj);
            }

            waveformObj = new OBJLoader().Load(objStream, mtlStream);
            //waveformObj.GetNamedChild("Cube").GetComponent<MeshRenderer>().material = material;
        }
        catch (Exception e)
        {
            Log(e.ToString());
        }

    }

    private void Log(string text)
    {
        this.text.text = this.text.text + "\n" + text;
        logNum++;

        if (logNum > 30)
        {
            logNum = 0;
            this.text.text = "";
        }
    }

    void Update()
    {
        
    }
}
