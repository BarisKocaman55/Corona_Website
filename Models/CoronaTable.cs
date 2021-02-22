using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corono_Website.Models
{
    [Table("CoronaTable")]
    public class CoronaTable
    {
        [Key, Column(Order = 1)]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int NumOfTest { get; set; }

        [Required]
        public int NumOfCase { get; set; }

        [Required]
        public int NumOfPatients { get; set; }

        [Required]
        public int NumOfDeaths { get; set; }

        [Required]
        public int NumOfHealed { get; set; }

        [Required]
        public int NumOfIntensiveCare { get; set; }

        
        public int TotalTest { get; set; }
        public int TotalCase { get; set; }
        public int TotalDeaths { get; set; }
        public int TotalHealed { get; set; }
    }
}