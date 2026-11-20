namespace MyAcademy_CargoTrack.Models
{
    public class RegisterViewModel
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
