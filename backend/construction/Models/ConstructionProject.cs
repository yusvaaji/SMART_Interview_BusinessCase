using System;
using System.ComponentModel.DataAnnotations;

namespace ConstructionProjectManagement.Models
{
    public class ConstructionProject
    {
        [Key]
        [StringLength(6, MinimumLength = 6)]
        public string ProjectId { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProjectName { get; set; }

        [Required]
        [MaxLength(500)] 
        public string ProjectLocation { get; set; }

        [Required]
        public ProjectStage ProjectStage { get; set; }

        [Required]
        public ProjectCategory ProjectCategory { get; set; }


        public string OtherCategory { get; set; }

        [Required]
        public DateTime ProjectConstructionStartDate { get; set; }

        [Required]
        [MaxLength(2000)]
        public string ProjectDetails { get; set; }

        [Required]
        public Guid ProjectCreatorId { get; set; }
    }

    public enum ProjectStage 
    {
        Planning = 0,
        Design = 1,
        Construction = 2,
        Completed = 3
    }

    public enum ProjectCategory
    {
        Education = 0,
        Health = 1,
        Office = 2,
        Others = 3
    }
}