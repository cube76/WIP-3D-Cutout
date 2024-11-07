using UnityEngine;

public class SubtitlePlace : MonoBehaviour
{
    public GameObject modelContainer;
    public GameObject subtitleObject;
    // Start is called before the first frame update
    void Start()
    {
        subtitleObject.transform.position = modelContainer.transform.position;
        subtitleObject.transform.position = new Vector3(modelContainer.transform.position.x, modelContainer.transform.position.y + modelContainer.transform.localScale.y, modelContainer.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetSubtitle()
    {
        subtitleObject.transform.position = modelContainer.transform.position;
        subtitleObject.transform.position = new Vector3(modelContainer.transform.position.x, modelContainer.transform.position.y + modelContainer.transform.localScale.y, modelContainer.transform.position.z);
    }
}
