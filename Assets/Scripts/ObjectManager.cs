using MixedReality.Toolkit.SpatialManipulation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectManager : MonoBehaviour
{
	private AnimationStateController animationState;

	public Animator anim;

	private bool isExpanded = false;
	private TMP_Text list3D;
	UnityAction<SelectExitEventArgs> m_MyFirstAction;

	void Start()
	{
	}

	private void Update()
	{
	}


	public void ExplodeModel()
	{
		if (!isExpanded)
		{

			DestroyInteraction(transform.gameObject);
			StartCoroutine(WaitUntilOpen());

			isExpanded = true;
			anim.SetInteger("condition", 1);
		}
		else
		{
			ResetModel();


		}
	}

	public void ResetModel()
	{
		DestroyInteractionChildren();
		AddInteraction(transform.gameObject);

		isExpanded = false;
		anim.SetInteger("condition", 2);
		anim.enabled = true;
		StartCoroutine(WaitForAnim());
	}

	public IEnumerator WaitForAnim()
	{
		while (!anim.GetCurrentAnimatorStateInfo(0).IsName("close"))
		{
			yield return null;
		}
		//Now, Wait until the current state is done playing
		while ((anim.GetCurrentAnimatorStateInfo(0).normalizedTime) < 1.0f)
		{
			yield return null;
		}

		anim.Rebind();
	}

	public IEnumerator WaitUntilOpen()
	{
		while (anim.enabled)
		{
			yield return null;
		}
		//Now, Wait until the current state is done playing
		while ((anim.GetCurrentAnimatorStateInfo(0).normalizedTime) < 1.0f &&
		   !anim.IsInTransition(0))
		{
			yield return null;
		}

		AddInteractionChildren();
	}

	public void GetChildrenName(GameObject gameObject)
	{
		gameObject.SetActive(true);
		gameObject.GetComponent<Follow>().enabled = true;
		GameObject textObject = GameObject.Find("DetailText");
		list3D = textObject.GetComponent<TMP_Text>();
		string[] list3DArray = new string[transform.childCount];
		int i = 0;
		foreach (Transform child in transform)
		{
			list3DArray[i++] = child.name;
		}

		string[] filteredArray = list3DArray.Where(s => !s.Contains("BoundingBox") && !s.Contains("Canvas")).ToArray();
		string[] result = RemoveDuplicates(filteredArray);
		list3D.SetText(string.Join(", ", result));
	}

	public static string[] RemoveDuplicates(string[] array)
	{
		// A HashSet to keep track of seen string prefixes (first 3 characters)
		HashSet<string> seenPrefixes = new HashSet<string>();
		List<string> resultList = new List<string>();

		foreach (string str in array)
		{
			// Take the first 3 characters (or fewer if the string is shorter than 3)
			string prefix = str.Substring(0, Math.Min(3, str.Length));

			// Check if the prefix was already seen
			if (!seenPrefixes.Contains(prefix))
			{
				seenPrefixes.Add(prefix);
				resultList.Add(str);
			}
		}

		return resultList.ToArray();
	}

	private void AddInteractionChildren()
	{
		foreach (Transform child in transform)
		{
			GameObject gameObject = child.gameObject;
			AddInteraction(gameObject);
		}
	}
	private void AddInteraction(GameObject gameObject)
	{
		if (gameObject.name != "Canvas")
		{
			BoxCollider boxCol = gameObject.GetComponent<BoxCollider>();
			if (boxCol != null)
			{
				boxCol.enabled = true;
			}
			else
			{
				BoxCollider boxCol2 = gameObject.AddComponent<BoxCollider>();
			}

			ObjectPlaceController rememberPlace = gameObject.AddComponent<ObjectPlaceController>();
			ObjectManipulator objectManipulator = gameObject.AddComponent<ObjectManipulator>();
			objectManipulator.selectMode = InteractableSelectMode.Multiple;
			objectManipulator.selectExited.AddListener(rememberPlace.CheckPosition);

		}
	}

	private void DestroyInteraction(GameObject gameObject)
	{
		BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
		if (boxCollider != null)
			boxCollider.GetComponent<BoxCollider>().enabled = false;
		BoundsControl boundsControl = gameObject.GetComponent<BoundsControl>();
		if (boundsControl != null)
			Destroy(boundsControl);
		ObjectManipulator objectManipulator = gameObject.GetComponent<ObjectManipulator>();
		if (objectManipulator != null)
			Destroy(objectManipulator);
		ObjectPlaceController rememberPlace = gameObject.GetComponent<ObjectPlaceController>();
		if (rememberPlace != null)
			Destroy(rememberPlace);
		Transform[] allChildren = transform.GetComponentsInChildren<Transform>(true);
		foreach (Transform child in allChildren)
		{
			if (child.name == "Canvas")
			{
				Destroy(child.gameObject);
			}
		}
	}
	private void DestroyInteractionChildren()
	{
		foreach (Transform child in transform)
		{
			GameObject gameObject = child.gameObject;
			DestroyInteraction(gameObject);
		}
	}

}

//class Model
//{
//    private string position;
//    private int amount;

//    public Model(string position)
//    {
//        Position = position;
//    }

//    public string Position
//    {
//        get { return position; }
//        set
//        {
//            switch (value)
//            {
//                case "main":
//                    amount = 3;
//                    break;
//                case "motor":
//                    amount = 2;
//                    break;
//                case "pump":
//                    amount = 4;
//                    break;
//                default:
//                    amount = 1;
//                    break;
//            }

//        }
//    }
//}