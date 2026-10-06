using PedidoApp.Domain;

namespace PedidoApp.DAL;

// Lo que necesita la capa de negocio para guardar el historial de un pedido.
// No dice como ni donde se guarda, eso lo decide la clase que lo implementa.
public interface IRepositorioHistorial
{
    string Carpeta { get; }

    int EventosRegistrados { get; }

    // Me dice la ruta del archivo que le toca a ese pedido.
    string ObtenerRutaArchivo(int numeroPedido);

    // Guarda un cambio de estado.
    void Registrar(ResumenPedido resumen);

    // Devuelve las lineas del historial de un pedido.
    IReadOnlyList<string> LeerHistorial(int numeroPedido);
}
