using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class Player
    {
        public string Name { get; set; }
        public int? TrackNumber { get; set; }

        public Player(string name)
        {
            TrackNumber = null;
            Name = name;
        }
    }
}
