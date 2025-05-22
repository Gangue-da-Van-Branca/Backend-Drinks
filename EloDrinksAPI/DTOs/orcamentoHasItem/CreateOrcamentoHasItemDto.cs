namespace EloDrinksAPI.DTOs.orcamentoHasItem
{
    public class CreateOrcamentoHasItemDto
    {
        public long OrcamentoIdOrcamento { get; set; }
        public long OrcamentoUsuarioIdUsuario { get; set; }
        public long ItemIdItem { get; set; }
        public int Quantidade { get; set; }
    }

}