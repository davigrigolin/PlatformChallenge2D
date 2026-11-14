using UnityEngine;

/// <summary>
/// Controle simples para testar a plataforma: movimentação lateral e pulo.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController2D : MonoBehaviour
{
    [SerializeField, Min(0f)] private float moveSpeed = 5f;
    [SerializeField, Min(0f)] private float jumpImpulse = 10.2f;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private bool isGrounded;
    private bool jumpRequested;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.UpArrow))
        {
            jumpRequested = true;
        }
    }

    private void FixedUpdate()
    {
        float horizontal = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            horizontal -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            horizontal += 1f;

        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

        if (jumpRequested && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
        }

        jumpRequested = false;
        isGrounded = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Considera o jogador apoiado quando há um contato abaixo do centro do collider.
        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint2D contact = collision.GetContact(i);
            bool contactIsBelowPlayer = contact.point.y < boxCollider.bounds.center.y;
            bool contactIsFloorLike = Mathf.Abs(contact.normal.y) > 0.5f;

            if (contactIsBelowPlayer && contactIsFloorLike)
            {
                isGrounded = true;
                break;
            }
        }
    }
}
