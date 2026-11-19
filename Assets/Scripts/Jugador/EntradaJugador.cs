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
    private InputAction garra, pelo, lanzar, interactuar, mochila, pausa;
    public bool Bloqueada { get; set; }
    public bool GarraPulsada => !Bloqueada && garra != null && garra.WasPressedThisFrame();
    public bool PeloPulsado => !Bloqueada && pelo != null && pelo.WasPressedThisFrame();
    public bool PeloSoltado => !Bloqueada && pelo != null && pelo.WasReleasedThisFrame();
    public bool PeloMantenido => !Bloqueada && pelo != null && pelo.IsPressed();
    public bool LanzarPulsado => !Bloqueada && lanzar != null && lanzar.WasPressedThisFrame();
    public bool InteractuarPulsado => !Bloqueada && interactuar != null && interactuar.WasPressedThisFrame();
    public bool MochilaPulsada => mochila != null && mochila.WasPressedThisFrame();
    public bool PausaPulsada => pausa != null && pausa.WasPressedThisFrame();
    public void LimpiarSalto() => saltoPendiente = false;

    public float Horizontal => !Bloqueada && mover != null && isActiveAndEnabled
        ? Mathf.Clamp(mover.ReadValue<Vector2>().x, -1f, 1f) : 0f;
    public bool Corriendo => !Bloqueada && correr != null && isActiveAndEnabled && correr.IsPressed();

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
        garra = Activar("Attack"); pelo = Activar("Hairball"); lanzar = Activar("Throw");
        interactuar = Activar("Interact"); mochila = Activar("Inventory"); pausa = Activar("Pause");
    }

    private InputAction Activar(string nombre)
    {
        var accion = instancia.FindAction("Player/" + nombre, false);
        accion?.Enable(); return accion;
    }
    private void RegistrarSalto(InputAction.CallbackContext contexto) { if (!Bloqueada) saltoPendiente = true; }

    public bool ConsumirSalto()
    {
        bool solicitado = !Bloqueada && saltoPendiente;
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
