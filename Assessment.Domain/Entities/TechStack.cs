using System;

namespace Assessment.Domain.Entities
{
    public class TechStack
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }
}