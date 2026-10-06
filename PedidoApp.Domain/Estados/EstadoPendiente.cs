namespace PedidoApp.Domain.Estados;

// Estado inicial. Desde aca se puede confirmar o cancelar.
public class EstadoPendiente : EstadoPedidoBase
{
    public override string Nombre
    {
        get { return "Pendiente"; }
    }

    public override IReadOnlyList<string> AccionesPermitidas
    {
        get { return new List<string> { "confirmar", "cancelar" }; }
    }

    public override void Confirmar(Pedido pedido)
    {
        pedido.CambiarEstado(new EstadoConfirmado());
    }

    public override void Cancelar(Pedido pedido)
    {
        pedido.CambiarEstado(new EstadoCancelado());
    }
}
