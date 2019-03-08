using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NamePassViewController : MonoBehaviour
{
    public InputField NameInput;
    public InputField PassInput;

    public event System.Action<string, string> SubmitPressed;

    private bool privateRoom = false;

    void Awake()
    {

    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            Submit();
        }
    }

    public void PrivateRoomToggled(bool val)
    {
        privateRoom = val;
        GetComponent<Animator>().SetBool("Open", val);
    }

    public void Submit()
    {
        // Ignore if off
        if (GetComponent<CanvasGroup>().alpha == 0)
        {
            return;
        }

        string name = NameInput.text;
        string pass = "";
        if (privateRoom)
        {
            pass = PassInput.text;
        }

        if (SubmitPressed != null)
        {
            SubmitPressed(name, pass);
        }
    }
}
