using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.API.Models
{
    public class User
    {
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DoB { get; set; }
        public DateTime FirstRegistered { get; set; }
        public DateTime RegisteredTo { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public int? Weight { get; set; }
        public int? Height { get; set; }
        public string? WantedResults { get; set; }
        public double? VariableForTrainerMatch { get; set; }
        public int? LeftOnRegistration { get; set; }
        [NotMapped]
        public string? DoBNm { get; set; }
        [NotMapped]
        public string? FirstRegisteredNm { get; set; }
        [NotMapped]
        public string? RegisteredToNm { get; set; }

        public void FillNotMappedData()
        {
            this.DoBNm = this.DoB.Date.ToString();
            this.FirstRegisteredNm = this.FirstRegistered.Date.ToString();
            this.RegisteredToNm = this.RegisteredTo.Date.ToString();
        }
    }
}
