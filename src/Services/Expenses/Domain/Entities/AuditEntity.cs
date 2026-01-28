namespace Domain.Entities;

public class AuditEntity
{
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public DateTime ModifiedOn { get; set; }
}
