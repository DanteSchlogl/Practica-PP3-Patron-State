namespace PedidoApp.Domain.Estados;

// El pedido ya esta confirmado. Se puede enviar o todavia cancelar.
public class EstadoConfirmado : EstadoPedidoBase
{
    public override string Nombre
    {
        get { return "Confirmado"; }
    }

    public override IReadOnlyList<string> AccionesPermitidas
    {
        get { return new List<string> { "enviar", "cancelar" }; }
    }

    public override void Enviar(Pedido pedido)
    {
        pedido.CambiarEstado(new EstadoEnviado());
    }

    public override void Cancelar(Pedido pedido)
    {
        pedido.CambiarEstado(new EstadoCancelado());
    }
}
