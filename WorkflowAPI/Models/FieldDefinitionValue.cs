using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowAPI.Models
{
    public class FieldDefinitionValue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Value { get; set; }
        public int FieldDefinitionId { get; set; }
        [ForeignKey(nameof(FieldDefinitionId))]
        public FieldDefinition FieldDefinition { get; set; }
    }
}