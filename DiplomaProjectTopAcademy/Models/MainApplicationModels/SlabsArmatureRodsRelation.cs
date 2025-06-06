using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomaProjectTopAcademy.Models.MainApplicationModels
{
    //PJT - Pure Join Table
    [PrimaryKey(nameof(IDSlab), nameof(IDArmatureRod))]
    public class SlabsArmatureRodsRelation
    {
        [ForeignKey("Slab")]
        public int IDSlab { get; set; } 
        [ForeignKey("ArmatureRod")]
        public int IDArmatureRod { get; set; }

        //Navigation properties
        public required Slab Slab { get; set; } // связь многие ко многим через таблицу
        public required ArmatureRod ArmatureRod { get; set; } //  связь многие ко многим через таблицу
    }
}
