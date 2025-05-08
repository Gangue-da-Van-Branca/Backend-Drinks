using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EloDrinksAPI.DTOs.item;
using EloDrinksAPI.Models;

namespace EloDrinksAPI.Mappers
{
    public class ItemMapper
    {
        public static ItemResponseDto ToDTO(Item item)
        {
            return new ItemResponseDto
            {
                IdItem = item.IdItem,
                Nome = item.Nome,
                Descricao = item.Descricao,
                Preco = item.Preco,
                Tipo = item.Tipo
            };
        }

        public static Item ToEntity(CreateItemDto dto)
        {
            return new Item
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                Preco = dto.Preco,
                Tipo = dto.Tipo
            };
        }

        public static void ApplyUpdate(UpdateItemDto dto, Item item)
        {
            item.Nome = dto.Nome;
            item.Descricao = dto.Descricao;
            item.Preco = dto.Preco;
            item.Tipo = dto.Tipo;
        }
    }
}