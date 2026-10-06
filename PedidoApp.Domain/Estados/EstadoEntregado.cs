namespace PedidoApp.Domain.Estados;

// Estado final. El pedido llego y no se puede hacer nada mas.
public class EstadoEntregado : EstadoPedidoBase
{
    public override string Nombre
    {
        get { return "Entregado"; }
    }

    public override IReadOnlyList<string> AccionesPermitidas
    {
        get { return new List<string>(); }
    }
}
