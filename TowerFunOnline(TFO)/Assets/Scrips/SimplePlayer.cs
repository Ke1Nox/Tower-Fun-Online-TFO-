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

    [Header("Knockback")]
    [SerializeField] private float knockbackTime = 0.25f;
    private float knockbackUntil = -1f;

    private PhotonView photonView;
    private Rigidbody2D rb;
    private bool isGrounded;

    private LavaRise lava;

    private float horizontal;
    private float lastHorizontal = 1f; // 1 derecha, -1 izquierda

    [Header("Empujón")]
    [Tooltip("Ruta dentro de Resources al prefab (ej: \"Prefabs/PushHitbox\")")]
    [SerializeField] private string pushHitboxPrefabPath = "Prefabs/PushHitbox";
    [SerializeField] private float pushOffset = 1f;
    [SerializeField] private float pushCooldown = 0.6f;
    private float lastPushTime = -999f;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody2D>();
        lava = FindObjectOfType<LavaRise>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        horizontal = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizontal) > 0.01f) lastHorizontal = Mathf.Sign(horizontal);

        if (lava == null)
            lava = FindObjectOfType<LavaRise>();

        bool lavaStarted = (lava != null && lava.IsRising);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && lavaStarted)
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

       
        if (Input.GetKeyDown(KeyCode.E) && Time.time - lastPushTime >= pushCooldown)
        {
            DoPush();
            lastPushTime = Time.time;
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        //NO pisamos la velocidad actua la física.
        bool inKnockback = Time.time < knockbackUntil;
        if (inKnockback) return;

        // Movimiento normal
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
    }

    // UI arriba del jugador
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
        float dir = lastHorizontal >= 0 ? 1f : -1f;
        Vector3 spawnPos = transform.position + new Vector3(pushOffset * dir, 0f, 0f);

        // Instanciar en red
        GameObject go = PhotonNetwork.Instantiate(pushHitboxPrefabPath, spawnPos, Quaternion.identity);

        
        go.transform.localScale = new Vector3(dir, 1f, 1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundMask) != 0)
            isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundMask) != 0)
            isGrounded = false;
    }

    [PunRPC]
    public void RPC_SetNickname(string nickname)
    {
        if (nicknameUI != null) nicknameUI.text = nickname;
    }

    [PunRPC]
    public void RPC_ApplyPush(float vx, float vy)
    {
        
        if (!photonView.IsMine) return;

        if (rb == null) rb = GetComponent<Rigidbody2D>();

       
        rb.velocity = new Vector2(0f, rb.velocity.y);

       
        rb.AddForce(new Vector2(vx, vy), ForceMode2D.Impulse);
        knockbackUntil = Time.time + knockbackTime;

        Debug.Log($"Recibí push: {vx}, {vy}. Knockback hasta: {knockbackUntil}");
    }
}
