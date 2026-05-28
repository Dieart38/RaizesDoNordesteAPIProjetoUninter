namespace RaizesNordeste.API.Domain.Enums
{
    public enum StatusPedido
    {
        AGUARDANDO_PAGAMENTO = 1,
        PAGAMENTO_RECUSADO = 2,
        RECEBIDO = 3,
        EM_PREPARO = 4,
        PRONTO = 5,
        ENTREGUE = 6,
        CANCELADO = 7
    }
}
