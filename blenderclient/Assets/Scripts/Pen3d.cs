using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

public class Pen3d : MonoBehaviour
{
    [SerializeField] private XRInputValueReader<float> m_TriggerInput = new XRInputValueReader<float>("Trigger");
    [SerializeField] private GameObject penTip;
    [SerializeField] private Material penTipDefault;
    [SerializeField] private Material penTipDrawing;
    [SerializeField] private GameObject lineRendererPrefab;
    [SerializeField] private List<LineRenderer> lineRenderers;

    private bool drawing;

    private LineRenderer activeLineRenderer = null;

    void Start()
    {

    }

    private void ClearLineRenderer(LineRenderer lr)
    {
        lr.SetPositions(new UnityEngine.Vector3[] {});
    }

    void Update()
    {
        if (m_TriggerInput.ReadValue() > 0.1f)
        {
            drawing = true;
            penTip.GetComponent<MeshRenderer>().sharedMaterial = penTipDrawing;

            if (activeLineRenderer == null)
            {
                activeLineRenderer = Instantiate(lineRendererPrefab).GetComponent<LineRenderer>();
                lineRenderers.Add(activeLineRenderer);
                ClearLineRenderer(activeLineRenderer);
                activeLineRenderer.positionCount = 1;
                activeLineRenderer.SetPosition(0, penTip.transform.position); // Set start position
            }

            // Add a line segment if movement exceeds threshold:
            if (UnityEngine.Vector3.Distance(activeLineRenderer.GetPosition(activeLineRenderer.positionCount - 1), penTip.transform.position) > .01f)
            {
                activeLineRenderer.positionCount = activeLineRenderer.positionCount + 1;
                activeLineRenderer.SetPosition(activeLineRenderer.positionCount - 1, penTip.transform.position);
            }
        }
        else
        {
            drawing = false;
            penTip.GetComponent<MeshRenderer>().sharedMaterial = penTipDefault;
            activeLineRenderer = null;
        }
    }
}
