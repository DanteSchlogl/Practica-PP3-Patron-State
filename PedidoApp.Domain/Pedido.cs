using PedidoApp.Domain.Estados;
using PedidoApp.Domain.Exceptions;

namespace PedidoApp.Domain;

// Modela un pedido de una tienda online. Se comporta distinto segun el estado en el que este.
public class Pedido
{
    private IEstadoPedido _estado;

    public Pedido(int numero, decimal total)
    {
        if (total <= 0) throw new TotalInvalidoException(total);

        Numero = numero;
        Total = total;
        FechaCreacion = DateTime.Now;

        // Todo pedido arranca pendiente.
        _estado = new EstadoPendiente();
    }

    public int Numero { get; private set; }

    public decimal Total { get; private set; }

    public DateTime FechaCreacion { get; private set; }

    public string EstadoActual
    {
        get { return _estado.Nombre; }
    }

    // Lo que se puede hacer ahora. Depende del estado en el que este.
    public IReadOnlyList<string> AccionesPermitidas
    {
        get { return _estado.AccionesPermitidas; }
    }

    // Estos metodos le pasan la pelota al estado actual, que decide si se puede o no.
    public void Confirmar()
    {
        _estado.Confirmar(this);
    }

    public void Enviar()
    {
        _estado.Enviar(this);
    }

    public void Entregar()
    {
        _estado.Entregar(this);
    }

    public void Cancelar()
    {
        _estado.Cancelar(this);
    }

    // Lo llama el estado cuando la accion si esta permitida, para pasar al estado nuevo.
    public void CambiarEstado(IEstadoPedido nuevoEstado)
    {
        if (nuevoEstado == null) throw new ArgumentNullException(nameof(nuevoEstado));

        _estado = nuevoEstado;
    }

    // Foto del pedido, para mostrar en pantalla o guardar en el historial.
    public ResumenPedido ObtenerResumen()
    {
        return new ResumenPedido
        {
            Numero = Numero,
            Total = Total,
            Estado = _estado.Nombre,
            FechaHora = DateTime.Now
        };
    }
}
