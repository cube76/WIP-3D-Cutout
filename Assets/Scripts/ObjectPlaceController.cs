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
	private Camera mainCamera;
	private Transform parentTransform;
	private BoxCollider box;

	void Start()
	{

		position = this.gameObject.transform.position;
		rotation = this.gameObject.transform.rotation;
		localScale = this.gameObject.transform.localScale;

		TextPerObject();
		mainCamera = Camera.main;
		parentTransform = canvasObject.transform.parent;
		box = parentTransform.GetComponent<BoxCollider>();

		canvasObject.SetActive(false);
	}


	void Update()
	{
		// World to local camera direction
		Vector3 toCameraWorld = mainCamera.transform.position - parentTransform.position;
		Vector3 toCameraLocal = parentTransform.InverseTransformDirection(toCameraWorld);

		// Get the dominant axis (X, Y, or Z)
		float absX = Mathf.Abs(toCameraLocal.x);
		float absY = Mathf.Abs(toCameraLocal.y);
		float absZ = Mathf.Abs(toCameraLocal.z);

		Vector3 offset = Vector3.zero;

		if (absY > absX && absY > absZ)
		{
			// Top or Bottom
			float direction = Mathf.Sign(toCameraLocal.y);
			offset = new Vector3(0, direction * box.size.y / 2f, 0);
		}
		else if (absX > absZ)
		{
			// Left or Right
			float direction = Mathf.Sign(toCameraLocal.x);
			offset = new Vector3(direction * box.size.x / 2f, 0, 0);
		}
		else
		{
			// Front or Back
			float direction = Mathf.Sign(toCameraLocal.z);
			offset = new Vector3(0, 0, direction * box.size.z / 2f);
		}

		// Final local position based on BoxCollider center + offset
		canvasObject.transform.localPosition = box.center + offset;

		// Rotate canvas to face camera
		Vector3 dirToCam = mainCamera.transform.position - canvasObject.transform.position;
		canvasObject.transform.rotation = Quaternion.LookRotation(dirToCam);
		canvasObject.transform.Rotate(0, 180f, 0);

		Vector3 rot = canvasObject.transform.rotation.eulerAngles;
		//canvasObject.transform.rotation = Quaternion.Euler(rot);
		canvasObject.transform.position += canvasObject.transform.forward * -0.1f;
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
		textMeshPro.color = new Color32(255, 255, 255, 255);

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

		box = gameObject.GetComponentInParent<BoxCollider>();
		//float posz = -(box.size.z / 2);
		canvasObject.transform.localPosition = new Vector3(box.center.x, box.size.y / 2, box.center.z);
		canvasObject.transform.localRotation = Quaternion.Euler(new Vector3(90f, 180f, 0f));
	}

	public void TextCameraOverlay()
	{
		canvasObject = GameObject.Find("CanvasText");
		GameObject textObject = GameObject.Find("ObjectNameText");
		textMeshPro = textObject.GetComponent<TextMeshProUGUI>();
		textMeshPro.text = gameObject.name;
	}
}
