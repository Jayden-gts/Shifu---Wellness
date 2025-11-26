using System.ComponentModel.DataAnnotations;

namespace Shifu.Models
{
    public class Resource
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Type { get; set; }  //we can get a Video, Article or PDF etc
        public string? ThumbnailUrl { get; set; }
        public string Url { get; set; }
        public string? Description { get; set; }
    }
}
