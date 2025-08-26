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
    [Tooltip("Si est� activo, el input vertical se aplica (�til si no us�s gravedad). Si est� desactivado, solo mueve en X.")]
    [SerializeField] private bool allowVerticalInput = false;

    [Header("Capas de colisi�n a reportar (opcional)")]
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
        rb.gravityScale = allowVerticalInput ? 0f : 1f; // si no quer�s gravedad, dej� 0 y allowVerticalInput = true
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true; // no rotar sobre Z
    }

    void Start()
    {
        // Setear nick para todos
        if (pv.IsMine)
            pv.RPC(nameof(RPC_SetNickname), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);

        // El due�o simula; remotos interpolan (kinematic)
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
            // Solo due�os leen input
            float x = Input.GetAxisRaw("Horizontal");
            float y = allowVerticalInput ? Input.GetAxisRaw("Vertical") : 0f;
            inputMove = new Vector2(x, y).normalized;
        }
        else
        {
            // Interpolaci�n de remotos
            transform.position = Vector3.Lerp(transform.position, netTargetPos, 0.2f);
        }

        // (Si el nickname est� en World Space, pod�s hacer que siga al jugador)
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
            // Con gravedad: solo X, conserva la Y f�sica
            rb.velocity = new Vector2(inputMove.x * moveSpeed, rb.velocity.y);
        }
    }

    // --- COLISIONES 2D ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!pv.IsMine) return;

        // Reporte opcional de colisi�n, �til para debug/logs
        if (((1 << collision.gameObject.layer) & collisionMask.value) != 0)
        {
            pv.RPC(nameof(RPC_InformCollision), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
        }
    }

    // Nota: el empuje entre jugadores lo maneja la f�sica sola si ambos tienen Rigidbody2D din�mico y colliders no-trigger.

    // --- PUN RPCs / Sync ---
    [PunRPC] public void RPC_SetNickname(string nickname) => nicknameUI.text = nickname;

    [PunRPC]
    public void RPC_InformCollision(string nickname)
    {
        Debug.Log($"Colisi�n detectada por: {nickname}");
    }

    // Sync m�nima (posici�n + velocidad) si no us�s PhotonTransformView/PhotonRigidbody2DView
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
            Vector2 _ = (Vector2)stream.ReceiveNext(); // recibido pero no usado aqu�
        }
    }
}

