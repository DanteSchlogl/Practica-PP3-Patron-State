using PedidoApp.Domain;

namespace PedidoApp.DAL.Formato;

// Formato simple: un renglon por cambio de estado.
public class FormateadorTextoPlano : IFormateadorHistorial
{
    public string Nombre
    {
        get { return "Texto plano"; }
    }

    public string Formatear(ResumenPedido resumen)
    {
        return "[" + resumen.FechaHora.ToString("dd/MM/yyyy HH:mm:ss") + "] "
            + "Pedido " + resumen.Numero
            + " - $" + resumen.Total
            + " - " + resumen.Estado;
    }
}
