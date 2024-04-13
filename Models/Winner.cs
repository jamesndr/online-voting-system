using System.ComponentModel.DataAnnotations;

namespace Onlinevotingsystem.Models
{
    public class Winner
    {
        [Key]
        public int Id { get; set; }
        public string CandidateName { get; set; }
        public string CandidateImg { get; set; }
        public string ElectionName { get; set; }
        public int NoofVotes { get; set; }

    }
}
