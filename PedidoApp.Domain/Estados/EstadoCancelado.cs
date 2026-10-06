namespace PedidoApp.Domain.Estados;

// Estado final. El pedido se cancelo y no se puede hacer nada mas.
public class EstadoCancelado : EstadoPedidoBase
{
    public override string Nombre
    {
        get { return "Cancelado"; }
    }

    public override IReadOnlyList<string> AccionesPermitidas
    {
        get { return new List<string>(); }
    }
}
