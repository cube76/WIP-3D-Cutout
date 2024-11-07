using TMPro;
using UnityEngine;

public class ChangeText : MonoBehaviour
{
    private string text;
    public TMP_Text messageText;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        messageText.SetText(text);
    }

    public void SetText(string value)
    {
        text = value;
    }
}
