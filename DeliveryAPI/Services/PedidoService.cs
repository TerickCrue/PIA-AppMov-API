﻿using DeliveryAPI.Data;
using DeliveryAPI.Data.Models;
using DeliveryAPI.Data.DTOs;
using Microsoft.EntityFrameworkCore;


namespace DeliveryAPI.Services;

public class PedidoService
{
    private readonly PiaAppMovContext _context;
    private readonly CarritoService _carritoService;

    public PedidoService(PiaAppMovContext context, CarritoService carritoService)
    {
        _context = context;
        _carritoService = carritoService;
    }

    public async Task<IEnumerable<PedidoDtoOut>> GetAll()
    {
        return await _context.Pedidos.Select(p => new PedidoDtoOut {
            Id = p.Id,
            UserId = p.User.Id,
            NombreUsuario = p.User.Nombre,
            BusinessId = p.BusinessId,
            NombreNegocio = p.Business.Nombre,
            DireccionEntrega = p.DireccionEntrega,
            MetodopagoId = p.MetodopagoId,
            MetodoPago = p.Metodopago.Metodo,
            CarritoId = p.CarritoId,
            Total = p.Total,
            Fecha = p.Fecha,
            Status = p.Status,
       
        }).ToListAsync();
    }


    public async Task<Pedido?> GetById(int id)
    {
        return await _context.Pedidos.FindAsync(id);
    }

    public async Task<PedidoDtoOut?> GetDtoById(int id)
    {
        return await _context.Pedidos.
            Where(p => p.Id == id).
            Select(p => new PedidoDtoOut
            {
                Id = p.Id,
                UserId = p.User.Id,
                NombreUsuario = p.User.Nombre,
                BusinessId = p.BusinessId,
                NombreNegocio = p.Business.Nombre,
                DireccionEntrega = p.DireccionEntrega,
                MetodopagoId = p.MetodopagoId,
                MetodoPago = p.Metodopago.Metodo,
                CarritoId = p.CarritoId,
                Total = p.Total,
                Fecha = p.Fecha,
                Status = p.Status

            }).SingleOrDefaultAsync();

    }

    public async Task<IEnumerable<PedidoDtoOut>> GetByNegocioId(int negocioId)
    {
        return await _context.Pedidos.Where(p => p.BusinessId == negocioId).
            Select(p => new PedidoDtoOut
            {
                Id = p.Id,
                UserId = p.User.Id,
                NombreUsuario = p.User.Nombre,
                BusinessId = p.BusinessId,
                NombreNegocio = p.Business.Nombre,
                DireccionEntrega = p.DireccionEntrega,
                MetodopagoId = p.MetodopagoId,
                MetodoPago = p.Metodopago.Metodo,
                CarritoId = p.CarritoId,
                Total = p.Total,
                Fecha = p.Fecha,
                Status = p.Status

            }).ToListAsync();
    }

    public async Task<IEnumerable<PedidoDtoOut>> GetByUserId(int userId)
    {
        return await _context.Pedidos.Where(p => p.UserId == userId).
            Select(p => new PedidoDtoOut
            {
                Id = p.Id,
                UserId = p.User.Id,
                NombreUsuario = p.User.Nombre,
                BusinessId = p.BusinessId,
                NombreNegocio = p.Business.Nombre,
                DireccionEntrega = p.DireccionEntrega,
                MetodopagoId = p.MetodopagoId,
                MetodoPago = p.Metodopago.Metodo,
                CarritoId = p.CarritoId,
                Total = p.Total,
                Fecha = p.Fecha,
                Status = p.Status

            }).ToListAsync();
    }

    public async Task<Pedido> Create(PedidoDtoIn nuevoPedidoDto)
    {
        var nuevoPedido = new Pedido();
        var total = await CalcularTotal(nuevoPedidoDto.CarritoId);

        nuevoPedido.UserId = nuevoPedidoDto.UserId;
        nuevoPedido.BusinessId = nuevoPedidoDto.BusinessId;
        nuevoPedido.DireccionEntrega = nuevoPedidoDto.DireccionEntrega;
        nuevoPedido.Total = total;
        nuevoPedido.MetodopagoId = nuevoPedidoDto.MetodopagoId;
        nuevoPedido.Status = "pendiente";
        nuevoPedido.CarritoId = nuevoPedidoDto.CarritoId;


        _context.Pedidos.Add(nuevoPedido);
        await _context.SaveChangesAsync();
        return nuevoPedido;
    }

    public async Task Update(int id, PedidoDtoIn pedido)
    {
        var pedidoExistente = await GetById(id);

        if (pedidoExistente is not null)
        {
            pedidoExistente.DireccionEntrega = pedido.DireccionEntrega;
            pedidoExistente.MetodopagoId = pedido.MetodopagoId;
            pedidoExistente.Status = pedido.Status;


            await _context.SaveChangesAsync();
        }

    }

    public async Task Delete(int id)
    {
        var orderToDelete = await GetById(id);

        if (orderToDelete is not null)
        {
            _context.Pedidos.Remove(orderToDelete);

            await _context.SaveChangesAsync();
        }
    }

    public async Task<decimal> CalcularTotal(int carritoId)
    {
        var carrito = await _carritoService.GetById(carritoId);

        if(carrito is not null)
        {
            var total = await _context.Carritoproductos.Where(cp => cp.CartId == carrito.Id).SumAsync(cp => cp.PrecioSubtotal) ?? 0;
            return total;
        }
        else
            return 0;

    }

}
