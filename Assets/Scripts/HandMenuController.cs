using System.Linq;
using UnityEngine;

public class HandMenuController : MonoBehaviour
{
    public enum Tag
    {
        Main,
        ThreeMenu,
        Pump,
        Coupling,
        Motor
    }

    private GameObject reconstruct;
    private GameObject home;
    private GameObject[] threeMenu;
    private GameObject[] pumpMenu;
    private GameObject listObject;
    private bool pressed = false;

    // Start is called before the first frame update
    void Start()
    {
        reconstruct = GameObject.Find("Action Button Reconstruct");
        home = GameObject.Find("Action Button Home");
        threeMenu = GameObject.FindGameObjectsWithTag("3Menu");
        pumpMenu = GameObject.FindGameObjectsWithTag("pump");
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void PumpReconstruct(GameObject child)
    {
        if (child.gameObject.tag != "pump_rotel" && child.gameObject.tag != "pump_UC" && child.gameObject.tag != "pump_NDE" && child.gameObject.tag != "pump_DE")
            child.gameObject.SetActive(false);
    }

    private void Reconstruct(string keyword, bool activate)
    {
        GameObject parentObject = GameObject.Find("MenuContent-Canvas");

        if (parentObject != null)
        {
            // Get all child objects of the parent
            Transform[] allChildren = parentObject.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in allChildren)
            {
                if (child.gameObject.name.Contains(keyword))
                {
                    child.gameObject.SetActive(activate);
                }
            }
        }
    }

    public void MenuPressed()
    {

        string[] allPump = { "Reconstruct", "Coupling", "Motor" };
        string[] allCoupling = { "Reconstruct", "Pump", "Motor" };
        string[] allMotor = { "Reconstruct", "Pump", "Coupling" };
        string[] pumps = { "Rotel", "Pump", "UC" };
        string[] motors = { "Casing", "Motor", "Rotor" };

        if (!pressed)
        {
            pressed = true;
            HandButton handButton = FindObjectOfType<HandButton>();
            handButton.position = gameObject.name;

            Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in allChildren)
            {
                if (child.gameObject.name == "UIButtonSpriteIcon")
                {
                    child.gameObject.SetActive(true);
                }
                else if (child.gameObject.name == "UIButtonFontIcon")
                {
                    child.gameObject.SetActive(false);
                }
            }

            if (gameObject.name.Contains("Reconstruct"))
            {
                Reconstruct("3main", true);
            }
            else if (gameObject.name.Contains("Rotel"))
            {
                string[] merged = pumps.Concat(allCoupling).ToArray();
                foreach (string pump in merged)
                    Reconstruct(pump, false);

                Reconstruct("Rotel", true);
            }
            else if (gameObject.name.Contains("UC"))
            {
                string[] merged = pumps.Concat(allCoupling).ToArray();
                foreach (string pump in merged)
                    Reconstruct(pump, false);
                Reconstruct("UC", true);
            }
            else if (gameObject.name.Contains("Pump NDE"))
            {
                string[] merged = pumps.Concat(allCoupling).ToArray();
                foreach (string pump in merged)
                    Reconstruct(pump, false);
                Reconstruct("Pump NDE", true);
            }
            else if (gameObject.name.Contains("Pump DE"))
            {
                string[] merged = pumps.Concat(allCoupling).ToArray();
                foreach (string pump in merged)
                    Reconstruct(pump, false);
                Reconstruct("Pump DE", true);
            }
            else if (gameObject.name.Contains("Pump"))
            {
                Reconstruct("Pump", true);
                foreach (string obj in allPump)
                    Reconstruct(obj, false);
            }
            else if (gameObject.name.Contains("Coupling"))
            {
                Reconstruct("Coupling", true);
                foreach (string obj in allCoupling)
                    Reconstruct(obj, false);
            }
            else if (gameObject.name.Contains("Motor NDE"))
            {
                string[] merged = motors.Concat(allMotor).ToArray();
                foreach (string pump in merged)
                    Reconstruct(pump, false);
                Reconstruct("Motor NDE", true);
            }
            else if (gameObject.name.Contains("Motor DE"))
            {
                string[] merged = motors.Concat(allMotor).ToArray();
                foreach (string pump in merged)
                    Reconstruct(pump, false);
                Reconstruct("Motor DE", true);
            }
            else if (gameObject.name.Contains("Casing"))
            {
                string[] merged = motors.Concat(allMotor).ToArray();
                foreach (string obj in merged)
                    Reconstruct(obj, false);
                Reconstruct("Casing", true);
            }
            else if (gameObject.name.Contains("Rotor"))
            {
                string[] merged = motors.Concat(allMotor).ToArray();
                foreach (string obj in merged)
                    Reconstruct(obj, false);
                Reconstruct("Rotor", true);
            }
            else if (gameObject.name.Contains("Motor"))
            {
                Reconstruct("Motor", true);
                Reconstruct("Stator", false);
                foreach (string obj in allMotor)
                    Reconstruct(obj, false);
            }
            else if (gameObject.name.Contains("Stator"))
            {
                Reconstruct("Stator", true);
                foreach (string obj in motors)
                    Reconstruct(obj, false);
            }

        }
        else
        {
            pressed = false;

            Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                if (child.gameObject.name == "UIButtonSpriteIcon")
                {
                    child.gameObject.SetActive(false);
                }
                else if (child.gameObject.name == "UIButtonFontIcon")
                {
                    child.gameObject.SetActive(true);
                }
            }

            if (gameObject.name.Contains("Pump 3main"))
            {
                foreach (string pump in pumps)
                    Reconstruct(pump, false);
                Reconstruct("3main", true);
            }
            else if (gameObject.name.Contains("Pump"))
            {
                Reconstruct("Pump", true);
            }
            else if (gameObject.name.Contains("Coupling"))
            {
                Reconstruct("3main", true);
                Reconstruct("Coupling", true);
            }
            else if (gameObject.name.Contains("Motor 3main"))
            {
                foreach (string motor in motors)
                    Reconstruct(motor, false);
                Reconstruct("3main", true);
            }
            else if (gameObject.name.Contains("Motor"))
            {
                Reconstruct("Motor", true);
                Reconstruct("Stator", false);
            }
            else if (gameObject.name.Contains("Stator"))
            {
                Reconstruct("Stator", true);
            }


        }
        //switch (position)
        //{
        //    case "main":
        //        this.position = position;
        //        foreach (GameObject obj in threeMenu)
        //            obj.SetActive(false);
        //        break;
        //    case "3menu":
        //        this.position = position;
        //        foreach (GameObject obj in threeMenu)
        //            obj.SetActive(true);
        //        break;
        //    case "pump":
        //        this.position = position;
        //        foreach (GameObject obj in threeMenu)
        //            obj.SetActive(false);
        //        break;
        //    case "coupling":
        //        this.position = position;
        //        foreach (GameObject obj in threeMenu)
        //            obj.SetActive(false);
        //        break;
        //    case "motor":
        //        this.position = position;
        //        foreach (GameObject obj in threeMenu)
        //            obj.SetActive(false);
        //        break;
        //    case "information":
        //        //this.position = position;
        //        list3D.SetText(List3D);
        //        break;
        //}
    }



}
