using System.ComponentModel.DataAnnotations;
using DataWebApi.Models;

namespace DataWebApi.Dto
{
    public class ValueDto
    {
        public DateTime Date { get; set; }
        public int Time { get; set; }
        public double Param { get; set; }
    }
}
