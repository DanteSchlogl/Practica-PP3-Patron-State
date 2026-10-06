using PedidoApp.DAL;
using PedidoApp.Domain;

namespace PedidoApp.BLL;

// Es la puerta de entrada para la pantalla. Arma el pedido y guarda el historial.
// La interfaz solo habla con esta clase y no conoce el resto de las capas.
public class ServicioPedido
{
    private readonly Pedido _pedido;
    private readonly IRepositorioHistorial _repositorio;

    public ServicioPedido(IRepositorioHistorial repositorio, int numero, decimal total)
    {
        if (repositorio == null) throw new ArgumentNullException(nameof(repositorio));

        _repositorio = repositorio;
        _pedido = new Pedido(numero, total);

        // Guardo el estado inicial, asi el historial arranca desde el principio.
        _repositorio.Registrar(_pedido.ObtenerResumen());
    }

    public int Numero
    {
        get { return _pedido.Numero; }
    }

    public decimal Total
    {
        get { return _pedido.Total; }
    }

    public string EstadoActual
    {
        get { return _pedido.EstadoActual; }
    }

    public IReadOnlyList<string> AccionesPermitidas
    {
        get { return _pedido.AccionesPermitidas; }
    }

    public string CarpetaHistoriales
    {
        get { return _repositorio.Carpeta; }
    }

    public int EventosRegistrados
    {
        get { return _repositorio.EventosRegistrados; }
    }

    // Cada accion cambia el estado y despues guarda el cambio.
    // Si el estado no la permite, se corta con la excepcion y no se guarda nada.
    public void Confirmar()
    {
        _pedido.Confirmar();
        Guardar();
    }

    public void Enviar()
    {
        _pedido.Enviar();
        Guardar();
    }

    public void Entregar()
    {
        _pedido.Entregar();
        Guardar();
    }

    public void Cancelar()
    {
        _pedido.Cancelar();
        Guardar();
    }

    public string ObtenerRutaHistorial()
    {
        return _repositorio.ObtenerRutaArchivo(_pedido.Numero);
    }

    public IReadOnlyList<string> LeerHistorial()
    {
        return _repositorio.LeerHistorial(_pedido.Numero);
    }

    private void Guardar()
    {
        _repositorio.Registrar(_pedido.ObtenerResumen());
    }
}
