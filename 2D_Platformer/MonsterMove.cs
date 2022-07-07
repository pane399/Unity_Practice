using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer sRenderer;
    CapsuleCollider2D CapCollider;

    public int nextMove;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sRenderer = GetComponent<SpriteRenderer>();
        CapCollider = GetComponent<CapsuleCollider2D>();
        Think();
    }
    
    void FixedUpdate()
    {
        //Move
        rigid.velocity = new Vector2(nextMove * 1.5f, rigid.velocity.y);

        //Platform Check
        float nextThinkTime = Random.Range(2f, 5f);
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            nextMove *= -1;
            sRenderer.flipX = nextMove == 1;
            CancelInvoke();
            Invoke("Think", nextThinkTime);
        }
    }

    // 재귀 함수
    void Think()
    {
        //Set Next Active
        nextMove = Random.Range(-1, 2);

        //Sprite Animation
        anim.SetInteger("WalkSpeed", nextMove);

        //Flip Sprite
        if (nextMove != 0)
        {
            sRenderer.flipX = nextMove == 1;
        }

        //Recursive
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }

    public void OnDamaged()
    {
        //Sprite Alpha
        sRenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        sRenderer.flipY = true;
        //Collider Disable
        CapCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //Destroy
        Invoke("DeActive", 2);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
