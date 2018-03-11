using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mario : MonoBehaviour {
    public float maxVel = 5f; //Max Speed when walking
    public float yJumpForce = 300f; //JumForce given when jumping

    private bool isjumpling = false; 
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 jumpforce;
    private bool movingRight = true;

    //public GameObject enemigo;
    public float health = 300f; //Health of our player
    public static float scoreMario = 0f; //Score of the player

    public AudioClip coin; //Music when colliding when coins
    public AudioClip jump; //Music when jumping
    

    private int coinCount;//Coincount so when we reach it, the game ends
  

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        jumpforce = new Vector2(0, 0);


    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //Update horizontal speed
        float v = Input.GetAxis("Horizontal");
        Vector2 vel = new Vector2(0, rb.velocity.y);

        v *= maxVel;

        vel.x = v;

        rb.velocity = vel;

        //Change to walking animation when needed
        if (v != 0)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

        //If player press jump
        if (Input.GetAxis("Jump") > 0.01f)
        {
            if (!isjumpling)
            {
                if (rb.velocity.y == 0)
                {
                    //if player jumped, we play animation for jumping
                    anim.SetBool("isJumping",true); //Change animation when jumping
                    
                    isjumpling = true;
                    //This 0 value is for the player to only go upside down
                    jumpforce.x = 0f;
                    //This will be a variable, the force in the jump will take it
                    jumpforce.y = yJumpForce;
                    //The rigidBody of the player object will take this force for moving
                    rb.AddForce(jumpforce);
                    //sound played when jumping
                    AudioSource.PlayClipAtPoint(jump, transform.position);
                }
            }
        }
        else
        {
            isjumpling = false;
        }
        if (movingRight && v < 0)
        {
            movingRight = false;
            Flip();
        }
        else if (!movingRight && v > 0)
        {
            movingRight = true;
            Flip();
        }

    }
    //method for flipping the sprite when going to opposite direction
    private void Flip()
    {
        var s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        //Object for eliminating the player 
        Shredder eliminar = collider.gameObject.GetComponent<Shredder>();
        //Object for coins in the game
        Coin moneda = collider.gameObject.GetComponent<Coin>();

        //If player collides with a Shredder trigger
        if (eliminar)
        {
            //The scene now is Lose
            LevelManager levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
            levelManager.LoadLevel("Lose");
        }
        //If the player collides with a coin Trigger
        else if (moneda)
        {
            //We increase the score by the value of the coin.points
            scoreMario += moneda.getPoints();
            //Plays the coin sound
            AudioSource.PlayClipAtPoint(coin, transform.position);
            coinCount++;
            //Destroys object so the player cannot see it
            Destroy(collider.gameObject);
           
            Debug.Log(scoreMario);
            //When the coinCount reaches the number of coins in the scene, the player wins
            if (coinCount == 3)
            {
                LevelManager levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
                levelManager.LoadLevel("Win");
            }
        }
    }


    
    void OnCollisionEnter2D(Collision2D collider)
    {
        //We find any enemies in the scene
        Cactus enemy = collider.gameObject.GetComponent<Cactus>();
        //If the player collides with an enemy, the enemy reduces player's health by its own damage
        //If player's health is 0, the player dies
        if (enemy)
        {
            health -= enemy.getDamage();
            if (health <= 0)
            {
                Die();
            }
        } else
        {
            //Debug.Log("Floor");
            //When player collides with floor, the jump animation stops
            //This is because I dont know how to stop that animation
            anim.SetBool("isJumping", false);
        }
    }
    /*
    void OnCollisionEnter2D(Collision2D collider)
    {
        
            //Debug.Log("Floor");
            //When player collides with floor, the jump animation stops
            //This is because I dont know how to stop that animation
            anim.SetBool("isJumping", false);
    }
    */
        //Function for killing the player
        void Die()
    {
        LevelManager levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        levelManager.LoadLevel("Lose");
        Destroy(gameObject);
    }

    
}
