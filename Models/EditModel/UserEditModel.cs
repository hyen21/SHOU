using System.ComponentModel.DataAnnotations;

namespace SHOU.Models.EditModel
{
    public class UserLoginEditModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được trống")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu không được trống.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class UserSignUpEditModel
    {
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Tên đăng nhập phải từ 6 đến 20 ký tự.")]
        [Required(ErrorMessage = "Tên đăng nhập không được trống")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        [StringLength(20, MinimumLength = 6, ErrorMessage = "Tên đăng nhập phải từ 6 đến 20 ký tự.")]
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu không được trống.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [StringLength(20, MinimumLength = 6, ErrorMessage = "Tên đăng nhập phải từ 6 đến 20 ký tự.")]
        [Display(Name = "Nhập lại mật khẩu")]
        [Required(ErrorMessage = "Nhập lại mật khẩu không được trống.")]
        [DataType(DataType.Password)]
        public string RePassword { get; set; }
    }
}
