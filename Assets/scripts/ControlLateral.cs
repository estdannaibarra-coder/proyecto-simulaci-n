using UnityEngine;

public class ControlLateral : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] Animator anim;
    [SerializeField] Transform visual;          // el hijo jester_lateral
    [SerializeField] Transform suelo;           // el objeto suelo

    [Header("Movimiento")]
    [SerializeField] float velocidadCorrer = 6f;
    [SerializeField] float minX = -8f;
    [SerializeField] float maxX = 8f;

    [Header("Salto")]
    [SerializeField] float alturaSalto = 3f;
    [SerializeField] float gravedad = 25f;
    [SerializeField] float frameSalto = 24f;    // frames del clip saltar
    [SerializeField] float fpsClip = 60f;
    [SerializeField] string estadoSalto = "saltar";

    [Header("Reposo")]
    [SerializeField] float segundosParaReposo = 5f;

    static readonly int pVelocidad = Animator.StringToHash("Velocidad");
    static readonly int pEnSuelo = Animator.StringToHash("EnSuelo");
    static readonly int pSalto = Animator.StringToHash("Salto");

    float velY;
    float yBase;            // altura del suelo (posición Y de reposo)
    bool enSuelo = true;
    float dirSalto;         // dirección horizontal mantenida en el aire
    float tiempoSinInput;
    float velSaltoInicial;
    float duracionAire;

    void Start()
    {
        yBase = transform.position.y;
        velSaltoInicial = Mathf.Sqrt(2f * gravedad * alturaSalto);
        duracionAire = 2f * velSaltoInicial / gravedad;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        bool pulsaSalto = Input.GetKeyDown(KeyCode.UpArrow);
        bool hayInput = Mathf.Abs(h) > 0.01f || Input.GetKey(KeyCode.UpArrow);

        // Contador de inactividad
        tiempoSinInput = hayInput ? 0f : tiempoSinInput + Time.deltaTime;
        bool reposoForzado = tiempoSinInput >= segundosParaReposo;

        // Iniciar salto
        if (pulsaSalto && enSuelo)
        {
            enSuelo = false;
            velY = velSaltoInicial;
            dirSalto = h;                       // 0 = salto vertical
            anim.SetBool(pEnSuelo, false);
            anim.SetTrigger(pSalto);

            // Ajustar la animación a la duración real del salto
            float duracionClip = frameSalto / fpsClip;
            anim.speed = duracionClip / duracionAire;
        }

        Vector3 pos = transform.position;

        if (enSuelo)
        {
            // Correr
            pos.x += h * velocidadCorrer * Time.deltaTime;
            float vel = reposoForzado ? 0f : Mathf.Abs(h);
            anim.SetFloat(pVelocidad, vel);
        }
        else
        {
            // Arco: horizontal fijo + vertical con gravedad
            pos.x += dirSalto * velocidadCorrer * Time.deltaTime;
            velY -= gravedad * Time.deltaTime;
            pos.y += velY * Time.deltaTime;
            anim.SetFloat(pVelocidad, 0f);

            if (pos.y <= yBase)
            {
                pos.y = yBase;
                velY = 0f;
                enSuelo = true;
                anim.speed = 1f;
                anim.SetBool(pEnSuelo, true);
            }
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;

        // Voltear según dirección (la animación mira a la derecha)
        float dir = enSuelo ? h : dirSalto;
        if (Mathf.Abs(dir) > 0.01f)
        {
            Vector3 s = visual.localScale;
            s.x = Mathf.Abs(s.x) * Mathf.Sign(dir);
            visual.localScale = s;
        }
    }
}