using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVariables : MonoBehaviour
{

    [SerializeField]
    float AwarenessDeclineRate;

    [SerializeField]
    float AwarnessIncreaseWhenTargetSpotted;

    float Awareness = 0;

    List<GameObject> NearbyEnemies = new List<GameObject>();

    [SerializeField]
    GameObject enemyTest;

    AIMovementHandler AIMovementHandler;

    #region MonoBehaviour Functions

    // Start is called before the first frame update
    void Start()
    {
        AIMovementHandler = GetComponent<AIMovementHandler>();
        NearbyEnemies.Add(enemyTest);
    }

    // Update is called once per frame
    void Update()
    {
        if(Awareness > 0)
        {
            Awareness = Mathf.Max(Awareness - AwarenessDeclineRate * Time.deltaTime, 0);
        }
    }

    #endregion

    #region public functions

    public float GetAwareness()
    {
        return Awareness;
    }

    public List<GameObject> GetNearbyEnemies()
    {
        return NearbyEnemies;
    }

    public Direction GetDirection()
    {
        return AIMovementHandler.GetDirection();
    }

    public void TargetSeen()
    {
        //Target seen so lets add some awarness
        Awareness += AwarnessIncreaseWhenTargetSpotted * Time.deltaTime;

    }

    #endregion

    #region Trigger Handling

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //TODO: Need to add a tag to things ai are interested about, is currently just adding anything other than itself
        if(collision.gameObject != transform.gameObject)
        {
            NearbyEnemies.Add(collision.gameObject);
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject != transform.gameObject)
        {
            NearbyEnemies.Remove(collision.gameObject);
            
        }
    }

    #endregion
}
