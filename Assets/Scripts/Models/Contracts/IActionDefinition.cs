using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Models.Contracts
{
    public interface IActionDefinition
    {
        float Duration { get; set; }
        float Delay { get; set; }
        AnimationCurve Curve { get; set; }
    }
}
