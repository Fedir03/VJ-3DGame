using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

enum PlayerState { STOP, MOVE, STUCK, ATTACK };

enum Direction { UP = 0, RIGHT, DOWN, LEFT };

public class MovePlayer : MonoBehaviour
{
    public float speed = 1.0f;              
    public float heightJump = 0.5f;         
    public AudioClip jumpSound, pushSound, attackSound;  
    public float tileSize = 2.0f;

    public float stuckDuration = 1.0f;      
    private float stuckTimer = 0.0f;        

    public float attackDuration = 0.5f;     
    private float attackTimer = 0.0f;       

    PlayerState state;               
    Direction dir;                   
    Vector3 initialPosMove, vecMove; 
    float timeInMove;                

    bool bMoveBox;          
    GameObject box;         
    Vector3 initialPosBox;  

    void Start()
    {
        state = PlayerState.STOP;
        dir = Direction.DOWN;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene("SampleScene");

        if (state == PlayerState.STUCK)
        {
            stuckTimer -= Time.deltaTime;
            if (stuckTimer <= 0f)
            {
                state = PlayerState.STOP;
            }
            return; 
        }

        if (state == PlayerState.ATTACK)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                state = PlayerState.STOP;
            }
            return;
        }

        if (state == PlayerState.STOP)
        {
            bool bMove = false;
            Direction dirMove = Direction.DOWN;
            if(Input.GetKey(KeyCode.UpArrow))
            {
                bMove = true;
                dirMove = Direction.RIGHT;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                bMove = true;
                dirMove = Direction.DOWN;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                bMove = true;
                dirMove = Direction.LEFT;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                bMove = true;
                dirMove = Direction.UP;
            }
            if (bMove)
                PrepareMovement(dirMove);
        }
        else if (state == PlayerState.MOVE)
        {
            UpdateMovement();
        }
    }

    private bool PrepareMovement(Direction dirMove)
    {
        bool bMove = true;
        
        float angleMove = Mathf.PI * (int)dirMove / 2.0f;

        initialPosMove = transform.localPosition;
        vecMove = new Vector3(Mathf.Sin(angleMove), 0.0f, Mathf.Cos(angleMove)) * tileSize;

        GameObject enemyObj = GetObjectInDirection("Enemy", initialPosMove, vecMove.normalized, 0.0f, tileSize);
        if (enemyObj != null)
        {
            Destroy(enemyObj);
            
            state = PlayerState.ATTACK;
            attackTimer = attackDuration;
            
            transform.Rotate(0.0f, 90.0f * ((int)dirMove - (int)dir), 0.0f);
            dir = dirMove;

            if (attackSound != null) AudioSource.PlayClipAtPoint(attackSound, Camera.main.transform.position);

            return true;
        }

        GameObject obj = GetObjectInDirection("Wall", initialPosMove, vecMove.normalized, 0.0f, tileSize);
        if ((obj != null) && (obj.tag == "Wall"))
            bMove = false;
        else
        {
            obj = GetObjectInDirection("Box", initialPosMove, vecMove.normalized, 0.0f, tileSize);
            if ((obj != null) && (obj.tag == "Box"))
            {
                box = obj.transform.parent.gameObject;
                obj = GetObjectInDirection(null, initialPosMove, vecMove.normalized, tileSize, tileSize * 2.0f);
                if ((obj == null) || ((obj.tag != "Wall") && (obj.tag != "Box")))
                {
                    bMoveBox = true;
                    initialPosBox = box.transform.position;
                }
                else
                    bMove = false;
            }
        }
        
        if (bMove)
        {
            state = PlayerState.MOVE;
            timeInMove = 0;
            transform.Rotate(0.0f, 90.0f * ((int)dirMove - (int)dir), 0.0f);
            dir = dirMove;

            if (jumpSound != null) AudioSource.PlayClipAtPoint(jumpSound, Camera.main.transform.position);
            if (bMoveBox && pushSound != null) AudioSource.PlayClipAtPoint(pushSound, Camera.main.transform.position);
        }

        return bMove;
    }

    private GameObject GetObjectInDirection(string tag, Vector3 P, Vector3 v, float min, float max)
    {
        float closestDistance = max + 1.0f;
        GameObject obj = null;

        RaycastHit[] hits = Physics.RaycastAll(P, v, max);
        foreach (RaycastHit hit in hits)
        {
            if((hit.distance > min) && (hit.distance < max) && ((tag == null) || (hit.collider.gameObject.tag == tag)))
                if(hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                    obj = hit.collider.gameObject;
                }
        }

        return obj;
    }

    private void UpdateMovement()
    {
        timeInMove += Time.deltaTime;
        if (timeInMove > 1.0f / speed)
        {
            transform.localPosition = initialPosMove + vecMove;
            
            if (bMoveBox)
            {
                bMoveBox = false;
                box.transform.localPosition = initialPosBox + vecMove;
            }

            bool landedOnSlime = false;
            Vector3 rayOriginDown = transform.position + Vector3.up * 0.5f;
            RaycastHit[] floorHits = Physics.RaycastAll(rayOriginDown, Vector3.down, 1.0f);
            
            foreach (RaycastHit hit in floorHits)
            {
                if (hit.collider.CompareTag("SlimeTrail"))
                {
                    Destroy(hit.collider.gameObject);
                    landedOnSlime = true;
                    break; 
                }
            }

            if (landedOnSlime)
            {
                state = PlayerState.STUCK;
                stuckTimer = stuckDuration;
            }
            else
            {
                state = PlayerState.STOP;
            }
        }
        else
        {
            Vector3 jumpMove = heightJump * Mathf.Sin(speed * timeInMove * Mathf.PI) * Vector3.up;
            transform.localPosition = initialPosMove + speed * timeInMove * vecMove + jumpMove;
            if (bMoveBox)
                box.transform.localPosition = initialPosBox + speed * timeInMove * vecMove;
        }
    }
}