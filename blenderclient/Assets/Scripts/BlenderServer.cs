using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using Dummiesman;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

public class BlenderServer : MonoBehaviour
{
    [SerializeField] TextMeshPro text;

    [SerializeField] Material material;

    [SerializeField] List<FloatValue> floatValues;

    private GameObject waveformObj;

    private int logNum = 0;

    private bool initialUpdate = true;

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

    private string GeneratePostData()
    {
        List<string> rows = new List<string>();
        foreach (FloatValue floatValue in floatValues)
        {
            rows.Add("\"" + floatValue.name + "\": " + floatValue.GetValue());
        }

        string output = "{" + string.Join(", ", rows) + "}";

        Debug.Log(output);

        return output;
    }

    private bool ValueModified()
    {
        bool valueModified = false;
        foreach (FloatValue floatValue in floatValues)
        {
            if (floatValue.GetValueModified())
            {
                floatValue.SetValueModified(false);
                valueModified = true;
            }
        }
        return valueModified;
    }

    private IEnumerator GetRequest(string uri)
    {
        while (true)
        {
            if (ValueModified() || initialUpdate)
            {
                Debug.Log("UPDATING GEOMETRY");
                initialUpdate = false;

                using (UnityWebRequest webRequest = UnityWebRequest.Post(uri, GeneratePostData(), "application/json"))
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
            }
            yield return new WaitForSeconds(0.01f);
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
            waveformObj.transform.position = transform.position;
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
