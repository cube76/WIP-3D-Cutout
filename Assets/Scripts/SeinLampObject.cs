using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeinLampObject : MonoBehaviour
{
    private float blinkDuration = 0.5f;
    private bool blink = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Blink(string name)
    {
        if (!blink)
        {
            blink = true;
            GameObject[] allDescendants = GetAllDescendants(GameObject.Find(name));
            if (allDescendants.Length > 0)
            {
                foreach (var descendant in allDescendants)
                {
                    var renderer = descendant?.GetComponent<MeshRenderer>();
                    if (renderer != null && renderer.materials.Length > 0)
                    {

                        if (renderer != null && renderer.materials.Length > 0)
                        {
                            StartCoroutine(Blink(renderer));
                        }
                    }
                }
            }
            else
            {
                Debug.Log("No descendants found.");
            }
        }
        else
        {
            blink = false;
        }
    }

    IEnumerator Blink(MeshRenderer renderer)
    {
        Color originalColor = renderer.materials[^1].color;
        while (blink) // Loop indefinitely
        {
            // Set to first color
            originalColor = renderer.materials[^1].color;
            renderer.materials[^1].color = new Color(0f, 204f / 255f, 102f / 255f);
            yield return new WaitForSeconds(blinkDuration); // Wait for the specified duration

            // Set to second color
            renderer.materials[^1].color = originalColor;
            yield return new WaitForSeconds(blinkDuration); // Wait for the specified duration
        }
    }

    GameObject[] GetAllDescendants(GameObject parent)
    {
        List<GameObject> descendantsList = new List<GameObject>();
        GetChildrenRecursive(parent.transform, descendantsList);
        return descendantsList.ToArray();
    }

    void GetChildrenRecursive(Transform parentTransform, List<GameObject> list)
    {
        // Loop through all children of the current parent transform
        foreach (Transform child in parentTransform)
        {
            list.Add(child.gameObject); // Add the child to the list
            GetChildrenRecursive(child, list); // Recursively get the child's children
        }
    }

    // Update is called once per frame
    void Update()
    {

    }



}
