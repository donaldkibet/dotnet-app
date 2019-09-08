using System.ComponentModel.DataAnnotations;

namespace dotnet_app.Data
{
    public class Value
    {
        
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}