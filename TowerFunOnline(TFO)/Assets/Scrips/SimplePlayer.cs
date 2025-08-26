using System;
using UnityEngine;
using Photon.Pun;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Collider2D))]
public class SimplePlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI nicknameUI;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 8f;
    [Tooltip("Si está activo, el input vertical se aplica (útil si no usás gravedad). Si está desactivado, solo mueve en X.")]
    [SerializeField] private bool allowVerticalInput = false;

    [Header("Capas de colisión a reportar (opcional)")]
    [SerializeField] private LayerMask collisionMask = ~0; // todas por defecto

    private PhotonView pv;
    private Rigidbody2D rb;

    // Input acumulado para FixedUpdate
    private Vector2 inputMove;

    // Suavizado para remotos
    private Vector3 netTargetPos;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody2D>();

        // Config recomendado
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = allowVerticalInput ? 0f : 1f; // si no querés gravedad, dejá 0 y allowVerticalInput = true
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true; // no rotar sobre Z
    }

    void Start()
    {
        // Setear nick para todos
        if (pv.IsMine)
            pv.RPC(nameof(RPC_SetNickname), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);

        // El dueño simula; remotos interpolan (kinematic)
        if (!pv.IsMine)
        {
            rb.isKinematic = true;
            netTargetPos = transform.position;
        }
        else
        {
            rb.isKinematic = false;
        }
    }

    void Update()
    {
        if (pv.IsMine)
        {
            // Solo dueños leen input
            float x = Input.GetAxisRaw("Horizontal");
            float y = allowVerticalInput ? Input.GetAxisRaw("Vertical") : 0f;
            inputMove = new Vector2(x, y).normalized;
        }
        else
        {
            // Interpolación de remotos
            transform.position = Vector3.Lerp(transform.position, netTargetPos, 0.2f);
        }

        // (Si el nickname está en World Space, podés hacer que siga al jugador)
        // if (nicknameUI != null) nicknameUI.transform.position = transform.position + new Vector3(0, 0.8f, 0);
    }

    void FixedUpdate()
    {
        if (!pv.IsMine) return;

        // Movimiento por velocidad (nada de Translate)
        if (allowVerticalInput && rb.gravityScale == 0f)
        {
            // Top-down o sin gravedad: mueve en X/Y
            rb.velocity = inputMove * moveSpeed;
        }
        else
        {
            // Con gravedad: solo X, conserva la Y física
            rb.velocity = new Vector2(inputMove.x * moveSpeed, rb.velocity.y);
        }
    }

    // --- COLISIONES 2D ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!pv.IsMine) return;

        // Reporte opcional de colisión, útil para debug/logs
        if (((1 << collision.gameObject.layer) & collisionMask.value) != 0)
        {
            pv.RPC(nameof(RPC_InformCollision), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
        }
    }

    // Nota: el empuje entre jugadores lo maneja la física sola si ambos tienen Rigidbody2D dinámico y colliders no-trigger.

    // --- PUN RPCs / Sync ---
    [PunRPC] public void RPC_SetNickname(string nickname) => nicknameUI.text = nickname;

    [PunRPC]
    public void RPC_InformCollision(string nickname)
    {
        Debug.Log($"Colisión detectada por: {nickname}");
    }

    // Sync mínima (posición + velocidad) si no usás PhotonTransformView/PhotonRigidbody2DView
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(rb.velocity);
        }
        else
        {
            netTargetPos = (Vector3)stream.ReceiveNext();
            Vector2 _ = (Vector2)stream.ReceiveNext(); // recibido pero no usado aquí
        }
    }
}

