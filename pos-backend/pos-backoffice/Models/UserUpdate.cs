using System.ComponentModel.DataAnnotations;

namespace pos_backoffice.Models
{
    public class UserUpdate
    {
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }

        [EnumDataType(typeof(Role))]
        public string? Role { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        private string? _password;
        [MinLength(6)]
        public string? Password
        {
            get => _password;
            set => _password = replaceEmptyWithNull(value);
        }

        private string? _confirmPassword;
        [Compare("Password")]
        public string? ConfirmPassword
        {
            get => _confirmPassword;
            set => _confirmPassword = replaceEmptyWithNull(value);
        }

        // helpers

        private string? replaceEmptyWithNull(string? value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}
