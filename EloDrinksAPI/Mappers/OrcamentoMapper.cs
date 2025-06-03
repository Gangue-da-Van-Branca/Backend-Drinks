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
                IdOrcamento = dto.IdOrcamento,
                Data = dto.Data,
                Cep = dto.Cep,
                HoraInicio = dto.HoraInicio,
                HoraFim = dto.HoraFim,
                QtdPessoas = dto.QtdPessoas,
                Preco = dto.Preco,
                Status = dto.Status,
                TipoEvento = dto.TipoEvento,
                UsuarioIdUsuario = dto.UsuarioIdUsuario,
                Endereco = dto.Endereco,
                DrinksSelecionados = dto.DrinksSelecionados
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
                IdUsuario = entity.UsuarioIdUsuario,
                BaseFesta = new BaseFestaDto
                {
                    TipoFesta = entity.TipoEvento,
                    DrinksSelecionados = entity.OrcamentoHasItems
                        .Where(i => i.ItemIdItemNavigation.Tipo == "Drink Alcoólico" || i.ItemIdItemNavigation.Tipo == "Soft Drink")
                        .Select(i => new DrinkDto
                        {
                            Id = i.ItemIdItem,
                            Nome = i.ItemIdItemNavigation.Nome,
                            Descricao = i.ItemIdItemNavigation.Descricao
                        }).ToList()
                },
                InfosContratante = new InfosContratanteDto
                {
                    Nome = entity.UsuarioIdUsuarioNavigation.Nome,
                    Sobrenome = entity.UsuarioIdUsuarioNavigation.Sobrenome,
                    Telefone = entity.UsuarioIdUsuarioNavigation.Telefone,
                    Email = entity.UsuarioIdUsuarioNavigation.Email,
                    Data = entity.Data.ToString("dd/MM/yyyy"),
                    Endereco = entity.Endereco ?? "",
                    HorarioInicio = entity.HoraInicio.ToString("HH:mm"),
                    HorarioFinal = entity.HoraFim.ToString("HH:mm"),
                    Cep = entity.Cep.ToString(),
                    Convidados = entity.QtdPessoas.ToString()
                },
                Opcionais = new OpcionaisDto
                {
                    Shots = entity.OrcamentoHasItems
                        .Where(i => i.ItemIdItemNavigation.Tipo == "Shot")
                        .ToDictionary(i => i.ItemIdItemNavigation.Nome, i => i.Quantidade),
                    Extras = entity.OrcamentoHasItems
                        .Where(i => i.ItemIdItemNavigation.Tipo == "Opcional")
                        .ToDictionary(i => i.ItemIdItemNavigation.Nome, i => i.Quantidade),
                    BaresAdicionais = entity.OrcamentoHasItems
                        .Where(i => i.ItemIdItemNavigation.Tipo == "Bar")
                        .Select(i => i.ItemIdItemNavigation.Nome)
                        .ToList()
                },

            };
        }

        public static OrcamentoFrontInputDto ToFrontendDTO(Orcamento orcamento)
        {
            var drinks = orcamento.OrcamentoHasItems
                .Where(i => i.ItemIdItemNavigation.Tipo == "drink")
                .Select(i => new DrinkDto
                {
                    Id = i.ItemIdItem,
                    Nome = i.ItemIdItemNavigation.Nome,
                    Descricao = i.ItemIdItemNavigation.Descricao
                }).ToList();

            var shots = orcamento.OrcamentoHasItems
                .Where(i => i.ItemIdItemNavigation.Tipo == "shot")
                .ToDictionary(i => i.ItemIdItemNavigation.Nome, i => i.Quantidade);

            var extras = orcamento.OrcamentoHasItems
                .Where(i => i.ItemIdItemNavigation.Tipo == "extra")
                .ToDictionary(i => i.ItemIdItemNavigation.Nome, i => i.Quantidade);

            var bares = orcamento.OrcamentoHasItems
                .Where(i => i.ItemIdItemNavigation.Tipo == "barAdicional")
                .Select(i => i.ItemIdItemNavigation.Nome)
                .ToList();

            return new OrcamentoFrontInputDto
            {
                BaseFesta = new BaseFestaDto
                {
                    TipoFesta = orcamento.TipoEvento,
                    DrinksSelecionados = drinks
                },
                InfosContratante = new InfosContratanteDto
                {
                    Nome = orcamento.UsuarioIdUsuarioNavigation.Nome,
                    Sobrenome = orcamento.UsuarioIdUsuarioNavigation.Sobrenome,
                    Telefone = orcamento.UsuarioIdUsuarioNavigation.Telefone,
                    Email = orcamento.UsuarioIdUsuarioNavigation.Email,
                    Data = orcamento.Data.ToString("dd/MM/yyyy"),
                    Endereco = orcamento.Endereco ?? "",
                    HorarioInicio = orcamento.HoraInicio.ToString("HH:mm"),
                    HorarioFinal = orcamento.HoraFim.ToString("HH:mm"),
                    Cep = orcamento.Cep.ToString(),
                    Convidados = orcamento.QtdPessoas.ToString()
                },
                Opcionais = new OpcionaisDto
                {
                    Shots = shots,
                    Extras = extras,
                    BaresAdicionais = bares
                },
                Preco = orcamento.Preco
            };
        }

    }
}