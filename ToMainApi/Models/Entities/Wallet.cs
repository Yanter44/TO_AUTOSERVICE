using Pipelines.Sockets.Unofficial.Arenas;

namespace ToMainApi.Models.Entities
{
    public class Wallet
    {
        public int Id { get; set; }

        public int AgentId { get; set; }
        public Agent Agent { get; set; }

        public decimal Balance { get; set; }

        public List<Transaction> Transactions { get; set; }
    }
}
