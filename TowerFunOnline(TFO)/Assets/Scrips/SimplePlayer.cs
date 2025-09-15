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

    // >>> añadido: última dirección de movimiento horizontal
    public Vector2 LastMoveDir { get; private set; } = Vector2.right;
    private float horizontal;

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

        // >>> actualizado: recordar última dirección si hay input
        if (Mathf.Abs(horizontal) > 0.01f)
            LastMoveDir = new Vector2(Mathf.Sign(horizontal), 0f);

        if (lava == null)
            lava = FindObjectOfType<LavaRise>();

        bool lavaStarted = (lava != null && lava.IsRising);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && lavaStarted)
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
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
        nicknameUI.text = nickname;
    }
}
