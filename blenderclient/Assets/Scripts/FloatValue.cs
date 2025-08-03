using UnityEngine;

[CreateAssetMenu(fileName = "FloatValue", menuName = "Scriptable Objects/FloatValue")]
public class FloatValue : ScriptableObject
{
    [SerializeField] private float value;

    [SerializeField] private bool valueModified;

    public void SetValue(float value)
    {
        if (value != this.value)
        {
            this.value = value;
            valueModified = true;
        }
    }

    public float GetValue()
    {
        return value;
    }

    public bool GetValueModified()
    {
        return valueModified;
    }

    public void SetValueModified(bool valueModified)
    {
        this.valueModified = valueModified;
    }
}
