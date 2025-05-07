using EloDrinksAPI.DTOs.usuario;
using EloDrinksAPI.Models;

public static class UsuarioMapper
{
    public static Usuario ToEntity(CreateUsuarioDto dto)
    {
        return new Usuario
        {
            Nome = dto.Nome,
            Sobrenome = dto.Sobrenome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            Senha = dto.Senha,
            DataCadastro = DateOnly.FromDateTime(DateTime.Today),
            Tipo = dto.Tipo
        };
    }

    public static UsuarioResponseDto ToDTO(Usuario usuario)
    {
        return new UsuarioResponseDto
        {
            IdUsuario = usuario.IdUsuario,
            Nome = usuario.Nome,
            Sobrenome = usuario.Sobrenome,
            Email = usuario.Email,
            Telefone = usuario.Telefone,
            DataCadastro = usuario.DataCadastro,
            Tipo = usuario.Tipo,
        };
    }

    public static void ApplyUpdate(UpdateUsuarioDto dto, Usuario usuario)
    {
        if (dto.Nome != null) usuario.Nome = dto.Nome;
        if (dto.Sobrenome != null) usuario.Sobrenome = dto.Sobrenome;
        if (dto.Email != null) usuario.Email = dto.Email;
        if (dto.Telefone != null) usuario.Telefone = dto.Telefone;
        // if (dto.Senha != null) usuario.Senha = dto.Senha;
        if (dto.Tipo != null) usuario.Tipo = dto.Tipo;
    }
}
