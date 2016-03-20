using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class City : ICloneable
    {
        public UInt16 Id { get; set; }
        public Double X { get; set; }
        public Double Y { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
                return false;

            City city = obj as City;

            if ((System.Object)city == null)
                return false;

            return (Id == city.Id) && (X == city.X) && (Y == city.Y);
        }

        public override int GetHashCode()
        {
            return (Int32)Math.Round(((Double)Id + X) * Y);
        }
    }
}
