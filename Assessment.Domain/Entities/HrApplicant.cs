namespace Assessment.Domain.Entities
{
    public class HrApplicant
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public string Email { get; set; } = "";

        public string PhoneNumber { get; set; } = "";

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public ICollection<HrTest> Tests { get; set; } = new List<HrTest>();
    }
}