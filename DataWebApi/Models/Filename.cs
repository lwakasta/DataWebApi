using System.ComponentModel.DataAnnotations;

namespace DataWebApi.Models
{
    public class Filename
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Value> Values { get; set; } = new List<Value>();
        public Result Result { get; set; }        
    }
}
