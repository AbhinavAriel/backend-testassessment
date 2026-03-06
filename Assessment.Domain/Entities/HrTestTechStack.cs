namespace Assessment.Domain.Entities
{
    public class HrTestTechStack
    {
        public Guid Id { get; set; }

        public Guid TestId { get; set; }
        public HrTest Test { get; set; } = null!;

        public Guid TechStackId { get; set; }
        public TechStack TechStack { get; set; } = null!;   
    }
}