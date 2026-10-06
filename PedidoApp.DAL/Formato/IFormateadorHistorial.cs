using PedidoApp.Domain;

namespace PedidoApp.DAL.Formato;

// Formas de escribir un cambio de estado. El repositorio usa una de estas.
// Si quiero otro formato, agrego otra clase aca y no toco el repositorio.
public interface IFormateadorHistorial
{
    string Nombre { get; }

    string Formatear(ResumenPedido resumen);
}
