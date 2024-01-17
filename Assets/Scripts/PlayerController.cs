using System.Collections;
using UnityEngine;
using TMPro;
using System.Xml.Serialization;
using System;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public bool IsDisabled { get; private set; } = false;
    public CharacterController Controller;
    public Animator Animator;
    public Transform Cam;
    public Transform GroundCheck;
    public LayerMask GroundMask;

    private LayerMask keyWorkerLayer;
    private LayerMask childLayer;

    private bool hasBeenCaught = false;
    private bool isCollisionChecked = false;

    public float GroundDistance = 0.4f;
    public float MoveSpeed = 5f;
    public float TurnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    private bool isGrounded;
    private bool isSwimming;
    private bool poolInteraction = false, picnicInteraction = false, childInteraction = false, mazeIneraction = false, playgroundInteraction = false;
    private bool pondExplored = false, picnicExplored = false, mazeExplored = false, playgroundExplored = false, explored = false;
    private bool atPicnic = false, atPlayground = false, atMaze = false;
    public bool keyStolen = false, atWorker = false, atChild = false, atDoor = false;

    private Vector3 velocity;
    public float Gravity = -9.81f;

    public TextMeshProUGUI ScoreText, TimesCaughtText;
    private int score, timesCaught;
    public TextMeshProUGUI Q1, Q2, Q3, Q4, Q5, Q6, Q7;

    void Start()
    {
        Cursor.visible = false;

        SetText();
        keyWorkerLayer = LayerMask.NameToLayer("KeyWorker");
        childLayer = LayerMask.NameToLayer("Childr");
    }

    void Update()
    {
        CheckCollisionWithWorker();

        if (!IsDisabled)
        {
            HandleMovement();
            HandleActions();
        }
        else
        {
            HandleDisabledState();
        }

        UpdateUI();
        PondInteraction();
        Explored();
        MazeInteraction();
        PicnicInteraction();
        PlaygroundInteraction();
        KeyInteration();
        ChildInteraction();
        DoorInteraction();
    }

    void SetText()
    {
        Q1.text = "Explore The Areas";
        Q2.text = "Swim(Interact) In the Pond";
        Q3.text = "Roll In The Picnic Area";
        Q4.text = "Help The Child Up";
        Q5.text = "Go to the middle of the maze";
        Q6.text = "Dig in the playground";
        Q7.text = "Steal the Key From the worker at the monkey bars";
        
    }

    void HandleMovement()
    {
        isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, TurnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            Controller.Move(moveDir.normalized * MoveSpeed * Time.deltaTime);
            Moves();
        }
        else
        {
            Animator.SetBool("isWalking", false);
        }

        velocity.y += Gravity * Time.deltaTime;
        Controller.Move(velocity * Time.deltaTime);

        
    }

    void HandleActions()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Roll();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            Dig();
        }
    }

    private void HandleDisabledState()
    {
        Teleport();
        StartCoroutine(ResetCollisionCheck());
        StartCoroutine(ResetCollisionCheckAndDisable());
    }

    void Moves()
    {
        Animator.SetBool("isWalking", true);
    }

    public void Roll()
    {
        // Implement roll functionality if needed.
        Animator.SetTrigger("RollTrigger");
    }

    public void Dig()
    {
        // Implement dig functionality if needed.
        Animator.SetTrigger("DigTrigger");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            Animator.SetBool("isSwimming", true);
            isSwimming = true;
            

        }

        if (other.CompareTag("Picnic"))
        {
            picnicExplored = true;
            Debug.Log("Picnic Area explored");
            atPicnic = true;
        }

        if (other.CompareTag("Playground"))
        {
            playgroundExplored = true;
            Debug.Log("Playground explored");
            atPlayground = true;
        }
        if (other.CompareTag("Maze"))
        {
            mazeExplored = true;
            Debug.Log("Maze explored");
        }
        if (other.CompareTag("Pond"))
        {
            pondExplored = true;
            Debug.Log("Pond explored");
        }
        if (other.CompareTag("MazeCenter"))
        {
            atMaze = true;
            Debug.Log("at maze center");
        }

        if (other.CompareTag("KeyWorker"))
        {
            atWorker = true;
            Debug.Log("at key worker");
        }
        if (other.CompareTag("Child"))
        {
            atChild = true;
            Debug.Log("at Child");
        }
        if (other.CompareTag("Door"))
        {
            atDoor = true;
            Debug.Log("at Child");
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            Animator.SetBool("isSwimming", false);
            isSwimming = false;
        }
        if (other.CompareTag("Door"))
        {
            atDoor = false;
            Debug.Log("at Child");
        }

        if (other.CompareTag("Picnic"))
        {
            atPicnic = false;
        }
        if (other.CompareTag("MazeCenter"))
        {
            atMaze = false;
            Debug.Log("at maze center");
        }
        if (other.CompareTag("Playground"))
        {
            
            atPlayground = false;
        }
        if (other.CompareTag("KeyWorker"))
        {
            atWorker = false;
            
        }
        if (other.CompareTag("Child"))
        {
            atChild = false;
            
        }
    }

    private void DoorInteraction()
    {
        if(atDoor&& keyStolen)
        {
            SceneManager.LoadScene(2);
        }
    }

    private void PondInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E) && isSwimming && poolInteraction == false)
        {
            score++;
            poolInteraction = true;
            Q2.text = "<s>Swim(Interact) In the Pond</s>";
        }
    }

    private void KeyInteration()
    {
        if (Input.GetKeyDown(KeyCode.E) && atWorker && keyStolen == false)
        {
            score++;
            keyStolen = true;
            Q1.text = "Escape";
            Q2.text = "";
            Q3.text = "";
            Q4.text = "";
            Q5.text = "";
            Q6.text = "";
            Q7.text = "";

            Debug.Log("Key Stolen");
        }
    }

    private void ChildInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E) && atChild && childInteraction == false)
        {
            score++;
            childInteraction = true;
            Q4.text = "<s>Help The Child Up</s>";
        }
    }
    private void PicnicInteraction()
    {
        if(Input.GetKeyDown(KeyCode.Space) && picnicInteraction == false && atPicnic)
        {
            score++;
            picnicInteraction = true;
            Q3.text = "<s>Roll In The Picnic Area</s>";
        }
    }

    

    private void PlaygroundInteraction()
    {
        if(Input.GetKeyDown(KeyCode.Q) && playgroundInteraction == false && atPlayground)
        {
            score++;
            playgroundInteraction = true;
            Q6.text = "<s>Dig in the playground</s>";
        }
    }

    private void MazeInteraction()
    {
        if(mazeIneraction == false && atMaze)
        {
            score++;
            mazeIneraction = true;
            Q5.text = "<s>Go to the middle of the maze</s>";
        }
    }

    

    private void Explored()
    {
        if (picnicExplored && playgroundExplored && mazeExplored && pondExplored && explored==false)
        {
            score++;
            explored = true;
            Q1.text = "<s>Explore The Areas</s>";
            Debug.Log("Areas Explored");
        }
    }

    

    private void CheckCollisionWithWorker()
    {
        if (!hasBeenCaught && !isCollisionChecked)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.6f, LayerMask.GetMask("Worker"));

            foreach (var collider in hitColliders)
            {
                IsDisabled = true;
                isCollisionChecked = true;
                hasBeenCaught = true;
                StartCoroutine(ResetCollisionCheck());
                timesCaught++;
            }
        }
    }

    private IEnumerator ResetCollisionCheck()
    {
        yield return new WaitForSeconds(1f);  // Adjust the delay as needed
        isCollisionChecked = false;
        hasBeenCaught = false;  // Reset the flag after the delay
    }

    private IEnumerator ResetCollisionCheckAndDisable()
    {
        yield return new WaitForSeconds(1f);  // Adjust the delay as needed
        StartCoroutine(ResetCollisionCheck());
        IsDisabled = false;
        
    }

    private void Teleport()
    {
        transform.position = new Vector3(-11f, 0.88f, 74f);
        UnityEngine.Debug.Log("Hit");
    }

    public void Interact()
    {
        // Implement interaction logic if needed.
    }

    void UpdateUI()
    {
        if (ScoreText != null)
        {
            ScoreText.text = "Score: " + score.ToString();
        }
        if (TimesCaughtText != null)
        {
            TimesCaughtText.text = "Times Caught: " + timesCaught.ToString();
        }
    }
}
