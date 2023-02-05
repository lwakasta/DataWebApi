using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace DataWebApi.Models
{
    public class Value
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("FileName")]
        public int FileId { get; set; }
        public virtual Filename FileName { get; set; }

        [RangeDateToNow("2000-01-01")]
        public DateTime Date { get; set; }

        [Range(0, Int32.MaxValue)]
        public int Time { get; set; }

        [Range(0, Double.MaxValue)]
        public double Param { get; set; }

        public bool ValidateValue()
        {
            var valContext = new ValidationContext(this);
            var valResult = new List<ValidationResult>();
            return Validator.TryValidateObject(this, valContext, valResult, true);
        }
    }

    public class RangeDateToNow : RangeAttribute
    {
        public RangeDateToNow(string from)
            : base(typeof(DateTime), from, DateTime.Now.ToString()) { }
    }
}
