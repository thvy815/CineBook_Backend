using System;
using System.Collections.Generic;

namespace ShowtimeService.Domain.Entities
{
    public class Province
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        // Navigation property
        public ICollection<Theater> Theaters { get; set; } = new List<Theater>();
    }
}
