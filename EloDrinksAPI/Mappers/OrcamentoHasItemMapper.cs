using EloDrinksAPI.DTOs.orcamentoHasItem;
using EloDrinksAPI.Models;

namespace EloDrinksAPI.Mappers
{
    public class OrcamentoHasItemMapper
    {
        public static OrcamentoHasItem ToEntity(CreateOrcamentoHasItemDto dto)
        {
            return new OrcamentoHasItem
            {
                OrcamentoIdOrcamento = dto.OrcamentoIdOrcamento,
                OrcamentoUsuarioIdUsuario = dto.OrcamentoUsuarioIdUsuario,
                ItemIdItem = dto.ItemIdItem,
                Quantidade = dto.Quantidade
            };
        }

        public static OrcamentoHasItemResponseDto ToDTO(OrcamentoHasItem entity)
        {
            return new OrcamentoHasItemResponseDto
            {
                OrcamentoIdOrcamento = entity.OrcamentoIdOrcamento,
                OrcamentoUsuarioIdUsuario = entity.OrcamentoUsuarioIdUsuario,
                ItemIdItem = entity.ItemIdItem,
                Quantidade = entity.Quantidade,
                NomeItem = entity.ItemIdItemNavigation?.Nome ?? string.Empty
            };
        }

        public static void ApplyUpdate(UpdateOrcamentoHasItemDto dto, OrcamentoHasItem entity)
        {
            if (dto.Quantidade.HasValue)
            {
                entity.Quantidade = dto.Quantidade.Value;
            }
        }
    }
}