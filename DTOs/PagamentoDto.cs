namespace RaizesNordeste.API.DTOs
{
    public class PagamentoDto
    {
        public string MetodoPagamento { get; set; } = "MOCK";
        public CartaoMockDto? CartaoMock { get; set; }
    }

    public class CartaoMockDto
    {
        public string Numero { get; set; } = string.Empty;
        public string Validade { get; set; } = string.Empty;
        public string Cvv { get; set; } = string.Empty;
    }
}