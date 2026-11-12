using System.Collections;
using UnityEngine;

public class SecuenciaAterrizaje : MonoBehaviour
{
    [SerializeField] float esperaAterrizaje = 1f;   // duración aproximada de aterrizar
    [SerializeField] CameraFollow camara;
    [SerializeField] FreeFall caida;
    [SerializeField] Animator animCaida;
    [SerializeField] TransicionLateral transicion;
    [SerializeField] string estadoAterrizar = "aterrizar";

    [Header("Cambio de personaje")]
    [SerializeField] GameObject jesterFrontal;
    [SerializeField] GameObject jesterLateral;

    bool lanzada;

    void Reset()
    {
        caida = GetComponent<FreeFall>();
    }

    void Update()
    {
        if (!caida.isFalling && !lanzada)
        {
            lanzada = true;
            StartCoroutine(Secuencia());
        }
    }

    IEnumerator Secuencia()
    {
        // Espera fija (más fiable que leer el estado del Animator)
        yield return new WaitForSeconds(esperaAterrizaje);

        yield return transicion.Cerrar();

        caida.enabled = false;
        jesterFrontal.SetActive(false);
        jesterLateral.SetActive(true);
        if (camara != null) camara.ActivarModoFinal();
        var control = GetComponent<ControlLateral>();
        if (control != null)
        {
            // El pivote del lateral queda en el suelo actual
            control.enabled = true;
        }

        yield return new WaitForSeconds(0.25f);

        yield return transicion.Abrir();
    }
}