using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Generic;

namespace The_Movies.Models
{
    public class Cinema
    {
        public string Name { get; set; } = string.Empty;
        public List<Hall> Halls { get; set; } = new List<Hall>();
    }
}