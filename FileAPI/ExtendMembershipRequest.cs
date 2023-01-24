using System.ComponentModel.DataAnnotations.Schema;

namespace DataBaseAPI.Models
{
    [NotMapped]
    public class ExtendMembershipRequest
    {
        public int Id { get; set; }
        public int NumberOfMonths { get; set; }

    }
}
