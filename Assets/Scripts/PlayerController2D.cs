using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controle do personagem de teste usando o novo Input System do Unity.
/// A/D ou setas movimentam; Espaço, W ou seta para cima pulam.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movimentação")]
    [SerializeField, Min(0f)] private float moveSpeed = 5f;
    [SerializeField, Min(0f)] private float jumpImpulse = 10.5f;

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
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        if (keyboard.spaceKey.wasPressedThisFrame ||
            keyboard.wKey.wasPressedThisFrame ||
            keyboard.upArrowKey.wasPressedThisFrame)
        {
            jumpRequested = true;
        }
    }

    private void FixedUpdate()
    {
        Keyboard keyboard = Keyboard.current;
        float horizontal = 0f;

        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                horizontal -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                horizontal += 1f;
        }

        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

        bool groundedThisStep = isGrounded;
        isGrounded = false; // OnCollisionStay2D marca novamente se ainda houver apoio.

        if (jumpRequested && groundedThisStep)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
        }

        jumpRequested = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Só considera apoiado quando há contato abaixo do centro do personagem.
        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint2D contact = collision.GetContact(i);
            bool contactIsBelowPlayer = contact.point.y < boxCollider.bounds.center.y;
            bool contactIsFloorLike = contact.normal.y > 0.5f;

            if (contactIsBelowPlayer && contactIsFloorLike)
            {
                isGrounded = true;
                return;
            }
        }
    }

}
