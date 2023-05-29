using DeliveryAPI.Data;
using DeliveryAPI.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using BC = BCrypt.Net.BCrypt;


namespace DeliveryAPI.Services;

public class UsuarioService
{
    private readonly PiaAppMovContext _context;

    public UsuarioService(PiaAppMovContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Usuario>> GetAll()
    {
        return await _context.Usuarios.ToListAsync();
    }

    public async Task<Usuario?> GetById(int id)
    {
        return await _context.Usuarios.FindAsync(id);
    }

    public async Task<Usuario?> GetByEmail(string email)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task <Usuario> Create(Usuario nuevoUsuario)
    {
        var usuario = new Usuario();
        usuario = nuevoUsuario;
        usuario.Contraseña = BC.HashPassword(nuevoUsuario.Contraseña);

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return nuevoUsuario;
    }

    public async Task Update(int id, Usuario usuario)
    {
        var existingUser = await GetById(id);

        if (existingUser is not null){
            existingUser.Nombre = usuario.Nombre;
            existingUser.Email = usuario.Email;
            existingUser.Telefono = usuario.Telefono;
            existingUser.Facultad = usuario.Facultad;
            existingUser.FotoPerfilUrl = usuario.FotoPerfilUrl;

            await _context.SaveChangesAsync();

        }
            
    }

    public async Task Delete(int id)
    {
        var userToDelete = await GetById(id);

        if (userToDelete is not null)
        {
            _context.Usuarios.Remove(userToDelete);
            await _context.SaveChangesAsync();
        }
    }






}

