using UnityEngine;

public class FreeFall : MonoBehaviour
{
    [Header("Referencia")]
    public Transform ground;              // arrastra aquí el Suelo

    [Header("Parámetros físicos")]
    public float gravity = 9.81f;
    public float dragCoefficient = 0.05f;

    [Header("Movimiento lateral (esquivar)")]
    public float horizontalSpeed = 6f;
    public float minX = -8f;
    public float maxX = 8f;

    [Header("Obstáculos")]
    public string obstacleTag = "obstaculo";

    [Header("Datos en tiempo real (solo lectura)")]
    public bool isFalling = true;
    public float time;
    public float velocityY;
    public float distanceFallen;
    public float theoreticalV;
    public float theoreticalY;
    public float terminalVelocity;

    float inputX;
    float startY;
    float startX;
    float groundTop;
    float halfHeight;

    SpriteRenderer sr;
    GameObject[] obstacles;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        startY = transform.position.y;
        startX = transform.position.x;
        halfHeight = sr.bounds.extents.y;
        groundTop = ground.GetComponent<SpriteRenderer>().bounds.max.y;

        obstacles = GameObject.FindGameObjectsWithTag(obstacleTag);

        terminalVelocity = dragCoefficient > 0f
            ? Mathf.Sqrt(gravity / dragCoefficient)
            : float.PositiveInfinity;
    }

    void Update()
    {
        if (!isFalling) return;

        float dt = Time.deltaTime;
        time += dt;
        inputX = Input.GetAxisRaw("Horizontal");

        // --- Simulación: integración de Euler ---
        float a = gravity - dragCoefficient * velocityY * velocityY;
        velocityY += a * dt;

        Vector3 pos = transform.position;
        pos.y -= velocityY * dt;
        pos.x += inputX * horizontalSpeed * dt;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);

        transform.position = pos;

        // --- Colisión con obstáculos ---
        if (HitsObstacle())
        {
            Restart();
            return;
        }

        // --- Colisión con el suelo ---
        float bottom = pos.y - halfHeight;
        if (bottom <= groundTop)
        {
            pos.y = groundTop + halfHeight;
            transform.position = pos;
            isFalling = false;
        }

        // --- Datos para comparar con la teoría ---
        distanceFallen = startY - pos.y;
        theoreticalV = gravity * time;
        theoreticalY = 0.5f * gravity * time * time;

        if (!isFalling)
        {
            Debug.Log($"Aterrizó | t = {time:F2} s | v = {velocityY:F2} m/s | " +
                      $"distancia = {distanceFallen:F2} m | " +
                      $"v teórica (sin aire) = {theoreticalV:F2} m/s");
        }
    }

    // Revisa si el personaje se solapa con algún obstáculo
    bool HitsObstacle()
    {
        Bounds myBounds = sr.bounds;

        foreach (GameObject obs in obstacles)
        {
            SpriteRenderer obsSr = obs.GetComponent<SpriteRenderer>();
            if (obsSr != null && myBounds.Intersects(obsSr.bounds))
                return true;
        }
        return false;
    }

    // Vuelve al inicio y reinicia todo
    void Restart()
    {
        transform.position = new Vector3(startX, startY, transform.position.z);
        velocityY = 0f;
        time = 0f;
        distanceFallen = 0f;
        isFalling = true;
    }
}