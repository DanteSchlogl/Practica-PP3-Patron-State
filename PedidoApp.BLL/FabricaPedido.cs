using PedidoApp.DAL;
using PedidoApp.DAL.Formato;

namespace PedidoApp.BLL;

// Los formatos de historial que puede elegir la pantalla.
public enum FormatoHistorial
{
    TextoPlano,
    Detallado
}

// Fabrica que arma todo junto: el repositorio, el formato y el servicio.
// Asi la pantalla pide "un servicio" y no necesita conocer la capa de datos.
public static class FabricaPedido
{
    public static ServicioPedido Crear(int numero, decimal total, string carpetaHistoriales, FormatoHistorial formato)
    {
        IFormateadorHistorial formateador;

        if (formato == FormatoHistorial.Detallado)
        {
            formateador = new FormateadorDetallado();
        }
        else
        {
            formateador = new FormateadorTextoPlano();
        }

        RepositorioHistorialArchivo repositorio = new RepositorioHistorialArchivo(carpetaHistoriales, formateador);

        return new ServicioPedido(repositorio, numero, total);
    }
}
