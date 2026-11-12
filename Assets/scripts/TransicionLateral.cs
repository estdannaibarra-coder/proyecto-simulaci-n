using System.Collections;
using UnityEngine;

public class TransicionLateral : MonoBehaviour
{
    [SerializeField] RectTransform cortina;
    [SerializeField] float anchoPantalla = 2400f;   // un poco más que la pantalla
    [SerializeField] float duracion = 0.6f;

    void Start()
    {
        // Fuera de pantalla a la izquierda
        cortina.anchoredPosition = new Vector2(-anchoPantalla, 0f);
    }

    // La cortina entra desde la izquierda y tapa todo
    public IEnumerator Cerrar()
    {
        yield return Mover(-anchoPantalla, 0f);
    }

    // La cortina sale por la derecha y descubre la escena
    public IEnumerator Abrir()
    {
        yield return Mover(0f, anchoPantalla);
        cortina.anchoredPosition = new Vector2(-anchoPantalla, 0f); // lista para la próxima
    }

    IEnumerator Mover(float desde, float hasta)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duracion;
            float k = Mathf.SmoothStep(0f, 1f, t);
            float x = Mathf.Lerp(desde, hasta, k);
            cortina.anchoredPosition = new Vector2(x, 0f);
            yield return null;
        }
        cortina.anchoredPosition = new Vector2(hasta, 0f);
    }
}