using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{

    //TODO: refactor this, getting too big!

    #region Private Fields
    [Tooltip("Character's speed, used to move around the map")]
    [SerializeField]
    float speed;

    Rigidbody2D rigidbody;

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

    Direction currentDirection;

    bool IsAttacking = false;

    bool IsIdle = true;

    float Horizontal;
    float Vertical;

    #endregion

    #region MonoBehaviour Callbacks

    // Start is called before the first frame update
    void Start()
    {
        initializeCallbacks();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        Horizontal = Input.GetAxisRaw("Horizontal");
        Vertical = Input.GetAxisRaw("Vertical");

        if (casting && Input.GetKeyUp("space"))
        {
            StopCasting();
        }

        if (Input.GetKeyDown("space"))
        {
            StartCasting();
        }

        if (IsAttacking)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Period))
        {
            
            IsAttacking = true;
            Animate.ChangeAnimationState("Attacking", animator, currentDirection);

            PrepareAttackHitboxes(currentDirection);

            Horizontal = 0;
            Vertical = 0;
        }

        

        

        //TODO: dont want to be doing this, but i must for now, means when holding space (casting) you cant move
        else if (Input.GetKey(KeyCode.Space) == false)
        {

           

            if (Vertical != 0 && Horizontal == 0)
            {
                if (Vertical > 0)
                {
                    if (currentDirection != Direction.UP || IsIdle)
                    {
                        currentDirection = Direction.UP;
                        Animate.ChangeAnimationState("WalkUp", animator, currentDirection);
                    }
                }
                else
                {
                    if (currentDirection != Direction.Down || IsIdle)
                    {
                        currentDirection = Direction.Down;
                        Animate.ChangeAnimationState("WalkDown", animator, currentDirection);
                    }
                }

                IsIdle = false;
            }
            else if (Horizontal != 0)
            {

                if (Horizontal > 0)
                {

                    if (currentDirection != Direction.RIGHT || IsIdle)
                    {
                        currentDirection = Direction.RIGHT;
                        Animate.ChangeAnimationState("WalkRight", animator, currentDirection);
                    }
                    //Flip();
                }
                else if (Horizontal < 0)
                {
                    if (currentDirection != Direction.LEFT || IsIdle)
                    {
                        currentDirection = Direction.LEFT;
                        Animate.ChangeAnimationState("WalkLeft", animator, currentDirection);
                    }

                    //Flip();
                }

                IsIdle = false;
            }
            else
            {
                if (IsIdle == false)
                {
                    
                    Animate.ChangeAnimationState("Idle", animator, currentDirection);
                    //currentDirection = Direction.IDLE;
                    IsIdle = true;


                }
            }

            //Vector3 tempVect = new Vector3(Horizontal, Vertical, 0);
            //tempVect = tempVect.normalized * speed * Time.deltaTime;

            //this.transform.position += tempVect;
        }
        else if (casting)
        {
            // character is currently casting
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (h != 0 || v != 0)
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

        if (Input.GetKeyDown("f"))
        {
            if (CastingCircleProjection != null)
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

        if (Input.GetKeyDown("r"))
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


        //void Flip()
        //{
        //    facingRight = !facingRight;
        //    Vector3 theScale = transform.localScale;
        //    theScale.x *= -1;
        //    transform.localScale = theScale;
        //}
    }

    private void FixedUpdate()
    {


        Vector2 velocity = new Vector2(Horizontal, Vertical).normalized * speed;

        //TODO: i dont like just setting velocity to zero when attacking, since it overrides any externally acting forces, but for now it works
        if(IsAttacking)
        {
            velocity = Vector2.zero;
        }

        rigidbody.velocity = velocity;
    }

    void StopCasting()
    {
        Animate.ChangeAnimationState("Idle", animator, currentDirection);
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

        Animate.ChangeAnimationState("Casting", animator, currentDirection);
        casting = true;

    }

    void StoppedAttactingAlert()
    {
       
        Animate.ChangeAnimationState("Idle", animator, currentDirection);
        IsIdle = true;
        IsAttacking = false;

    }

    void PrepareAttackHitboxes(Direction direction)
    {
        if (direction == Direction.Down)
        {
            GetComponent<HitBoxController>().SetNewAnimation("SouthAttack");
        }
        else if (direction == Direction.UP)
        {
            GetComponent<HitBoxController>().SetNewAnimation("NorthAttack");
        }
        else if (direction == Direction.LEFT)
        {
            GetComponent<HitBoxController>().SetNewAnimation("WestAttack");
        }
        else if (direction == Direction.RIGHT)
        {
            GetComponent<HitBoxController>().SetNewAnimation("EastAttack");
        }
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

    //void FixedUpdate()
    //{
    //    // Cast a ray straight down.
    //    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up);

    //    if (hit.collider != null)
    //    {
    //        Debug.LogError("hit something of name: " + hit.collider.name);
    //    }

    //}

}
