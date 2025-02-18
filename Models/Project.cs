﻿namespace Buisness.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string ProjectNumber { get; set; } = string.Empty;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CustomerId { get; set; }
        public int StatusId { get; set; }
        public int ProductId { get; internal set; }
        public int UserId { get; internal set; }
    }
}
