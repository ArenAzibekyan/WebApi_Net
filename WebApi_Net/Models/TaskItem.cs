using System;

namespace WebApi_Net.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
    }
}