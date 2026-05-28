namespace RaizesNordeste.API.DTOs
{
    public class RespostaPadrao
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public object? Dados { get; set; }
        public List<ErroDetalhe>? Erros { get; set; }
        public int StatusCode { get; set; }
    }

    public class ErroDetalhe
    {
        public string Campo { get; set; } = string.Empty;
        public string Problema { get; set; } = string.Empty;
    }
}