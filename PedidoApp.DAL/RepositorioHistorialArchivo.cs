using PedidoApp.DAL.Excepciones;
using PedidoApp.DAL.Formato;
using PedidoApp.Domain;

namespace PedidoApp.DAL;

// Guarda el historial en un archivo de texto por pedido: historial_pedido_1.txt
public class RepositorioHistorialArchivo : IRepositorioHistorial
{
    private readonly IFormateadorHistorial _formateador;

    // Para que dos escrituras al mismo tiempo no rompan el archivo.
    private readonly object _candado = new object();

    public RepositorioHistorialArchivo(string carpeta, IFormateadorHistorial formateador)
    {
        if (string.IsNullOrWhiteSpace(carpeta))
        {
            Carpeta = "Historiales";
        }
        else
        {
            Carpeta = carpeta;
        }

        if (formateador == null)
        {
            _formateador = new FormateadorTextoPlano();
        }
        else
        {
            _formateador = formateador;
        }
    }

    public string Carpeta { get; private set; }

    public int EventosRegistrados { get; private set; }

    // Armo el nombre del archivo con el numero de pedido.
    public string ObtenerRutaArchivo(int numeroPedido)
    {
        return Path.Combine(Carpeta, "historial_pedido_" + numeroPedido + ".txt");
    }

    public void Registrar(ResumenPedido resumen)
    {
        if (resumen == null) throw new ArgumentNullException(nameof(resumen));

        string ruta = ObtenerRutaArchivo(resumen.Numero);
        string linea = _formateador.Formatear(resumen) + Environment.NewLine;

        try
        {
            lock (_candado)
            {
                Directory.CreateDirectory(Carpeta);  // la creo si no existe
                File.AppendAllText(ruta, linea);     // agrego al final, no piso lo anterior
            }

            EventosRegistrados++;
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
        {
            throw new HistorialException("No pude escribir el historial en " + Path.GetFullPath(ruta), ruta, ex);
        }
    }

    public IReadOnlyList<string> LeerHistorial(int numeroPedido)
    {
        string ruta = ObtenerRutaArchivo(numeroPedido);

        try
        {
            if (!File.Exists(ruta))
            {
                return new List<string>();
            }

            return File.ReadAllLines(ruta);
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
        {
            throw new HistorialException("No pude leer el historial de " + Path.GetFullPath(ruta), ruta, ex);
        }
    }
}
