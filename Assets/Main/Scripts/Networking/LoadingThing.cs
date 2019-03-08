using UnityEngine;
using System.Collections;

public class LoadingThing : MonoBehaviour
{
    public float RotationSpeed;
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(0, 0, RotationSpeed * Time.unscaledDeltaTime);
	}
}
