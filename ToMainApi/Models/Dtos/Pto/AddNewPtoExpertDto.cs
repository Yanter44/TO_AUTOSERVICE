namespace ToMainApi.Models.Dtos.Pto
{
    public class AddNewPtoExpertDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string SequenceNumber { get; set; }
        public IFormFile KeyArchive { get; set; }
    }
}
