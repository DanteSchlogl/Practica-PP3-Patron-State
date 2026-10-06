namespace PedidoApp.Domain;

// Lo que sabe hacer un estado del pedido.
public interface IEstadoPedido
{
    // El nombre del estado, por ejemplo "Pendiente".
    string Nombre { get; }

    // Las acciones que se pueden hacer estando en este estado.
    IReadOnlyList<string> AccionesPermitidas { get; }

    void Confirmar(Pedido pedido);
    void Enviar(Pedido pedido);
    void Entregar(Pedido pedido);
    void Cancelar(Pedido pedido);
}
