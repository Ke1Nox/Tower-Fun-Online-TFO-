using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class SimplePlayer : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI nicknameUI;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private LayerMask groundMask;

    private PhotonView photonView;
    private Rigidbody2D rb;
    private bool isGrounded;

    private LavaRise lava;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody2D>();
        lava = FindObjectOfType<LavaRise>();
    }

    private float horizontal;
    private float lastHorizontal = 1f; // dirección mirando (1 derecha, -1 izquierda)

    [Header("Empujón (networked)")]
    [Tooltip("Ruta dentro de Resources al prefab (ej: \"Prefabs/PushHitbox\")")]
    [SerializeField] private string pushHitboxPrefabPath = "Prefabs/PushHitbox";
    [SerializeField] private float pushOffset = 1f;
    [SerializeField] private float pushCooldown = 0.6f;
    private float lastPushTime = -999f;

    void Update()
    {
        if (!photonView.IsMine) return;

        horizontal = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizontal) > 0.01f) lastHorizontal = Mathf.Sign(horizontal);

        if (lava == null)
        {
            lava = FindObjectOfType<LavaRise>();
        }

        bool lavaStarted = (lava != null && lava.IsRising);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && lavaStarted)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // Empujón: E
        if (Input.GetKeyDown(KeyCode.E) && Time.time - lastPushTime >= pushCooldown)
        {
            DoPush();
            lastPushTime = Time.time;
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
    }

    // ui arriba del jugador 
    void LateUpdate()
    {
        if (nicknameUI != null)
        {
            Vector3 offset = new Vector3(0, 1f, 0);
            nicknameUI.transform.position = transform.position + offset;
            nicknameUI.transform.rotation = Quaternion.identity;
        }
    }

    private void DoPush()
    {
        // Asegurar que exista el prefab en Resources/...
        if (string.IsNullOrEmpty(pushHitboxPrefabPath))
        {
            Debug.LogWarning("PushHitbox prefab path vacío. Pon el prefab en Resources y setea pushHitboxPrefabPath.");
            return;
        }

        // Calcula spawn según la última dirección
        float dir = lastHorizontal >= 0 ? 1f : -1f;
        Vector3 spawnPos = transform.position + new Vector3(pushOffset * dir, 0f, 0f);

        // Instanciar en red (todos verán la hitbox)
        PhotonNetwork.Instantiate(pushHitboxPrefabPath, spawnPos, Quaternion.identity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundMask) != 0)
        {
            isGrounded = true;
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

    // Este RPC se llama en el cliente "dueño" del jugador alcanzado.
    [PunRPC]
    public void RPC_ApplyPush(float vx, float vy)
    {
        // Solo el dueño del PhotonView debe aplicar la fuerza localmente
        if (!photonView.IsMine) return;

        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(new Vector2(vx, vy), ForceMode2D.Impulse);
        }
    }
}