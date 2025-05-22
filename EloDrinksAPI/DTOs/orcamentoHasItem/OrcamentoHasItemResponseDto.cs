namespace EloDrinksAPI.DTOs.orcamentoHasItem
{
    public class OrcamentoHasItemResponseDto
    {
        public long OrcamentoIdOrcamento { get; set; }
        public long OrcamentoUsuarioIdUsuario { get; set; }
        public long ItemIdItem { get; set; }
        public int Quantidade { get; set; }
        public string NomeItem { get; set; } = string.Empty;
    }
}