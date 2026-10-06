namespace PedidoApp.Domain.Exceptions;

// Se lanza cuando se quiere hacer una accion que no corresponde al estado actual.
public class TransicionInvalidaException : Exception
{
    public TransicionInvalidaException(string estadoActual, string accion)
        : base("No se puede " + accion + " un pedido en estado " + estadoActual)
    {
        EstadoActual = estadoActual;
        Accion = accion;
    }

    public string EstadoActual { get; private set; }

    public string Accion { get; private set; }
}
