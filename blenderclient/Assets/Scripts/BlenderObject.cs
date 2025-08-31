using UnityEngine;

public class BlenderObject : MonoBehaviour
{
    public FloatValue rotationX { get; set; }
    public FloatValue rotationY { get; set; }
    public FloatValue rotationZ { get; set; }

    void Start()
    {
        
    }

    void Update()
    {
        transform.eulerAngles = new Vector3(rotationX.GetValue(), rotationY.GetValue(), rotationZ.GetValue());
    }
}
