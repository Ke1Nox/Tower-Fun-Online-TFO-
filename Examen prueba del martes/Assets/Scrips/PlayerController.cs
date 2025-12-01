using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float fadingCrono;
    [SerializeField] float speed;
    [SerializeField] float rotSpeed;
    bool dead;
    [SerializeField] LayerMask ground;
    bool moving = true;
    [SerializeField] SkinnedMeshRenderer[] skinnedMeshRenderer;
    [SerializeField] private GameObject uf;

    private Animator animator;

    public Animator Anim => animator;

    private void Awake()
    { fadingCrono = 0.15f; }

    private void Start()
    {
   
       GameManager.Instance.RegisterPlayer(this);
        rb = GetComponent<Rigidbody>();

        animator = GetComponent<Animator>();

        var gm = GameManager.Instance;

        gm.OnPlayerDeath += OnPlayerDeath;

        animator.SetBool("IsLive", true);

        //AudioSource = GetComponent<AudioSource>();

        //GameManager.GMSharedInstance.LoadData();

        transform.position = new Vector3(PlayerPrefs.GetFloat("posX"), PlayerPrefs.GetFloat("posY"), PlayerPrefs.GetFloat("posZ"));
    }

    private void Update()
    { /*animatioParametres();*/ }

    void FixedUpdate()
    {
        if (!dead)
        { movement();
         AnimacionesParametros();
        }
    }

    void movement()
    {
        
       
            Quaternion rot = Quaternion.Euler(0, Input.GetAxisRaw("Horizontal") * rotSpeed, 0); // DEPILATEEE Y UNTATE E L SPLASH DE CHOCOLATEEE
            rb.MoveRotation(rb.rotation * rot);

            if (Input.GetAxisRaw("Vertical") > 0)
            {

                rb.MovePosition(transform.position + transform.forward * speed * Time.fixedDeltaTime);
                moving = true;

            }
            else if (Input.GetAxisRaw("Vertical") < 0)
            {
                
                rb.MovePosition(transform.position - transform.forward * speed * Time.fixedDeltaTime);
            moving = true;

        }
            else if (Input.GetAxisRaw("Vertical") == 0)
            {
            moving = false;
        }
        

    }

    private void AnimacionesParametros()
    {
        animator.SetFloat("Vel", Input.GetAxisRaw("Horizontal"));
        animator.SetBool("Moving", moving);
    }
    void OnPlayerDeath()
    {
        animator.SetTrigger("Death");
        animator.SetBool("IsLive", false);
        moving = false;
        uf.SetActive(true);
        StartCoroutine(MsjUf());
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals("PlataDead"))
        {

            GameManager.Instance.Morir();
           

        }
    }

    private IEnumerator MsjUf()
    {
        yield return new WaitForSeconds(2);
        uf.SetActive(false);
    }

    void fading()
    {
        if (dead)
        {
            fadingCrono -= Time.deltaTime;
            foreach (var meshes in skinnedMeshRenderer)
            {
                if (fadingCrono > 0)
                { meshes.enabled = true; }
                else if (fadingCrono <= 0)
                { meshes.enabled = false; }
            }
            if (fadingCrono < -0.15f)
            { fadingCrono = 0.15f; }
        }
    }

    void restartPlayer()
    {
        dead = false;
        transform.position = Vector3.zero;
        foreach (var meshes in skinnedMeshRenderer)
        {
            meshes.enabled = true;
            fadingCrono = 0.15f;
        }
    }

}