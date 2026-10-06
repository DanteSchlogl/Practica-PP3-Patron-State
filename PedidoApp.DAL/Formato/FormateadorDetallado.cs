using System.Text;
using PedidoApp.Domain;

namespace PedidoApp.DAL.Formato;

// Otro formato posible: cada cambio ocupa varios renglones.
public class FormateadorDetallado : IFormateadorHistorial
{
    public string Nombre
    {
        get { return "Detallado"; }
    }

    public string Formatear(ResumenPedido resumen)
    {
        StringBuilder texto = new StringBuilder();

        texto.AppendLine("--- cambio de estado ---");
        texto.AppendLine("Fecha: " + resumen.FechaHora.ToString("dd/MM/yyyy HH:mm:ss"));
        texto.AppendLine("Pedido: " + resumen.Numero);
        texto.AppendLine("Total: $" + resumen.Total);
        texto.AppendLine("Estado: " + resumen.Estado);

        return texto.ToString();
    }
}
