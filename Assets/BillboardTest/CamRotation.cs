using UnityEngine;

public class CamRotation : MonoBehaviour
{

    public float rotationSpeed = 10;

    void Update()
    {

        if (Input.anyKey)
        {
            Vector3 rotation = transform.eulerAngles;

            rotation.z += Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime; // Standart Left-/Right Arrows and A & D Keys

            transform.eulerAngles = rotation;

            RotationEvent e = new RotationEvent(rotation.z);

            e.FireEvent();

        }
    }
}