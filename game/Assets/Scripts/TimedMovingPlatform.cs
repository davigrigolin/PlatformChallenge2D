using System.Collections;
using UnityEngine;

/// <summary>
/// Plataforma que começa imóvel e sem gravidade. Ao receber o primeiro contato
/// físico do jogador, desloca-se horizontalmente por 5 segundos e depois cai.
/// A ativação é permanente e não pode ser reiniciada.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class TimedMovingPlatform : MonoBehaviour
{
    [Header("Movimento horizontal")]
    [SerializeField] private Vector2 movementDirection = Vector2.right;
    [SerializeField, Min(0.01f)] private float horizontalSpeed = 1.5f;
    [SerializeField, Min(0.01f)] private float movementDuration = 5f;

    [Header("Queda")]
    [SerializeField, Min(0f)] private float gravityScaleAfterMovement = 1f;

    [Header("Ativação")]
    [SerializeField] private string playerTag = "Player";

    private Rigidbody2D rb;
    private bool hasBeenActivated;

    public bool HasBeenActivated => hasBeenActivated;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Estado inicial: parada e sem influência da gravidade.
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // A flag é definida antes da coroutine: nenhum contato posterior reinicia o processo.
        if (hasBeenActivated || !collision.collider.CompareTag(playerTag))
            return;

        hasBeenActivated = true;
        StartCoroutine(MoveThenFall());
    }

    private IEnumerator MoveThenFall()
    {
        float horizontalDirection = Mathf.Approximately(movementDirection.x, 0f)
            ? 1f
            : Mathf.Sign(movementDirection.x);

        float elapsed = 0f;

        // O tempo é contado em passos de física. O valor padrão é exatamente 5 segundos.
        while (elapsed < movementDuration)
        {
            float step = Mathf.Min(Time.fixedDeltaTime, movementDuration - elapsed);
            Vector2 nextPosition = rb.position +
                Vector2.right * (horizontalDirection * horizontalSpeed * step);

            rb.MovePosition(nextPosition);
            elapsed += step;
            yield return new WaitForFixedUpdate();
        }

        // Interrompe completamente o deslocamento horizontal antes de ativar a gravidade.
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = gravityScaleAfterMovement;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
}
