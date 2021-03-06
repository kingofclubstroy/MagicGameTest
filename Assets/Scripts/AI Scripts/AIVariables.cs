﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class AIVariables : MonoBehaviour
{
    //TODO: need to refactor this out into different parts, it is currently doing too much

    [SerializeField]
    float turnStartDistance;

    public GameObject TestPrefab;

    [SerializeField]
    private ConvertedEntityHolder convertedEntityHolder;

    public bool findingPath = false;

    public bool reachedDestination = false;

    [SerializeField]
    GameObject startingNearbyObject;


    [SerializeField]
    float AwarenessDeclineRate;

    [SerializeField]
    float AwarnessIncreaseWhenTargetSpotted;

    float Awareness = 0;

    List<GameObject> NearbyEnemies = new List<GameObject>();

    [SerializeField]
    GameObject enemyTest;

    [SerializeField]
    public bool IsAggressive;

    AIMovementHandler AIMovementHandler;

    public GameObject FocusedEnemy;

    Entity entity; 
    EntityManager entityManager;

    DynamicBuffer<PathPosition> pathPositionBuffer;

    PathFollow pathFollow;

    [SerializeField]
    public float AttackRange;

    float timeSinceLastAttack = 0;

    [SerializeField]
    float AttackSpeed;

    [SerializeField]
    public int AttackDamage;

    int chunkSize;


    #region MonoBehaviour Functions

    // Start is called before the first frame update
    void Start()
    {

        chunkSize = ObstacleController.instance.ChunkSize;

        AIMovementHandler = GetComponent<AIMovementHandler>();

        entity = convertedEntityHolder.GetEntity();
        entityManager = convertedEntityHolder.GetEntityManager();

        //AddNearbyEnemy(startingNearbyObject);

        //NearbyEnemies.Add(enemyTest);
    }

    // Update is called once per frame
    void Update()
    {
        if (Awareness > 0)
        {
            Awareness = Mathf.Max(Awareness - AwarenessDeclineRate * Time.deltaTime, 0);

        }

        if (Input.GetMouseButtonDown(0))
        {
            
            SetPathfindingParams(Camera.main.ScreenToWorldPoint(Input.mousePosition));
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

    public void TargetSeen(GameObject gameObject)
    {
        //Target seen so lets add some awarness
        //Awareness += AwarnessIncreaseWhenTargetSpotted * Time.deltaTime;

        Debug.LogError("TargetSeen, setting Focused enemy");

        FocusedEnemy = gameObject;

    }

    public void MoveThisDirection(Vector2 dir)
    {
        AIMovementHandler.SetDirection(dir);
        //transform.position += (Vector3) (dir * Time.deltaTime);
    }

    public void Attack()
    {
        if(Time.time - timeSinceLastAttack >= AttackSpeed)
        {
            timeSinceLastAttack = Time.time;
            AIMovementHandler.Attack();

        }
    }

    

    #endregion

    #region Entity stuff

    public void SetPathfindingParams(Vector2 end)
    {

        int2 startPosition = ObstacleController.instance.WorldToIndex(transform.position);
        int2 endPosition = ObstacleController.instance.WorldToIndex(end);

        //Setting focused target to be null so we can look for anyone while moving
        FocusedEnemy = null;

        entityManager.AddComponentData(
            entity,
            new PathfindingParams(startPosition, endPosition)
            );

        findingPath = true;


    }

    public void MoveAlongPath()
    {
        int2 pos;
        Vector2 current;
        if (findingPath == true)
        {
            pathFollow = entityManager.GetComponentData<PathFollow>(entity);

            pathPositionBuffer = entityManager.GetBuffer<PathPosition>(entity);

           

            reachedDestination = false;

            if (pathPositionBuffer.IsCreated == false || pathFollow.NewPath == false)
            {
               
                //TODO: may want to do something here, as we are waiting currently
                return;
            }
            else
            {
                findingPath = false;
                pathFollow.NewPath = false;

                //Make sure to update the entity here, as the pathfollow is not a reference!!
                entityManager.SetComponentData(entity, pathFollow);

                if (pathPositionBuffer.Length == 0)
                {
                    Debug.LogError("path buffer has no length");
                    reachedDestination = true;
                    return;
                }


                pos = pathPositionBuffer[pathFollow.pathIndex].position;
                current = new Vector2((pos.x * chunkSize) + chunkSize/2, (pos.y * chunkSize) + chunkSize/2);

                AIMovementHandler.SetDirection(current - (Vector2)transform.position);

                return;



            }
        }

       

        if (Vector2.Distance(transform.position, AIMovementHandler.targetPosition) < turnStartDistance)
        {
            //We will reach the destanation with movement to spare, so do we just start moving to the next target? lets try that
            pathFollow.pathIndex -= 1;
            if (pathFollow.pathIndex == -1)
            {
                reachedDestination = true;
                return;
            }

            pos = pathPositionBuffer[pathFollow.pathIndex].position;
            current = new Vector2((pos.x * chunkSize) + chunkSize/2, (pos.y * chunkSize) + chunkSize/2);

            AIMovementHandler.SetNewWaypoint(current - (Vector2)transform.position);



        }

       


       

    }

    #endregion

   public void AddNearbyEnemy(GameObject enemy)
    {
        NearbyEnemies.Add(enemy);
        Debug.Log("Nearby count = " + NearbyEnemies.Count);
    }

    public void RemoveNearbyEnemy(GameObject enemy)
    {
        NearbyEnemies.Remove(enemy);

        Debug.Log("Nearby count = " + NearbyEnemies.Count);
    }

    void SmoothOutPath(DynamicBuffer<PathPosition> pathPositionBuffer)
    {
        for( int i = pathPositionBuffer.Length - 1; i >= 0;  i--)
        {



        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
       

        if(collision.GetType() == typeof(PolygonCollider2D))
        {
            //We are hit by an attack!!!
            Debug.Log("ai hit by an attack!!!!");
            Animate.ChangeAnimationState("Hit", AIMovementHandler.animator, AIMovementHandler.currentDirection);
        }
    }

    private void OnDrawGizmosSelected()
    {
        
        if(pathPositionBuffer.Length > 0)
        {
            Debug.Log("drawing gizmos, pathBuffer length = " + pathPositionBuffer.Length);

            foreach (PathPosition pos in pathPositionBuffer)
            {


                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Vector2 p = new Vector2(pos.position.x  * chunkSize + chunkSize / 2, pos.position.y * chunkSize + chunkSize / 2);

                Debug.Log(p);

                Gizmos.DrawCube(p, new Vector3(chunkSize, chunkSize, 1));

            }
        }

    }




}
