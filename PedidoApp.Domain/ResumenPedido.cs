namespace PedidoApp.Domain;

// Datos del pedido en un momento dado, para mostrar en pantalla o guardar.
public class ResumenPedido
{
    public int Numero { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = "";
    public DateTime FechaHora { get; set; }
}
