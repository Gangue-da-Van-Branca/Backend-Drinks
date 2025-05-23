namespace EloDrinksAPI.DTOs.orcamentoHasItem
{
    public class CreateOrcamentoHasItemDto
    {
        public string OrcamentoIdOrcamento { get; set; }
        public string OrcamentoUsuarioIdUsuario { get; set; }
        public string ItemIdItem { get; set; }
        public int Quantidade { get; set; }
    }

}