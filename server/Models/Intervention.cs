using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _.Models
{
    public class Intervention
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // Auto‑generated primary key

        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
    }
}
