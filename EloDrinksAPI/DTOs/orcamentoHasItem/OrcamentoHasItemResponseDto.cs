namespace EloDrinksAPI.DTOs.orcamentoHasItem
{
    public class OrcamentoHasItemResponseDto
    {
        public string OrcamentoIdOrcamento { get; set; } = null!;
        public string OrcamentoUsuarioIdUsuario { get; set; } = null!;
        public string ItemIdItem { get; set; } = null!;
        public int Quantidade { get; set; }
        public string NomeItem { get; set; } = string.Empty;
    }
}