using EloDrinksAPI.DTOs.orcamento;
using EloDrinksAPI.Models;

namespace EloDrinksAPI.Mappers
{
    public class OrcamentoMapper
    {
        public static Orcamento ToEntity(CreateOrcamentoDto dto)
        {
            return new Orcamento
            {
                Data = dto.Data,
                Cep = dto.Cep,
                HoraInicio = dto.HoraInicio,
                HoraFim = dto.HoraFim,
                QtdPessoas = dto.QtdPessoas,
                Preco = dto.Preco,
                Status = dto.Status,
                TipoEvento = dto.TipoEvento,
                UsuarioIdUsuario = dto.UsuarioIdUsuario
            };
        }

        public static void ApplyUpdate(UpdateOrcamentoDto dto, Orcamento entity)
        {
            entity.Data = dto.Data;
            entity.Cep = dto.Cep;
            entity.HoraInicio = dto.HoraInicio;
            entity.HoraFim = dto.HoraFim;
            entity.QtdPessoas = dto.QtdPessoas;
            entity.Preco = dto.Preco;
            entity.Status = dto.Status;
            entity.TipoEvento = dto.TipoEvento;
        }

        public static OrcamentoResponseDto ToDTO(Orcamento entity)
        {
            return new OrcamentoResponseDto
            {
                IdOrcamento = entity.IdOrcamento,
                Data = entity.Data,
                Cep = entity.Cep,
                HoraInicio = entity.HoraInicio,
                HoraFim = entity.HoraFim,
                QtdPessoas = entity.QtdPessoas,
                Preco = entity.Preco,
                Status = entity.Status,
                TipoEvento = entity.TipoEvento,
                UsuarioIdUsuario = entity.UsuarioIdUsuario,
                NomeUsuario = entity.UsuarioIdUsuarioNavigation?.Nome // se incluído no Include
            };
        }
    }
}