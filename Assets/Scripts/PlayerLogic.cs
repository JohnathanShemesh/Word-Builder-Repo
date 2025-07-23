using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    public static PlayerLogic Instance;


    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public int playerLives = 3;
    private Rigidbody2D rb;
    public Animator animator;
    public Transform raycastOrigin;
    public BoxCollider2D boxCollider;
    public float rayLength = 0.5f;
    public LayerMask groundLayer;
    private bool isGrounded;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    void Awake()
    {
        Debug.Log("UIManager Awake called");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
       

    }
    void Start()
    {            
        boxCollider = GetComponent<BoxCollider2D>();
        originalColliderSize = boxCollider.size;
        originalColliderOffset = boxCollider.offset;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (raycastOrigin == null)
            Debug.LogError("raycastOrigin not found!");
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        animator.SetFloat("Speed",Mathf.Abs(rb.velocity.x));
        if(rb.velocity.x < 0)
        {
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (rb.velocity.x > 0)
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }

        Vector2 boxSize = new Vector2(boxCollider.size.x * 0.9f, 0.05f);
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxSize, 0f, Vector2.down, rayLength, groundLayer);

        if (hit.collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        bool isCrouching = isGrounded && Input.GetKey(KeyCode.S);
        if (isGrounded && Input.GetKey(KeyCode.S))
        {
            Crouch(isCrouching);
            animator.SetBool("IsCrouching", true);
        }
        else 
        {
            Crouch(isCrouching);
            animator.SetBool("IsCrouching", false);
        }
    }
    
    public void Crouch(bool isCrouching)
    {
        if (isCrouching)
        {
            boxCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y / 2f);
            boxCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - originalColliderSize.y / 4f);
        }
        else
        {
            boxCollider.size = originalColliderSize;
            boxCollider.offset = originalColliderOffset;
        }
    }

    private void OnDrawGizmos()
    {
        if (boxCollider != null)
        {
            Gizmos.color = Color.red;
            Vector2 boxSize = new Vector2(boxCollider.size.x * 0.9f, 0.05f);
            Gizmos.DrawWireCube(boxCollider.bounds.center + Vector3.down * rayLength, boxSize);
        }
    }



    private List<string> currentWordLetters = new();  // List to track remaining letters

    public void InitializeWordLetters(string word)
    {
        currentWordLetters = new List<string>();
        foreach (char c in word)
        {
            currentWordLetters.Add(c.ToString().ToUpper());
        }

        Debug.Log("Initialized word letters:");
        foreach (var letter in currentWordLetters)
        {
            Debug.Log(letter);
        }
    }

    public bool CollectLetterSprite(LetterDataSO collectedLetter)
    {
        string letterName = collectedLetter.letterName.ToUpper().Trim();

        Debug.Log($"Trying to collect letter: '{letterName}'");

        if (!currentWordLetters.Contains(letterName))
        {
            Debug.LogWarning($"Letter '{letterName}' is NOT in the remaining word letters!");
            LoseLife();
            return false;
        }

        // Remove the first matching instance of the letter
        currentWordLetters.Remove(letterName);
        UIManager.Instance.RevealLetter(letterName);
        Debug.Log($"Collected letter '{letterName}'. Remaining: {currentWordLetters.Count}");

        if (currentWordLetters.Count == 0)
        {
            Debug.Log("Word completed!");
            GameManager.Instance.OnWordCompleted();
        }

        return true;
    }

    public void LoseLife()
    {
        playerLives--;
        UIManager.Instance.UpdateHearts(playerLives);
        if (playerLives <= 0)
        {
            Debug.Log("Game Over");
            UIManager.Instance.ShowGameOver();
        }
    }

    public void ResetLives()
    {
        playerLives = 3;
        UIManager.Instance.UpdateHearts(playerLives);
    }
}
