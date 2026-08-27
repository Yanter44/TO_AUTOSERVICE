namespace ToMainApi.Common
{
    public static class ResponseMessages
    {
        // === Успешные ответы ===
        public const string Success = "Операция прошла успешно";

        public const string ApplicationSuccesfullyCreated = "Запись успешно создана";
        public const string ApplicationSuccesfullyUpdated = "Запись успешно обновлена";
        public const string ApplicationSuccesfullyDeleted = "Запись успешно удалена";

        public const string LoginSuccess = "Вход выполнен успешно";
        public const string RegistrationSuccess = "Регистрация прошла успешно";

        // === Неуспешные ответы ===
        public const string UnSuccess = "Операция не прошла успешно";
        public const string Error = "Произошла ошибка";
    }
}
