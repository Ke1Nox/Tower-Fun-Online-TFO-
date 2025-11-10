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

    
    private Rigidbody2D rb;
    private bool isGrounded;

    private LavaRise lava;

    private float horizontal;
    private float lastHorizontal = 1f; // 1 derecha, -1 izquierda
    private float vertical;
    private float lastVertical = -1f;

    [Header("Empujón")]
    [Tooltip("Ruta dentro de Resources al prefab (ej: \"Prefabs/PushHitbox\")")]
    [SerializeField] private string pushHitboxPrefabPath = "Prefabs/PushHitbox";
    [SerializeField] private float pushOffset = 1f;
    [SerializeField] private float pushCooldown = 0.6f;

    [Header("Ghost Shoot")]
    [SerializeField] private string ghostBulletPrefabPath = "Prefabs/GhostBulletPush"; // ruta en Resources/ o Pool de PUN
    [SerializeField] private float ghostShootCooldown = 0.35f;
    [SerializeField] private float ghostMuzzleOffsetX = 0.6f; 

    private float lastGhostShotTime = -999f;

    static public bool isdead = false;
    private float lastPushTime = -999f;

    void Start()
    {
       
        rb = GetComponent<Rigidbody2D>();
        lava = FindObjectOfType<LavaRise>();
    }

    void Update()
    {
        //Moviminetos
        if (!photonView.IsMine) return;
        
            horizontal = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizontal) > 0.01f) lastHorizontal = Mathf.Sign(horizontal);
        
           
            vertical = Input.GetAxis("Vertical");
            if (Mathf.Abs(vertical) < 0.01f) lastVertical = Mathf.Sign(vertical);
        


        if (lava == null)
            lava = FindObjectOfType<LavaRise>();

        bool lavaStarted = (lava != null && lava.IsRising);

        //impedir saltar jugador antes que inicie la partida
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && lavaStarted)
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

       //push e 
        if (Input.GetKeyDown(KeyCode.E) && Time.time - lastPushTime >= pushCooldown)
        {
            DoPush();
            lastPushTime = Time.time;
        }

        //fantasma 
        // Disparo fantasma con ESPACIO (solo cuando está "Dead")
        if (CompareTag("Dead") && Input.GetKeyDown(KeyCode.Space) && Time.time - lastGhostShotTime >= ghostShootCooldown)
        {
            lastGhostShotTime = Time.time;
            ShootGhostBullet();
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        bool inKnockback = Time.time < knockbackUntil;
        if (inKnockback) return;

        if (CompareTag("Dead"))
        {
            // Movimiento vertical cuando está muerto
            rb.velocity = new Vector2(rb.velocity.x, vertical * moveSpeed);
        }
        else
        {
            // Movimiento horizontal normal
            rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
        }
    }

    // UI arriba del jugador (no rota ahora)
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
        //para donde mira el player
        float dir = lastHorizontal >= 0 ? 1f : -1f;
        Vector3 spawnPos = transform.position + new Vector3(pushOffset * dir, 0f, 0f);

        // Instanciar en red
        GameObject go = PhotonNetwork.Instantiate(pushHitboxPrefabPath, spawnPos, Quaternion.identity);

        
        go.transform.localScale = new Vector3(dir, 1f, 1f);
    }


    //verificador de piso
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

    //disparo fantasma
    private void ShootGhostBullet()
    {
        //spawn nueva baala
        Vector3 spawnPos = transform.position + new Vector3(-Mathf.Abs(ghostMuzzleOffsetX), 0f, 0f);

        GameObject gb = PhotonNetwork.Instantiate(ghostBulletPrefabPath, spawnPos, Quaternion.identity);

        
        var bullet = gb.GetComponent<GhostBulletPush>();
        if (bullet != null) bullet.Initialize(Vector2.left);
    }

    [PunRPC]
    public void RPC_SetNickname(string nickname)
    {
        if (nicknameUI != null) nicknameUI.text = nickname;
    }


    //empuje
    [PunRPC]
    public void RPC_ApplyPush(float vx, float vy)
    {
        
        if (!photonView.IsMine) return;

        if (rb == null) rb = GetComponent<Rigidbody2D>();

       
        rb.velocity = new Vector2(0f, rb.velocity.y);


        rb.AddForce(new Vector2(vx, vy), ForceMode2D.Impulse);
        knockbackUntil = Time.time + knockbackTime;

       
    }
    [PunRPC]
    public void RPC_BecomeGhost(float x, float y)
    {
        if (!photonView.IsMine) return;

        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;

        transform.position = new Vector3(x, y, 0f);
        gameObject.tag = "Dead";

        SimplePlayer.isdead = true;
    }
}
