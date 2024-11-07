using MixedReality.Toolkit.UX;
using UnityEngine;

public class HandButton : MonoBehaviour
{
    public string position = "";
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InputExpander(GameObject gameObject)
    {
        PressableButton button = this.gameObject.GetComponent<PressableButton>();
        ObjectManager modelExpander = gameObject.GetComponent<ObjectManager>();
        button.OnClicked.AddListener(modelExpander.ExplodeModel);
    }
}
