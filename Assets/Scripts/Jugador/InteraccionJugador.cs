using UnityEngine;
public sealed class InteraccionJugador : MonoBehaviour
{
    private EntradaJugador entrada;
    private RecursosJugador recursos;
    public Recogible Cercano { get; private set; }
    private readonly Collider2D[] encontrados=new Collider2D[24];
    private void Awake() { entrada=GetComponent<EntradaJugador>(); recursos=GetComponent<RecursosJugador>(); }
    private void Update()
    {
        Cercano=null;
        if (entrada.Bloqueada) return;
        var filtro=new ContactFilter2D { useTriggers=true };
        int n=Physics2D.OverlapCircle(transform.position,1.5f,filtro,encontrados);
        float distancia=float.MaxValue;
        for (int i=0;i<n;i++) {
            var r=encontrados[i].GetComponent<Recogible>(); if (r==null) continue;
            float d=Vector2.Distance(transform.position,r.transform.position);
            if (d<distancia) { distancia=d; Cercano=r; }
        }
        if (Cercano!=null && entrada.InteractuarPulsado) Cercano.Interactuar(recursos);
    }
}
