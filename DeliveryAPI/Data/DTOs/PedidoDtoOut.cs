namespace DeliveryAPI.Data.DTOs
{
    public class PedidoDtoOut
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string? NombreUsuario { get; set; }

        public int BusinessId { get; set; }

        public string? NombreNegocio { get; set; }

        public string? DireccionEntrega { get; set; }

        public decimal Total { get; set; }

        public int MetodopagoId { get; set; }

        public string? MetodoPago { get; set; }

        public int CarritoId { get; set; }

        public DateTime? Fecha { get; set; }

        public string Status { get; set; } = null!;
    }
}
