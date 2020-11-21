using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{

    #region Private Fields
    [Tooltip("Character's speed, used to move around the map")]
    [SerializeField]
    float speed;

    [SerializeField]
    CrawlController crawlController;

    Crawl runningCrawl;
    Vector2 runningStartPosition;

    [SerializeField]
    Animator animator;

    bool facingRight = true;

    bool casting = false;

    [SerializeField]
    GameObject CastingCircleProjectionPrefab;

    GameObject CastingCircleProjection;

    bool castingLocationChanged = false;

    #endregion

    #region MonoBehaviour Callbacks

    // Start is called before the first frame update
    void Start()
    {
        initializeCallbacks();
    }

    // Update is called once per frame
    void Update()
    {

        if(casting && Input.GetKeyUp("space"))
        {
            StopCasting();
        }

        if (Input.GetKeyDown("space"))
        {

            StartCasting();

        }

        //TODO: dont want to be doing this, but i must for now, means when holding space (casting) you cant move
        if (Input.GetKey(KeyCode.Space) == false)
        {
           
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (v != 0 && h == 0)
            {
                if (v > 0)
                {
                    animator.SetBool("Walking_Up", true);
                    animator.SetBool("Walking_Down", false);
                    animator.SetBool("Walking_Left", false);
                }
                else
                {
                    animator.SetBool("Walking_Up", false);
                    animator.SetBool("Walking_Down", true);
                    animator.SetBool("Walking_Left", false);
                }
            }
            else if (h != 0)
            {
                animator.SetBool("Walking_Up", false);
                animator.SetBool("Walking_Down", false);


                if (h > 0 && !facingRight) { 
                    animator.SetBool("Walking_Left", false);
                    animator.SetBool("Walking_Right", true);
                    //Flip();
                }
                else if (h < 0 && facingRight)
                {
                    animator.SetBool("Walking_Left", true);
                    animator.SetBool("Walking_Right", false);
                    //Flip();
                }
            } else
            {
                animator.SetBool("Walking_Up", false);
                animator.SetBool("Walking_Down", false);
                animator.SetBool("Walking_Left", false);
            }

            Vector3 tempVect = new Vector3(h, v, 0);
            tempVect = tempVect.normalized * speed * Time.deltaTime;

            this.transform.position += tempVect;
        } else if (casting)
        {
            // character is currently casting
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if(h != 0 || v != 0)
            {

                if (castingLocationChanged == false)
                {
                    //We are wanting to move the casting projection, so lets see if we need to instantiate it
                    if (CastingCircleProjection == null)
                    {
                        CastingCircleProjection = Instantiate(CastingCircleProjectionPrefab, this.transform.position, Quaternion.identity);
                        CastingProjectionCreatedEvent e = new CastingProjectionCreatedEvent();
                        e.castingProjection = CastingCircleProjection;
                        e.FireEvent();

                    }

                    CastingCircleProjection.transform.position += new Vector3(h, v).normalized * speed * Time.deltaTime;
                }
            }

        }

        if(Input.GetKeyDown("f"))
        {
            if(CastingCircleProjection != null)
            {
                CastingLocationChangedEvent e = new CastingLocationChangedEvent();
                e.go = CastingCircleProjection;
                e.FireEvent();
                castingLocationChanged = true;
                Destroy(CastingCircleProjection);
                CastingCircleProjection = null;
                
                
            }
        }

        if (Input.GetKeyDown("j"))
        {
            SpellCastCall e = new SpellCastCall();
            e.FireEvent();
            //CrawlController.instance.ConsumeCrawl(transform.position, consumeAmount, consumePixelsPerFrame);
        }
            if (Input.GetKeyDown("p"))
        {
            Vector2 bottom = transform.position;
            crawlController.CreateCrawl(bottom);
        }

        if(Input.GetKeyDown("r"))
        {
            Vector2 bottom = transform.position;
            crawlController.AddFire(bottom);
            //animator.SetBool("Casting", true);

        }

        if (Input.GetKeyDown("q"))
        {

            Vector2 bottom = transform.position;

            WaterControllerScript.instance.AddWater(bottom, 100);

        }

        //if (Input.GetKeyDown("e"))
        //{

        //    if(runningCrawl == null)
        //    {
        //        runningCrawl = crawlController.CreateCrawl(transform.position);
        //        runningStartPosition = transform.position;
        //        runningStartTime = Time.time;
        //    } 

        //} else if(runningCrawl != null)
        //{

        //    if (Time.time - runningStartTime >= 3)
        //    {
        //        runningCrawl = null;

        //    }
        //    else
        //    {
        //        int startingPixel = crawlController.GetWidth()/2;

        //        int startX = startingPixel + (int)transform.position.x;
        //        int startY = startingPixel + (int)transform.position.y;


        //        runningCrawl.GrowByPosition(new Vector2(startX, startY));
        //    }
            
        //}


        void Flip()
        {
            facingRight = !facingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    void StopCasting()
    {
        animator.SetBool("Casting", false);
        casting = false;
        Destroy(CastingCircleProjection);
        CastingCircleProjection = null;
        CastingProjectionDestroyedEvent e = new CastingProjectionDestroyedEvent();
        e.FireEvent();
        castingLocationChanged = false;

        StoppedCastingEvent ev = new StoppedCastingEvent();
        ev.FireEvent();
    }

    void StartCasting()
    {
        CastingLocationChangedEvent e = new CastingLocationChangedEvent();
        e.go = gameObject;
        e.FireEvent();

        StartedCastingEvent ev = new StartedCastingEvent();
        ev.FireEvent();

        animator.SetBool("Casting", true);
        casting = true;

        animator.SetBool("Walking_Up", false);
        animator.SetBool("Walking_Down", false);
        animator.SetBool("Walking_Left", false);
        animator.SetBool("Walking_Right", false);


    }

    #endregion

    #region Callback functions

    void initializeCallbacks()
    {
        StopCastingCall.RegisterListener(StopCastingCallback);
    }

    void StopCastingCallback(StopCastingCall e)
    {
        StopCasting();
    }

    #endregion
}
