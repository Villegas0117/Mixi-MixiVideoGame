using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class EntradaJugador : MonoBehaviour
{
    [SerializeField] private InputActionAsset acciones;
    private InputActionAsset instancia;
    private InputAction mover;
    private InputAction saltar;
    private InputAction correr;
    private bool saltoPendiente;

    public float Horizontal => mover != null && isActiveAndEnabled
        ? Mathf.Clamp(mover.ReadValue<Vector2>().x, -1f, 1f) : 0f;
    public bool Corriendo => correr != null && isActiveAndEnabled && correr.IsPressed();

    private void OnEnable()
    {
        if (acciones == null)
        {
            Debug.LogError("EntradaJugador requiere el asset de acciones existente.", this);
            enabled = false;
            return;
        }
        // Copia local: no habilitar otras acciones ni alterar las acciones globales.
        instancia = Instantiate(acciones);
        mover = instancia.FindAction("Player/Move", true);
        saltar = instancia.FindAction("Player/Jump", true);
        correr = instancia.FindAction("Player/Sprint", true);
        saltoPendiente = false;
        saltar.performed += RegistrarSalto;
        mover.Enable();
        saltar.Enable();
        correr.Enable();
    }

    private void RegistrarSalto(InputAction.CallbackContext contexto) => saltoPendiente = true;

    public bool ConsumirSalto()
    {
        bool solicitado = saltoPendiente;
        saltoPendiente = false;
        return solicitado;
    }

    private void OnApplicationFocus(bool enfocado)
    {
        if (!enfocado) saltoPendiente = false;
    }

    private void OnDisable()
    {
        saltoPendiente = false;
        if (instancia == null) return;
        saltar.performed -= RegistrarSalto;
        instancia.Disable();
        Destroy(instancia);
        instancia = null;
        mover = saltar = correr = null;
    }
}
