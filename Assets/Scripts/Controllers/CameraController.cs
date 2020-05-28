using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField]
    private GameObject player;        //Public variable to store a reference to the player game object

    [SerializeField]
    private Vector3 offset;            //Private variable to store the offset distance between the player and camera

    GameObject currentObject;

    [SerializeField]
    float cameraSpeed = 1f;

    // Use this for initialization
    void Start()
    {

        currentObject = player;
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - currentObject.transform.position;

        // Warning, look at "Callback Functions region if confused
        initializeCallbackFunctions();

    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        if (currentObject != null)
        {
            // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
            transform.position = Vector3.Lerp(transform.position, currentObject.transform.position, Time.deltaTime * cameraSpeed); //(playerTransform - transform.position) * 0.5f; //new Vector3(playerTransform.x,  playerTransform.y, transform.position.z);
        }
    }

    // All callback functions the camera recieves
    #region Callback functions

    void initializeCallbackFunctions()
    {
        CastingProjectionDestroyedEvent.RegisterListener(castingProjectionDestroyed);
        CastingProjectionCreatedEvent.RegisterListener(castingProjectionCreated);
        CastingLocationChangedEvent.RegisterListener(castingLocationChanged);
    }

    void castingProjectionCreated(CastingProjectionCreatedEvent e)
    {
        Debug.Log("Camera Controller: castingProjectionCreated");
        currentObject = e.castingProjection;
    }

    void castingProjectionDestroyed(CastingProjectionDestroyedEvent e)
    {
        Debug.Log("Camera Controller: castingProjectionDestroyed");
        currentObject = player;
    }

    void castingLocationChanged(CastingLocationChangedEvent e)
    {
        Debug.Log("Camera Controller: castingLocationChanged");
        currentObject = e.go;
    }

    #endregion
}