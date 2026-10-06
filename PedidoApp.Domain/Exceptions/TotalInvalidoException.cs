namespace PedidoApp.Domain.Exceptions;

// Se lanza si el total del pedido no es mayor a cero.
public class TotalInvalidoException : Exception
{
    public TotalInvalidoException(decimal total)
        : base("El total del pedido tiene que ser mayor a 0. Me llego: " + total)
    {
        Total = total;
    }

    public decimal Total { get; private set; }
}
