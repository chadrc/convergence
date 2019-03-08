using UnityEngine;
using System.Collections;

public class TowerRangeRotate : MonoBehaviour
{
    public float RotationSpeed;
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(new Vector3(0, 0, RotationSpeed));
	}
}
