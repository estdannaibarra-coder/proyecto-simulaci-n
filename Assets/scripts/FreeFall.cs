using UnityEngine;

public class FreeFall : MonoBehaviour
{
    [Header("Referencia")]
    public Transform ground;

    [Header("Parámetros físicos")]
    public float gravity = 9.81f;
    public float dragCoefficient = 0.05f;

    [Header("Movimiento lateral (esquivar)")]
    public float horizontalSpeed = 6f;
    public float minX = -8f;
    public float maxX = 8f;

    [Header("Obstáculos")]
    public string obstacleTag = "obstaculo";

    [Header("Hitbox (solo si NO hay SpriteRenderer, ej. el jester)")]
    public Vector2 hitboxSize = new Vector2(2f, 5f);
    public Vector2 hitboxOffset = new Vector2(0f, 0f); // centro respecto al pivote

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

    SpriteRenderer sr;
    GameObject[] obstacles;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();   // puede ser null (jester)
        startY = transform.position.y;
        startX = transform.position.x;
        groundTop = ground.GetComponent<SpriteRenderer>().bounds.max.y;

        obstacles = GameObject.FindGameObjectsWithTag(obstacleTag);

        terminalVelocity = dragCoefficient > 0f
            ? Mathf.Sqrt(gravity / dragCoefficient)
            : float.PositiveInfinity;
    }

    // Usa el SpriteRenderer si existe; si no, la hitbox manual
    Bounds GetBounds()
    {
        if (sr != null) return sr.bounds;

        Vector3 s = transform.lossyScale;
        Vector3 center = transform.position +
            new Vector3(hitboxOffset.x * s.x, hitboxOffset.y * s.y, 0f);
        Vector3 size = new Vector3(hitboxSize.x * Mathf.Abs(s.x),
                                   hitboxSize.y * Mathf.Abs(s.y), 100f);
        return new Bounds(center, size);
    }

    void Update()
    {
        if (!isFalling) return;

        float dt = Time.deltaTime;
        time += dt;
        inputX = Input.GetAxisRaw("Horizontal");

        float a = gravity - dragCoefficient * velocityY * velocityY;
        velocityY += a * dt;

        Vector3 pos = transform.position;
        pos.y -= velocityY * dt;
        pos.x += inputX * horizontalSpeed * dt;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;

        if (HitsObstacle())
        {
            Restart();
            return;
        }

        float bottom = GetBounds().min.y;
        if (bottom <= groundTop)
        {
            pos.y += groundTop - bottom;
            transform.position = pos;
            isFalling = false;
        }

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

    bool HitsObstacle()
    {
        Bounds myBounds = GetBounds();
        foreach (GameObject obs in obstacles)
        {
            SpriteRenderer obsSr = obs.GetComponent<SpriteRenderer>();
            if (obsSr != null && myBounds.Intersects(obsSr.bounds))
                return true;
        }
        return false;
    }

    void Restart()
    {
        transform.position = new Vector3(startX, startY, transform.position.z);
        velocityY = 0f;
        time = 0f;
        distanceFallen = 0f;
        isFalling = true;
    }

    void OnDrawGizmosSelected()
    {
        if (GetComponent<SpriteRenderer>() != null) return;
        Vector3 s = transform.lossyScale;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            transform.position + new Vector3(hitboxOffset.x * s.x, hitboxOffset.y * s.y, 0f),
            new Vector3(hitboxSize.x * Mathf.Abs(s.x), hitboxSize.y * Mathf.Abs(s.y), 0f));
    }
}