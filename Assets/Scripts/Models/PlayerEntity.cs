using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class PlayerEntity
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public PlayerEntity(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}