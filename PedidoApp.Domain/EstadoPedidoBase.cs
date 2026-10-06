using PedidoApp.Domain.Exceptions;

namespace PedidoApp.Domain;

// Base de todos los estados. Las acciones que no van tiran error,
// asi cada estado solo escribe las transiciones que si permite.
public abstract class EstadoPedidoBase : IEstadoPedido
{
    public abstract string Nombre { get; }

    public abstract IReadOnlyList<string> AccionesPermitidas { get; }

    public virtual void Confirmar(Pedido pedido)
    {
        throw new TransicionInvalidaException(Nombre, "confirmar");
    }

    public virtual void Enviar(Pedido pedido)
    {
        throw new TransicionInvalidaException(Nombre, "enviar");
    }

    public virtual void Entregar(Pedido pedido)
    {
        throw new TransicionInvalidaException(Nombre, "entregar");
    }

    public virtual void Cancelar(Pedido pedido)
    {
        throw new TransicionInvalidaException(Nombre, "cancelar");
    }
}
