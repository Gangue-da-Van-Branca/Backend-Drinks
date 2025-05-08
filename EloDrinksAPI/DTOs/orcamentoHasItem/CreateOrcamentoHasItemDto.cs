namespace EloDrinksAPI.DTOs.orcamentoHasItem
{
    public class CreateOrcamentoHasItemDto
    {
        public int OrcamentoIdOrcamento { get; set; }
        public int OrcamentoUsuarioIdUsuario { get; set; }
        public int ItemIdItem { get; set; }
        public int Quantidade { get; set; }
    }

}