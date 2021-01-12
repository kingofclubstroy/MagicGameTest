using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RotationEvent.RegisterListener(Rotate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Rotate(RotationEvent e)
    {
        Vector3 rotation = transform.eulerAngles;

        rotation.z = e.rotation; // Standart Left-/Right Arrows and A & D Keys

        transform.eulerAngles = rotation;
        
    }
}
