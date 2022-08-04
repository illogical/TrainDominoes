using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Interfaces
{
    public interface IDominoInfo
    {
        int ID { get; set; }
        int TopScore { get; set; }
        int BottomScore { get; set; }
        bool Flipped { get; set; }
    }
}
