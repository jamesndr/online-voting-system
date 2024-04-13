using System.ComponentModel.DataAnnotations;

namespace Onlinevotingsystem.Models
{
    public class ForgotPass
    {
        [Key]
        public int Id { get; set; }
        public string email { get; set; }
        public string phoneNo { get; set; }
        public string newPassword { get; set; }
    }
}
