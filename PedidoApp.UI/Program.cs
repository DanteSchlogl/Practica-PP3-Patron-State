using PedidoApp.BLL;
using PedidoApp.Domain.Exceptions;

namespace PedidoApp.UI;

// Programa principal. Primero corre una demostracion sola y despues
// muestra un menu para probar los cambios de estado a mano.
internal static class Program
{
    private const string CarpetaHistoriales = "Historiales";

    private static int Main(string[] args)
    {
        Console.WriteLine("INSTITUTO UNIVERSITARIO LEONARDO DA VINCI");
        Console.WriteLine("Practicas Profesionalizantes III - Practica");
        Console.WriteLine("Patron State aplicado a la clase Pedido");
        Console.WriteLine("Alumno: Dante Schlogl");
        Console.WriteLine();

        // Con --demo corre solo la demostracion y sale, sin menu.
        bool soloDemo = false;

        foreach (string arg in args)
        {
            if (arg.ToLower() == "--demo") soloDemo = true;
        }

        ServicioPedido pedido = FabricaPedido.Crear(1, 1500, CarpetaHistoriales, FormatoHistorial.TextoPlano);

        Console.WriteLine("Carpeta de historiales: " + Path.GetFullPath(pedido.CarpetaHistoriales));

        Demostracion(pedido);

        if (!soloDemo)
        {
            Menu(pedido);
        }

        Resumen(pedido);

        return 0;
    }

    // Recorro los casos mas importantes: el camino normal y los errores.
    private static void Demostracion(ServicioPedido pedido)
    {
        Console.WriteLine();
        Console.WriteLine("=== Demostracion ===");
        MostrarEstado(pedido);

        Console.WriteLine();
        Console.WriteLine("Paso 1: confirmo el pedido.");
        pedido.Confirmar();
        MostrarEstado(pedido);

        Console.WriteLine();
        Console.WriteLine("Paso 2: lo envio.");
        pedido.Enviar();
        MostrarEstado(pedido);

        Console.WriteLine();
        Console.WriteLine("Paso 3: lo entrego.");
        pedido.Entregar();
        MostrarEstado(pedido);

        Console.WriteLine();
        Console.WriteLine("Paso 4: intento cancelarlo, pero ya esta entregado.");
        Intentar("cancelar", pedido);

        Console.WriteLine();
        Console.WriteLine("Paso 5: intento enviarlo de nuevo.");
        Intentar("enviar", pedido);

        Console.WriteLine();
        Console.WriteLine("=== Otro caso: cancelar un pedido ===");

        ServicioPedido otro = FabricaPedido.Crear(2, 2500, CarpetaHistoriales, FormatoHistorial.TextoPlano);
        MostrarEstado(otro);

        Console.WriteLine();
        Console.WriteLine("Cancelo el pedido 2 desde pendiente.");
        otro.Cancelar();
        MostrarEstado(otro);

        Console.WriteLine();
        Console.WriteLine("Ahora intento enviarlo, pero ya esta cancelado.");
        Intentar("enviar", otro);

        Console.WriteLine();
        Console.WriteLine("=== Otro caso: total invalido ===");

        try
        {
            FabricaPedido.Crear(3, 0, CarpetaHistoriales, FormatoHistorial.TextoPlano);
            Console.WriteLine("Salio bien.");
        }
        catch (TotalInvalidoException ex)
        {
            Console.WriteLine("Se controlo el error: " + ex.Message);
        }
    }

    // Pruebo una accion y muestro el error si el estado no la permite.
    private static void Intentar(string accion, ServicioPedido pedido)
    {
        try
        {
            if (accion == "confirmar")
            {
                pedido.Confirmar();
            }
            else if (accion == "enviar")
            {
                pedido.Enviar();
            }
            else if (accion == "entregar")
            {
                pedido.Entregar();
            }
            else
            {
                pedido.Cancelar();
            }

            Console.WriteLine("Salio bien.");
        }
        catch (TransicionInvalidaException ex)
        {
            Console.WriteLine("Se controlo el error: " + ex.Message);
        }
    }

    // Menu simple para probar a mano.
    private static void Menu(ServicioPedido pedido)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== Menu (pedido 1) ===");
            Console.WriteLine("1) Confirmar");
            Console.WriteLine("2) Enviar");
            Console.WriteLine("3) Entregar");
            Console.WriteLine("4) Cancelar");
            Console.WriteLine("5) Ver el estado actual");
            Console.WriteLine("6) Ver el historial");
            Console.WriteLine("0) Salir");
            Console.Write("Opcion: ");

            string? opcion = Console.ReadLine();

            // Si ya no hay entrada (por ejemplo al correrlo por un pipe) salgo sin error.
            if (opcion == null)
            {
                Console.WriteLine();
                Console.WriteLine("No hay mas entrada, salgo.");
                return;
            }

            opcion = opcion.Trim();

            if (opcion == "0") return;

            Console.WriteLine();

            try
            {
                if (opcion == "1")
                {
                    pedido.Confirmar();
                    Console.WriteLine("Listo.");
                }
                else if (opcion == "2")
                {
                    pedido.Enviar();
                    Console.WriteLine("Listo.");
                }
                else if (opcion == "3")
                {
                    pedido.Entregar();
                    Console.WriteLine("Listo.");
                }
                else if (opcion == "4")
                {
                    pedido.Cancelar();
                    Console.WriteLine("Listo.");
                }
                else if (opcion == "5")
                {
                    MostrarEstado(pedido);
                }
                else if (opcion == "6")
                {
                    MostrarHistorial(pedido);
                }
                else
                {
                    Console.WriteLine("Opcion invalida.");
                }
            }
            catch (TransicionInvalidaException ex)
            {
                Console.WriteLine("No se pudo: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }

    // Muestro el estado del pedido y que acciones se pueden hacer ahora.
    private static void MostrarEstado(ServicioPedido pedido)
    {
        Console.WriteLine();
        Console.WriteLine("Pedido " + pedido.Numero + " - Total $" + pedido.Total);
        Console.WriteLine("Estado: " + pedido.EstadoActual);

        if (pedido.AccionesPermitidas.Count == 0)
        {
            Console.WriteLine("Acciones permitidas: ninguna");
        }
        else
        {
            Console.WriteLine("Acciones permitidas: " + string.Join(", ", pedido.AccionesPermitidas));
        }
    }

    // Muestro el historial guardado en el archivo.
    private static void MostrarHistorial(ServicioPedido pedido)
    {
        Console.WriteLine("Archivo: " + Path.GetFullPath(pedido.ObtenerRutaHistorial()));

        IReadOnlyList<string> lineas = pedido.LeerHistorial();

        if (lineas.Count == 0)
        {
            Console.WriteLine("Todavia no hay cambios.");
            return;
        }

        foreach (string linea in lineas)
        {
            Console.WriteLine(linea);
        }
    }

    // Muestro el historial y el estado final.
    private static void Resumen(ServicioPedido pedido)
    {
        Console.WriteLine();
        Console.WriteLine("=== Historial del pedido 1 ===");
        MostrarHistorial(pedido);

        Console.WriteLine();
        Console.WriteLine("=== Resumen ===");
        Console.WriteLine("Estado final del pedido 1: " + pedido.EstadoActual);
        Console.WriteLine("Cambios guardados: " + pedido.EventosRegistrados);
        Console.WriteLine("Fin del programa.");
    }
}
