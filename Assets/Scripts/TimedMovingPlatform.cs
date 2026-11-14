using System.Collections;
using UnityEngine;

/// <summary>
/// A plataforma fica imóvel e sem gravidade até o primeiro contato físico
/// com o jogador. Depois anda horizontalmente por 5 segundos e cai.
/// A ativação é permanente e nunca é reiniciada.
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
    private bool isMoving;
    private bool hasStartedFalling;
    private float elapsedMovementTime;

    public bool HasBeenActivated => hasBeenActivated;
    public bool IsMoving => isMoving;
    public bool HasStartedFalling => hasStartedFalling;
    public float RemainingTime => Mathf.Max(0f, movementDuration - elapsedMovementTime);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Estado inicial exigido: parado, Kinematic e sem gravidade.
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // A flag é marcada antes de iniciar a rotina: nenhum novo contato reinicia o timer.
        if (hasBeenActivated || !collision.gameObject.CompareTag(playerTag))
            return;

        hasBeenActivated = true;
        StartCoroutine(MoveThenFall());
    }

    private IEnumerator MoveThenFall()
    {
        isMoving = true;
        elapsedMovementTime = 0f;

        float direction = Mathf.Approximately(movementDirection.x, 0f)
            ? 1f
            : Mathf.Sign(movementDirection.x);

        // Usa passos de física e limita o último passo para totalizar 5 segundos.
        while (elapsedMovementTime < movementDuration)
        {
            float step = Mathf.Min(Time.fixedDeltaTime, movementDuration - elapsedMovementTime);
            Vector2 displacement = Vector2.right * (direction * horizontalSpeed * step);
            rb.MovePosition(rb.position + displacement);
            elapsedMovementTime += step;
            yield return new WaitForFixedUpdate();
        }

        // Primeiro encerra todo movimento horizontal; depois ativa a gravidade.
        isMoving = false;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = gravityScaleAfterMovement;
        rb.bodyType = RigidbodyType2D.Dynamic;
        hasStartedFalling = true;
    }
}
