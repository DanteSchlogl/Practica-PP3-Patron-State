namespace PedidoApp.Domain.Estados;

// El pedido ya salio. Solo queda entregarlo: ya no se puede cancelar.
public class EstadoEnviado : EstadoPedidoBase
{
    public override string Nombre
    {
        get { return "Enviado"; }
    }

    public override IReadOnlyList<string> AccionesPermitidas
    {
        get { return new List<string> { "entregar" }; }
    }

    public override void Entregar(Pedido pedido)
    {
        pedido.CambiarEstado(new EstadoEntregado());
    }
}
