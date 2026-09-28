namespace ToMainApi.Models.Enums
{
    public enum ApplicationStatus
    {
        /// <summary>Создана, валидируется (списание/резерв).</summary>
        Validating,
        /// <summary>Прошла валидацию, ожидает модератора.</summary>
        OnModeration,
        /// <summary>Отклонена модератором.</summary>
        RejectedByModerator,

        /// <summary>Одобрена модератором, готова к ЕАИСТО.</summary>
        Approved,
        /// <summary>ЕАИСТО обрабатывает.</summary>
        EaistoProcessing,
        /// <summary>Успех — карта получена.</summary>
        Completed,

        /// <summary>Ошибка от ЕАИСТО.</summary>
        EaistoError,
        
        /// <summary> Неясная ошибка</summary>
        Error,
    }
}
