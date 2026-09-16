namespace ToMainApi.Models.Dtos.User
{
    public class UserBlockHistoryDto
    {
        public int Id { get; set; }

        // Блокировка
        public DateTime BlockedAt { get; set; }
        public string Reason { get; set; }
        public string BlockedByFIO { get; set; }
        public int BlockedByUserId { get; set; }
        public DateTime? BlockedUntil { get; set; }

        // Разблокировка (null, если ещё активна)
        public DateTime? UnblockedAt { get; set; }
        public string UnblockReason { get; set; }
        public string UnblockedByFIO { get; set; }
        public int? UnblockedByUserId { get; set; }

        // Вычисляемые
        public bool IsActive { get; set; }        // блокировка ещё действует
        public bool IsPermanent { get; set; }     // BlockedUntil == null
        public bool WasAutoUnblocked { get; set; }
    }
}
