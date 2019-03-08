using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControllerInput : MonoBehaviour
{
    public Text LeftXAxisText;
    public Text LeftYAxisText;
    public Text RightXAxisText;
    public Text RightYAxisText;
    public Text DPadXText;
    public Text DPadYText;
    public Text TriggerText;

    void Start()
    {
        string[] joysticks = Input.GetJoystickNames();
        foreach (var j in joysticks)
        {
            Debug.Log(j);
        }
    }
	// Update is called once per frame
	void Update ()
    {
        float rightXAxis, rightYAxis, leftXAxis, leftYAxis, dPadX, dPadY, triggerAxis;

        rightXAxis = Input.GetAxis("RightJoyStickHorizontal");
        rightYAxis = Input.GetAxis("RightJoyStickVertical");

        leftXAxis = Input.GetAxis("LeftJoyStickHorizontal");
        leftYAxis = Input.GetAxis("LeftJoyStickVertical");

        dPadX = Input.GetAxis("DPadHorizontal");
        dPadY = Input.GetAxis("DPadVertical");

        triggerAxis = Input.GetAxis("TriggerAxis");

        RightXAxisText.text = "Right X Axis: " + rightXAxis;
        RightYAxisText.text = "Right Y Axis: " + rightYAxis;

        LeftXAxisText.text = "Left X Axis: " + leftXAxis;
        LeftYAxisText.text = "Left Y Axis: " + leftYAxis;

        DPadXText.text = "D-Pad X: " + dPadX;
        DPadYText.text = "D-Pad Y: " + dPadY;

        TriggerText.text = "Trigger: " + triggerAxis;

        if (Input.GetKeyDown(KeyCode.Joystick2Button0))
        {
            Debug.Log("A button pressed");
        }
	}
}
