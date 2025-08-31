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
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class BlenderServer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    [SerializeField] Material material;

    [SerializeField] List<FloatValue> floatValues;

    [SerializeField] private string url;

    [SerializeField] private TeleportationArea teleportationArea;

    [SerializeField] private InteractionLayerMask interactionLayers;

    [SerializeField] private List<string> logs = new List<string>();

    [SerializeField] private FloatValue rotationX;
    [SerializeField] private FloatValue rotationY;
    [SerializeField] private FloatValue rotationZ;

    private GameObject waveformObj;

    private bool initialUpdate = true;

    void Start()
    {
        Log("Entering coroutine ...");
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            Log(ip.ToString());
        }
        StartCoroutine(GetRequest());
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

    private IEnumerator GetRequest()
    {
        while (true)
        {
            if (ValueModified() || initialUpdate)
            {
                Log($"Mesh update requested from {url} ({DateTime.Now})");
                initialUpdate = false;

                using (UnityWebRequest webRequest = UnityWebRequest.Post(url, GeneratePostData(), "application/json"))
                {
                    yield return webRequest.SendWebRequest();

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
                            Log($"Mesh received from {url} ({DateTime.Now})");
                            UpdateMesh(webRequest.downloadHandler.text);
                            break;
                    }
                }
            }
            yield return new WaitForSeconds(1f);
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

            DestroyWaveformObj();
            CreateWaveformObj(objStream, mtlStream);
        }
        catch (Exception e)
        {
            Log(e.ToString());
        }
    }

    private void CreateWaveformObj(MemoryStream objStream, MemoryStream mtlStream)
    {
        waveformObj = new OBJLoader().Load(objStream, mtlStream);
        //GameObject targetGeometry = waveformObj.GetNamedChild("TargetGeometry");
        //targetGeometry.GetComponent<MeshRenderer>().sharedMaterial = material;

        foreach (Transform child in waveformObj.transform)
        {
            GameObject childGo = child.gameObject;
            childGo.GetComponent<MeshRenderer>().sharedMaterial = material;

            MeshCollider targetCollider = childGo.AddComponent<MeshCollider>();
            waveformObj.transform.position = transform.position;
            targetCollider.sharedMesh = childGo.GetComponent<MeshFilter>().sharedMesh;
            targetCollider.convex = false;
        }

        TeleportationArea area = waveformObj.AddComponent<TeleportationArea>();
        area.interactionLayers = interactionLayers;

        BlenderObject bo = waveformObj.AddComponent<BlenderObject>();
        bo.rotationX = rotationX;
        bo.rotationY = rotationY;
        bo.rotationZ = rotationZ;
    }

    private void DestroyWaveformObj()
    {
        if (waveformObj)
        {
            Destroy(waveformObj);
        }
    }

    private void Log(string text)
    {
        logs.Add(text);

        if (logs.Count > 5)
        {
            logs.RemoveAt(0);
        }
        this.text.text = string.Join('\n', logs);
    }

    void Update()
    {

    }
}
