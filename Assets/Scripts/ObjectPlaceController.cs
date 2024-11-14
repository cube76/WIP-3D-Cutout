using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectPlaceController : MonoBehaviour
{
    private Vector3 position;
    private Quaternion rotation;
    private Vector3 localScale;
    private bool inPlace = false;
    private float threshold = 0.5f;
    private GameObject textObject;
    GameObject canvasObject;
    private TextMeshProUGUI textMeshPro;

    void Start()
    {

        position = this.gameObject.transform.position;
        rotation = this.gameObject.transform.rotation;
        localScale = this.gameObject.transform.localScale;

        TextPerObject();

        canvasObject.SetActive(false);
    }


    void Update()
    {
    }


    public void CheckPosition(SelectExitEventArgs args)
    {
        if (inPlace)
        {
            if (this.gameObject.transform.position.y <= position.y + threshold && this.gameObject.transform.position.y >= position.y - threshold &&
                this.gameObject.transform.position.x <= position.x + threshold && this.gameObject.transform.position.x >= position.x - threshold &&
                this.gameObject.transform.position.z <= position.z + threshold && this.gameObject.transform.position.z >= position.z - threshold)
            {
                Debug.Log("not in place.");
                this.gameObject.transform.position = position;
                this.gameObject.transform.rotation = rotation;
                this.gameObject.transform.localScale = localScale;
                inPlace = false;
                //buttonDetail.SetActive(false);
                canvasObject.SetActive(false);
            }
        }
        else
        {
            if (this.gameObject.transform.position.y >= position.y + threshold || this.gameObject.transform.position.y <= position.y - threshold ||
                this.gameObject.transform.position.x >= position.x + threshold || this.gameObject.transform.position.x <= position.x - threshold ||
                this.gameObject.transform.position.z <= position.z + threshold || this.gameObject.transform.position.z >= position.z - threshold)
            {
                HandButton handMenuController = FindObjectOfType<HandButton>();
                if (!handMenuController.position.Contains("3main"))
                {
                    textMeshPro.fontSize = 0.01f;
                }
                else
                {
                    textMeshPro.fontSize = 0.04f;
                }
                if (gameObject.name == "Lower Case")
                {
                    RectTransform rectTransform = canvasObject.GetComponent<RectTransform>();
                    Vector3 changePosition = rectTransform.localPosition;
                    changePosition.y = 6f;
                    rectTransform.localPosition = changePosition;
                }
                inPlace = true;
                canvasObject.SetActive(true);
            }

        }
    }

    public void TextPerObject()
    {
        textObject = new GameObject("Title");
        textMeshPro = textObject.AddComponent<TextMeshProUGUI>();

        // Set the text and its properties
        textMeshPro.text = gameObject.name;
        textMeshPro.fontSize = 0.04f;
        textMeshPro.color = new Color32(16, 16, 16, 255); ;

        Canvas canvas = FindObjectOfType<Canvas>();
        canvasObject = new GameObject("Canvas");
        canvas = canvasObject.AddComponent<Canvas>();
        RectTransform rectTransformc = canvasObject.GetComponent<RectTransform>();
        rectTransformc.sizeDelta = new Vector2(0.2f, 0.05f);

        //  Add a CanvasScaler for better scaling on different resolutions
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        // Position 
        canvas.transform.SetParent(gameObject.transform);
        textObject.transform.SetParent(canvas.transform);
        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(0.2f, 0.05f);

        BoxCollider box = gameObject.GetComponentInParent<BoxCollider>();
        //float posz = -(box.size.z / 2);
        canvasObject.transform.localPosition = new Vector3(box.center.x, box.size.y / 2, box.center.z);
    }

    public void TextCameraOverlay()
    {
        canvasObject = GameObject.Find("CanvasText");
        GameObject textObject = GameObject.Find("ObjectNameText");
        textMeshPro = textObject.GetComponent<TextMeshProUGUI>();
        textMeshPro.text = gameObject.name;
    }
}
