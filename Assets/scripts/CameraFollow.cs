using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smooth = 5f;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Tras la transición")]
    public float yFija = 0f;
    public float suavidadCambio = 2f;
    public bool limitarX = false;
    public float minX = -10f;
    public float maxX = 10f;

    [Header("Zoom")]
    public float sizeCaida = 13f;
    public float sizeFinal = 8f;

    Camera cam;
    bool modoFinal;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = sizeCaida;
    }

    // Lo llama SecuenciaAterrizaje con la pantalla tapada por la cortina
    public void ActivarModoFinal()
    {
        modoFinal = true;
        cam.orthographicSize = sizeFinal;   // cambio instantáneo, oculto por la cortina

        // Colocar la cámara ya en su sitio final, sin que se vea el desplazamiento
        Vector3 p = transform.position;
        float x = target != null ? target.position.x + offset.x : p.x;
        transform.position = new Vector3(x, yFija, offset.z);
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (!modoFinal)
        {
            Vector3 desired = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desired, smooth * Time.deltaTime);
        }
        else
        {
            Vector3 pos = transform.position;
            float x = Mathf.Lerp(pos.x, target.position.x + offset.x, smooth * Time.deltaTime);
            if (limitarX) x = Mathf.Clamp(x, minX, maxX);
            transform.position = new Vector3(x, yFija, offset.z);
        }
    }
}