namespace EloDrinksAPI.DTOs.orcamentoHasItem
{
    public class OrcamentoHasItemResponseDto
    {
        public int OrcamentoIdOrcamento { get; set; }
        public int OrcamentoUsuarioIdUsuario { get; set; }
        public int ItemIdItem { get; set; }
        public int Quantidade { get; set; }
        public string NomeItem { get; set; } = string.Empty;
    }
}