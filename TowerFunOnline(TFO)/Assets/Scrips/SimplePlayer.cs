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

    private PhotonView photonView;
    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        float horizontal = Input.GetAxis("Horizontal");

      
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    //rotacion recto del nickname 
    void LateUpdate()
    {
        if (nicknameUI != null)
        {
            // Siempre encima del jugador
            Vector3 offset = new Vector3(0, 1f, 0); 
            nicknameUI.transform.position = transform.position + offset;

            // Mantener rotación fija 
            nicknameUI.transform.rotation = Quaternion.identity;
        }
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
}