using System.Collections.Generic;

namespace Assets.Scripts.Models
{
    public class Station
    {
        // a station has 8 tracks
        public List<Track> Tracks = new List<Track>(8);  // tracks by track number
        // TODO: track which tracks have trains on them

        public DominoEntity Engine { get; private set; }

        public Station(DominoEntity engine)
        {
            Engine = engine;
        }        
    }
}