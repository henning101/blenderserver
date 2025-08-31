using System.Numerics;
using UnityEngine;

public class LightRotator : MonoBehaviour
{
    [SerializeField] private FloatValue lightRotation;

    void Update()
    {
        transform.rotation = UnityEngine.Quaternion.Slerp(transform.rotation, UnityEngine.Quaternion.Euler(lightRotation.GetValue(), transform.eulerAngles.x, transform.eulerAngles.z), 1);
    }
}
