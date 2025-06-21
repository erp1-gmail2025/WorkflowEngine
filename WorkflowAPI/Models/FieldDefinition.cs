using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkflowAPI.Enums;

namespace WorkflowAPI.Models
{
    public class FieldDefinition
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public FieldType FieldType { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }
        public int StateId { get; set; }
        [ForeignKey(nameof(StateId))]
        public State State { get; set; }
        public string Config { get; set; } // JSON Config nếu gọi API
        public string Validation { get; set; } // JSON để quản lí validate
    }
}