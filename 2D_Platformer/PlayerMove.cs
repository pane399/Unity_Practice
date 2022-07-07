using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    public GameManager gameManager;
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer sRenderer;
    Animator anim;
    CapsuleCollider2D CapCollider;
    AudioSource audioSource;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        CapCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }
    
    void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }
        audioSource.Play();
    }

    void Update()
    {
        //Jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            PlaySound("JUMP");
        }

        //Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.1f, rigid.velocity.y);
        }

        //Direction Sprite
        if (Input.GetButton("Horizontal"))
        {
            sRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        //Animation isWalking
        if (Mathf.Abs(rigid.velocity.x) < 0.1)
        {
            anim.SetBool("isWalking", false);
        }
        else
            anim.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
        //Move Speed
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //Max Speed
        if (rigid.velocity.x > maxSpeed) //Right Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < maxSpeed * (-1)) //Left Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        // Landing Platform
        if (rigid.velocity.y < 0)
        {
            //Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance <= 0.5f)
                    anim.SetBool("isJumping", false);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            //Attack
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
                PlaySound("ATTACK");
            }
            //Damaged
            else
            {
                OnDamaged(collision.transform.position);
                PlaySound("DAMAGED");
            }
        }
        if (collision.gameObject.tag == "Spike")
        {
            OnDamaged(collision.transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            //Point
            bool isBronze = collision.gameObject.name.Contains("Coin_B");
            bool isSilver = collision.gameObject.name.Contains("Coin_S");
            bool isGold = collision.gameObject.name.Contains("Coin_G");

            if (isBronze)
                gameManager.stagePoint += 50;
            else if (isSilver)
                gameManager.stagePoint += 100;
            else if (isGold)
                gameManager.stagePoint += 200;

            //Deactive Item
            collision.gameObject.SetActive(false);

            PlaySound("ITEM");
        }
        else if (collision.gameObject.tag == "Finish")
        {
            PlaySound("FINISH");
            //Next Stage
            gameManager.NextStage();
        }
    }

    void OnAttack(Transform enemy)
    {
        //Point
        gameManager.stagePoint += 100;

        //Reaction Force
        rigid.AddForce(Vector2.up * 9, ForceMode2D.Impulse);

        //Enemy die
        MonsterMove monsterMove = enemy.GetComponent<MonsterMove>();
        monsterMove.OnDamaged();
    }

    void OnDamaged(Vector2 targetPos)
    {
        //Life Point Down
        gameManager.lifePointDown();

        //Change Layer (Immortal Active)
        gameObject.layer = 11;

        //View Alpha
        sRenderer.color = new Color(1, 1, 1, 0.4f);

        //Reaction Force
        int dirc = (transform.position.x - targetPos.x) > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        //Animation
        anim.SetTrigger("doDamaged");

        Invoke("OffDamaged", 1.5f);
    }

    void OffDamaged()
    {
        gameObject.layer = 10;
        sRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        //Sprite Alpha
        sRenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        sRenderer.flipY = true;
        //Collider Disable
        CapCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        PlaySound("DIE");
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

}
