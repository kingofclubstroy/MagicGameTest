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

    [SerializeField]
    int MaxHealth;


    public int CurrentHealth;
   

    #endregion

    #region MonoBehaviour Callbacks

    // Start is called before the first frame update
    void Start()
    {
        initializeCallbacks();
        rigidbody = GetComponent<Rigidbody2D>();
        CurrentHealth = MaxHealth;
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
            Debug.Log("update loop stooped cause attacting");
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
                    if (currentDirection != Direction.NORTH || IsIdle)
                    {
                        currentDirection = Direction.NORTH;
                        Animate.ChangeAnimationState("Walk", animator, currentDirection);
                    }
                }
                else
                {
                    if (currentDirection != Direction.SOUTH || IsIdle)
                    {
                        currentDirection = Direction.SOUTH;
                        Animate.ChangeAnimationState("Walk", animator, currentDirection);
                    }
                }

                IsIdle = false;
            }
            else if (Horizontal != 0)
            {

                if (Horizontal > 0)
                {

                    if (currentDirection != Direction.EAST || IsIdle)
                    {
                        currentDirection = Direction.EAST;
                        Animate.ChangeAnimationState("Walk", animator, currentDirection);
                    }
                    //Flip();
                }
                else if (Horizontal < 0)
                {
                    if (currentDirection != Direction.WEST || IsIdle)
                    {
                        currentDirection = Direction.WEST;
                        Animate.ChangeAnimationState("Walk", animator, currentDirection);
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

    #endregion

    #region functions

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
        Debug.Log("Stoped attacking");
        Animate.ChangeAnimationState("Idle", animator, currentDirection);
        IsIdle = true;
        IsAttacking = false;

    }

    void PrepareAttackHitboxes(Direction direction)
    {
        if (direction == Direction.SOUTH)
        {
            GetComponent<HitBoxController>().SetNewAnimation("SouthAttack");
        }
        else if (direction == Direction.NORTH)
        {
            GetComponent<HitBoxController>().SetNewAnimation("NorthAttack");
        }
        else if (direction == Direction.WEST)
        {
            GetComponent<HitBoxController>().SetNewAnimation("WestAttack");
        }
        else if (direction == Direction.EAST)
        {
            GetComponent<HitBoxController>().SetNewAnimation("EastAttack");
        }
    }

    public void SetIdle()
    {
        Debug.Log("Setting idle");
        IsIdle = true;
        Animate.ChangeAnimationState("Idle", animator, currentDirection);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.GetType() == typeof(PolygonCollider2D))
        {
            //We are hit by an attack!!!
            Debug.Log("character got hit by an attack!!!!");
            Animate.ChangeAnimationState("Hit", animator, currentDirection);

            TakeDamage(collision.gameObject.GetComponent<AIVariables>().AttackDamage);

        }
    }

    void TakeDamage(int damageAmount)
    {
        CurrentHealth -= damageAmount;

        if(CurrentHealth <= 0)
        {
            //I am ded...
            Destroy(gameObject);
        }
    }

    void HealHealth(int HealAmount)
    {
        CurrentHealth += HealAmount;

        if(CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
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
