namespace ToMainApi.Models.Dtos.Requirementss
{
    public class EditDocumentRequirementDto
    {
        public int Id { get; set; }
        public string DocumentType { get; set; }
        public string DisplayName { get; set; }
        public bool IsRequire { get; set; }
    }
}
