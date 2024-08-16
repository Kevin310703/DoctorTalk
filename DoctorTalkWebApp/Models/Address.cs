using System.ComponentModel.DataAnnotations;

namespace DoctorTalkWebApp.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int ZipCode  { get; set; }
    }
}
