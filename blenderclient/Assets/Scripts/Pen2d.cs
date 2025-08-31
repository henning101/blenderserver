using System.Linq;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Pen2d : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler, IPointerUpHandler
{
    [SerializeField] private RawImage rawImage;

    [SerializeField] private Texture2D texture;

    [SerializeField] private Camera camera;

    //[SerializeField] private TextMeshProUGUI positionText;

    private bool drawing;

    public void OnPointerDown(PointerEventData eventData)
    {
        drawing = true;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            GetComponent<RectTransform>(),
            eventData.pointerCurrentRaycast.screenPosition,
            camera,
            out localPoint
        );
        localPoint += new Vector2(200 / 2, 200 / 2);
        localPoint *= 512.0f / 200.0f;

        //positionText.text = localPoint.ToString();

        if (drawing)
        {
            if (localPoint.x >= 0 && localPoint.x < texture.width)
            {
                if (localPoint.y >= 0 && localPoint.y < texture.height)
                {
                    texture.SetPixel((int)localPoint.x, (int)localPoint.y, Color.white);
                    texture.Apply();
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        drawing = false;
    }

    void Start()
    {
        Color[] colors = Enumerable.Repeat<Color>(Color.black, texture.width * texture.height).ToArray<Color>();
        texture.SetPixels(colors);
        texture.Apply();
    }

    void Update()
    {
        
    }
}
