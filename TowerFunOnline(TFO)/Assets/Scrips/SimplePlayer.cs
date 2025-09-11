using UnityEngine;
using Photon.Pun;
using TMPro;

public class SimplePlayer : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI nicknameUI;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private LayerMask groundMask;

    [Header("Empuje")]
    [SerializeField] private float pushForce = 5f; // Fuerza de empuje configurable

    private PhotonView photonView;
    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody2D>();

        // Aseguramos que no rote cuando choca
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        float horizontal = Input.GetAxis("Horizontal");

        // Movimiento lateral
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);

        // Saltar si está en el suelo
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Detectar suelo
        if (((1 << collision.gameObject.layer) & groundMask) != 0)
        {
            isGrounded = true;
        }

        // Empujar a otros jugadores
        if (!photonView.IsMine) return; // Solo el jugador local maneja empuje

        if (collision.gameObject.CompareTag("Player"))
        {
            PhotonView otherView = collision.gameObject.GetComponent<PhotonView>();
            if (otherView != null && !otherView.IsMine) // Empujar solo a otros
            {
                Rigidbody2D otherRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (otherRb != null)
                {
                    // Dirección desde mí hacia el otro jugador
                    Vector2 pushDir = (collision.transform.position - transform.position).normalized;

                    // Aplicar fuerza de empuje
                    otherRb.AddForce(pushDir * pushForce, ForceMode2D.Impulse);
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundMask) != 0)
        {
            isGrounded = false;
        }
    }

    [PunRPC]
    public void RPC_SetNickname(string nickname)
    {
        nicknameUI.text = nickname;
    }
}
