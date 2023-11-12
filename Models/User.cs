using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SHOU.Models;

public partial class User
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Tên đăng nhập không được trống")]
    [Display(Name = "Tên đăng nhập")]
    public string UserName { get; set; } = null!;

    [Display(Name = "Mật khẩu")]
    [Required(ErrorMessage = "Mật khẩu không được trống.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Avatar { get; set; }

    public DateTime? Birthday { get; set; }

    public bool? Gender { get; set; }
}
