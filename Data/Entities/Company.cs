using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetingRooms.Data.Entities
{
    [Table("Companies")]
    public class Company
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] PasswordHash { get; set; }

        public IEnumerable<MeetingRoom> MeetingRoomes { get; set; }
    }
}
