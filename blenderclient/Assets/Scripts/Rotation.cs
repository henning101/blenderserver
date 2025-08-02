using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] Vector3 rotation;

    void Start()
    {
    }

    void Update()
    {
        this.transform.Rotate(rotation * Time.deltaTime);
    }
}
