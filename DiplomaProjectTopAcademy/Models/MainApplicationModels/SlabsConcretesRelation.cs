using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DiplomaProjectTopAcademy.Models.MainApplicationModels
{
    public class SlabsConcretesRelation
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Slab")]
        public int IDSlab { get; set; }
        [ForeignKey("Concrete")]
        public int IDConcrete { get; set; }

        //Navigation properties

    }
}
