using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Transform Twei1;
    public Transform Jiao;
    public Transform CheckBrick;
    
    public Transform FirstBrick;
    public Transform PlayerPosition;

    [SerializeField] private GameObject levelComplete;
    [SerializeField] private float speed = 3000f;
    public float range = 1;
    public int Score;

    public string PlayerState = "Stay";
    public string RotationState = "NotRotated";

    public GameObject PlayerBrick;
    public GameObject MapBrick;
    public GameObject player;
    public GameObject OpenedChest;
    public GameObject ClosedChest;

    private Vector2 firstTouchPosition;
    private Vector2 lastTouchPosition;

    public Vector3 Target;

    Vector3 movement;

    public TextMeshProUGUI TextScore;
    
    void Update()
    {
        RotateByMouse();
        rayCastCheck();
        //Debug.Log()
        //Debug.Log(RotationState);
    }
        
    private void rayCastCheck()
    {
        Ray ray = new Ray(CheckBrick.position, transform.TransformDirection(Vector3.down) * range);
        Debug.DrawRay(CheckBrick.position, transform.TransformDirection(Vector3.down) * range, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            Debug.Log(hit.collider.tag);
            if (
                hit.collider.tag == "MapBrick" || 
                hit.collider.tag == "PlayerBrick" || 
                hit.collider.tag == "Bridge" || 
                hit.collider.tag == "UnderMapBrick" ||
                hit.collider.tag == "Push" ||
                hit.collider.tag == "FinishLine"               
                )
            {
                Move(CheckBrick.position);
            }

            else if (hit.collider.tag == "Wall")
            {
                
                RotationState = "NotRotated";
                PlayerState = "Stay";
            }
            
            else if (hit.collider.tag == "BoxClosed")
            {
                
                OpenedChest.SetActive(true);                
                ClosedChest.SetActive(false);

                levelComplete.SetActive(true);
            }
            else
            {
                PlayerState = "Stay";
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        //an gach
        if (other.gameObject.tag == "MapBrick")
        {           
            Vector3 InitiatePosition = new Vector3(transform.position.x, -0.5f, transform.position.z);

            GameObject prefab_PlayerBrick = Instantiate(PlayerBrick, InitiatePosition, Quaternion.Euler(-90, 0, 0), transform);
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
            Score++;
            TextScore.text = "Score: "  + Score.ToString();
            
            Destroy(other.gameObject);
        }

        if (PlayerState == "Moving")
        {
            if (other.gameObject.tag == "Push")
            {
                //Debug.Log(other.transform.localEulerAngles);
                RotateByPush(other);
                //RotationState = "Rotated";    
            }
        }   
    }

    public void RotateByPush(Collider Push)
    {
        //Debug.Log(PushPrefabs.transform.localEulerAngles);
        if (Push.transform.localEulerAngles.y == 90)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Abs(transform.eulerAngles.y - Push.transform.localEulerAngles.y), transform.eulerAngles.z);
            Debug.Log("Pushed by 90");
        }

        else if (Push.transform.localEulerAngles.y == 270)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90 - transform.eulerAngles.y, transform.eulerAngles.z);
            Debug.Log("Pushed by 270");

        }

        else if (Push.transform.localEulerAngles.y == 180)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - 270, transform.eulerAngles.z);
            Debug.Log("Pushed by 180");

        }

        else if (Push.transform.localEulerAngles.y == 0)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Abs(transform.eulerAngles.y) - 90, transform.eulerAngles.z);
            Debug.Log("Pushed by 0");

        }
    }
    private void OnTriggerExit(Collider other)
    {       
        //xoa gach
        if (other.gameObject.tag == "Bridge")
        {
            other.tag = "PlayerBrick";
            Instantiate(PlayerBrick, other.transform.position, Quaternion.Euler(-90, 0, 0));
            GameObject ChildBrick = player.transform.Find("Brick(Clone)").gameObject;
            if (ChildBrick != null)
            {
                Destroy(ChildBrick);
                Jiao.transform.position = new Vector3(Jiao.transform.position.x, Jiao.transform.position.y - 0.3f, Jiao.transform.position.z);
                Twei1.transform.position = new Vector3(Twei1.transform.position.x, Twei1.transform.position.y, Twei1.transform.position.z);
            }
        }

        if (other.gameObject.tag == "FinishLine")
        {
            destroyAllPlayerBrick("Brick");
            Jiao.transform.position = new Vector3(Jiao.transform.position.x, other.transform.position.y, Jiao.transform.position.z);
            Twei1.transform.position = new Vector3(Twei1.transform.position.x, other.transform.position.y, Twei1.transform.position.z);
        }
    }

    public void destroyAllPlayerBrick(string tag)
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);
        {
            foreach(GameObject target in gameObjects)
            {
                GameObject.Destroy(target);
            }
        }            
    }
    private void Move(Vector3 Target)
    {
        
        if (PlayerState == "Stay" )
        {
            Target = new Vector3(Target.x, Target.y - 1f, Target.z);
            transform.position = Vector3.MoveTowards(transform.position, Target, speed * Time.deltaTime);
            PlayerState = "Moving";
        }

        if (PlayerState == "Moving")
        {
            Target = new Vector3(Target.x, Target.y - 1f, Target.z);
            //Debug.Log(Target);
            transform.position = Vector3.MoveTowards(transform.position, Target, speed * Time.deltaTime);
        }
    }   

    private void getFingerCoords()
    {      
            if (Input.GetMouseButtonDown(0))
            {
                firstTouchPosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                lastTouchPosition = Input.mousePosition;
            }
    }
  
    private void RotateByMouse()
    {
        if (PlayerState == "Stay")
        {
            getFingerCoords();

            if (Vector2.Distance(firstTouchPosition, Vector2.zero) < 0.1f || 
                Vector2.Distance(lastTouchPosition, Vector2.zero) < 0.1f)
            {
                return;
            }
            Debug.Log("quay tay");
            float Distance = Vector3.Distance(firstTouchPosition, lastTouchPosition);
            if (lastTouchPosition.y > firstTouchPosition.y)
            {
                if (Mathf.Abs((lastTouchPosition.x - firstTouchPosition.x) / (Distance)) < 0.70710678f)
                {
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);
                    //CurrentDirection = "Forward";
                    //Move("Forward");
                }

                else if ((lastTouchPosition.x - firstTouchPosition.x) / (Distance) >= 0.70710678f && (lastTouchPosition.x - firstTouchPosition.x) / (Distance) <= 1f)
                {
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, 270, transform.eulerAngles.z);
                    //CurrentDirection = "Right";
                    //Move("Right");
                }

                else if ((lastTouchPosition.x - firstTouchPosition.x) / (Distance) >= -1f && (lastTouchPosition.x - firstTouchPosition.x) / (Distance) <= -0.70710678f)
                {
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90, transform.eulerAngles.z);
                    //CurrentDirection = "Left";
                    //Move("Left");
                }
            }

            else
            {
                if (Mathf.Abs((lastTouchPosition.x - firstTouchPosition.x) / (Distance)) < 0.70710678f)
                {

                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
                    //CurrentDirection = "Back";
                    //Move("Back");
                }

                else if ((lastTouchPosition.x - firstTouchPosition.x) / (Distance) >= 0.70710678f && (lastTouchPosition.x - firstTouchPosition.x) / (Distance) <= 1f)
                {
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, 270, transform.eulerAngles.z);
                    //CurrentDirection = "Right";
                    //Move("Right");
                }

                else if ((lastTouchPosition.x - firstTouchPosition.x) / (Distance) >= -1f && (lastTouchPosition.x - firstTouchPosition.x) / (Distance) <= -0.70710678f)
                {
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90, transform.eulerAngles.z);
                    //CurrentDirection = "Left";
                    //Move("Left");
                }
            }
            firstTouchPosition = lastTouchPosition = Vector2.zero;
        }
    }

    private void Start()
    {
        transform.position = new Vector3(FirstBrick.position.x , FirstBrick.position.y + 3, FirstBrick.position.z);       
    }
}
