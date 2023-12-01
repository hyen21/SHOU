using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHOU.Models.EditModel
{
    public class PostEditModel
    {
        public class NewPost
        {
            public string? Id { get; set; } 

            public string? IdUser { get; set; } 

            [Display(Name = "Nội dung")]
            public string? Content { get; set; }

            public string? Video { get; set; }

            public DateTime? CreateAt { get; set; }

          
            public string? Image { get; set; }

        }

        public class UpdatePost
        {
            public string? Id { get; set; }

            public string? IdUser { get; set; }

            [Display(Name = "Nội dung")]
            public string? Content { get; set; }

            public string? Image { get; set; }

        }
        
       
    }
}
