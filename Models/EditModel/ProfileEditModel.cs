using System.ComponentModel.DataAnnotations;

namespace SHOU.Models.EditModel
{
    public class ProfileEditModel
    {
        public string Id { get; set; } = null!;
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Họ tên không được trống")]
        [Display(Name = "Họ tên")]
        public string Name { get; set; }

        [Display(Name = "Gmail")]
        public string? Email { get; set; }

        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        public string? Avatar { get; set; }

        public string? Background { get; set; }

        [Display(Name = "Ngày sinh")]
        public DateTime? Birthday { get; set; }

        [Display(Name = "Giới tính")]
        public bool? Gender { get; set; }
        public bool ReadOnly { get; set; }
    }
}
