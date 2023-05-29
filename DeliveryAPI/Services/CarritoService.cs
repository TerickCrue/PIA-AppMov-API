using DeliveryAPI.Data.DTOs;
using DeliveryAPI.Data.Models;
using DeliveryAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace DeliveryAPI.Services;

public class CarritoService
{


    private readonly PiaAppMovContext _context;
    private readonly ProductoService _productoService;

    public CarritoService(PiaAppMovContext context, ProductoService productoService)
    {
        _context = context;
        _productoService = productoService;
    }

    public async Task<IEnumerable<Carrito>> GetAll()
    {
        return await _context.Carritos.ToListAsync();
    }


    public async Task<IEnumerable<Carrito>> GetCarritosSinPedido(int usuarioId)
    {
        var carritos = await _context.Carritos
            .Where(c => c.UserId == usuarioId)
            .ToListAsync();

        var pedidos = await _context.Pedidos.ToListAsync();

        var carritosSinPedido = carritos.Where(c => !pedidos.Any(p => p.CarritoId == c.Id)).ToList();

        return carritosSinPedido;
    }


    public async Task<Carrito?> GetById(int id)
    {
        return await _context.Carritos.FindAsync(id);
    }


    public async Task<IEnumerable<Carrito>> GetByNegocioId(int negocioId)
    {
        return await _context.Carritos.Where(p => p.BusinessId == negocioId).ToListAsync();
    }


    public async Task<IEnumerable<Carrito>> GetByUserId(int userId)
    {
        return await _context.Carritos.Where(p => p.UserId == userId).ToListAsync();
    }


    public async Task<IEnumerable<Carritoproducto>> GetProductosEnCarrito(int carritoId)
    {
        var productos = await _context.Carritoproductos
            .Where(cp => cp.CartId == carritoId)
            .ToListAsync();

        return productos;
    }


    public async Task<IEnumerable<CarritoProductoDtoOut>> GetProductosDtoEnCarrito(int carritoId)
    {
        var productos = await _context.Carritoproductos
            .Where(cp => cp.CartId == carritoId)
            .Select(cp => new CarritoProductoDtoOut
            {
                CartId = cp.CartId,
                ProductoId = cp.ProductId,
                NombreProducto = cp.Product.Nombre,
                Cantidad = cp.Cantidad,
                PrecioSubtotal = cp.PrecioSubtotal
            })
            .ToListAsync();

        return productos;
    }


    public async Task<Carrito> Create(CarritoDtoIn nuevoCarritoDto)
    {
        var nuevoCarrito = new Carrito();

        nuevoCarrito.UserId = nuevoCarritoDto.UserId;
        nuevoCarrito.BusinessId = nuevoCarritoDto.BusinessId;

        _context.Carritos.Add(nuevoCarrito);
        await _context.SaveChangesAsync();

        return nuevoCarrito;
    }


    public async Task Update(int id, CarritoDtoIn carrito)
    {
        var carritoExistente = await GetById(id);

        if (carritoExistente is not null)
        {
            carritoExistente.UserId = carrito.UserId;
            carritoExistente.BusinessId = carrito.BusinessId;

            await _context.SaveChangesAsync();
        }

    }


    public async Task Delete(int id)
    {
        var cartToDelete = await GetById(id);

        if (cartToDelete is not null)
        {
            _context.Carritos.Remove(cartToDelete);

            await _context.SaveChangesAsync();
        }
    }


    public async Task <Carritoproducto>AgregarProductoACarrito(int userId, int negocioId, CarritoProductoDtoIn productoIngresado)
    {

        var carrito = await ExisteCarrito(userId, negocioId);
        var carritoId = 0;

        if (carrito is null) {
            
            var nuevoCarrito = new CarritoDtoIn();

            nuevoCarrito.UserId = userId;
            nuevoCarrito.BusinessId = negocioId;
            var carritoCreado = await Create(nuevoCarrito);

            carritoId = carritoCreado.Id;

        }
        else
        {
            carritoId = carrito.Id;
        }

        var carritoProducto = new Carritoproducto();

        var producto = await _productoService.GetById(productoIngresado.ProductoId);

        var subtotal = producto.Precio * productoIngresado.Cantidad;
        
        carritoProducto.CartId = carritoId;
        carritoProducto.ProductId = productoIngresado.ProductoId;
        carritoProducto.Cantidad = productoIngresado.Cantidad;
        carritoProducto.PrecioSubtotal = subtotal;


        _context.Carritoproductos.Add(carritoProducto);
        await _context.SaveChangesAsync();

        return carritoProducto;
    }

    public async Task EliminarProductoDeCarrito(int carritoId, int productoId)
    {

        var carritoProducto = _context.Carritoproductos
            .FirstOrDefault(cp => cp.ProductId == productoId && cp.CartId == carritoId);

        if (carritoProducto is not null)
        {
            _context.Carritoproductos.Remove(carritoProducto);
            await _context.SaveChangesAsync();
        }

    }

    public async Task ModificarCantidadDeProducto(CarritoProductoDtoIn carritoProductoDto)
    {

        var carritoProducto = _context.Carritoproductos
            .FirstOrDefault(cp => cp.ProductId == carritoProductoDto.ProductoId);

        var producto = await _productoService.GetById(carritoProductoDto.ProductoId);
        var precio = producto.Precio;

        if (carritoProducto is not null)
        {
            carritoProducto.Cantidad = carritoProductoDto.Cantidad;
            carritoProducto.PrecioSubtotal = carritoProductoDto.Cantidad * precio;
            await _context.SaveChangesAsync();
        }

    }




    public async Task<Carrito> ExisteCarrito(int usuarioId, int negocioId)
    {
        // Buscar el carrito correspondiente en la base de datos de forma asíncrona
        Carrito carrito = await _context.Carritos.FirstOrDefaultAsync(c => c.UserId == usuarioId && c.BusinessId == negocioId);

        // Devolver el carrito encontrado (puede ser null si no se encuentra)
        return carrito;
    }
}
