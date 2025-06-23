using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowAPI.Models
{
    public class CustomEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomEntityID { get; set; }
        public string EntityType { get; set; } // Ví dụ: CustomAction, CustomActivity
        public string Data { get; set; } // Lưu dữ liệu JSON hoặc serialized
        public int? ProcessID { get; set; }

        public Process Process { get; set; }
    }
}