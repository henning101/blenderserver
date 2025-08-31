using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FloatValueText : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI text;

    void Start()
    {
        SetValue(slider.value);
    }

    void Update()
    {
        SetValue(slider.value);
    }

    public void SetValue(float value)
    {
        text.text = $"{value:0.00}";
    }
}
