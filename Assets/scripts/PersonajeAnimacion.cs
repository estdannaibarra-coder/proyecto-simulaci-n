using UnityEngine;

public class PersonajeAnimacion : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] FreeFall caida;
    [SerializeField] float velocidadCaida = 0.35f;
    [SerializeField] float velocidadAterrizaje = 1f;

    static readonly int pEnSuelo = Animator.StringToHash("EnSuelo");

    void Reset()
    {
        caida = GetComponent<FreeFall>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        bool enSuelo = !caida.isFalling;
        anim.SetBool(pEnSuelo, enSuelo);
        anim.speed = enSuelo ? velocidadAterrizaje : velocidadCaida;
    }
}