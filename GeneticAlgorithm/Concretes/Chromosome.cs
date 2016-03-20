using GeneticAlgorithm.Extensions;
using GeneticAlgorithm.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.Concretes
{
    public class Chromosome : IMutateable
    {
        // Absolute fitness
        public Double RouteLength
        {
            get
            {
                return CalculateFitnessFunction();
            }
            set
            {
                RouteLength = value;
            }
        }

        private IList<City> _route;
        public IList<City> Route { get { return _route; } } 

        // implement also cstr with _route init by a list of positions

        public Chromosome(Int16 citiesAmount, Double xRange = 20, Double yRange = 20)
        {
            _route = new List<City>(citiesAmount);

            for(UInt16 i = 0; i < citiesAmount; ++i)
            {
                City city = new City { Id = i, X = RandomExtension.Instance.NextDouble() * xRange, Y = RandomExtension.Instance.NextDouble() * yRange };
                _route.Add(city);
            }
        }

        public Chromosome(Chromosome anotherChromosome, Boolean isShuffled = true)
        {
            _route = new List<City>();
            _route = ListExtensions.Clone(anotherChromosome.Route);

            if(isShuffled)
            {
                ListExtensions.Shuffle(_route);
            }            
        }

        public bool Mutate()
        {
            Int32 pos1 = RandomExtension.Instance.Next(_route.Count);
            Int32 pos2 = RandomExtension.Instance.Next(_route.Count);

            try
            {
                City tmp = _route[pos1];
                _route[pos1] = _route[pos2];
                _route[pos2] = tmp;

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }            
        }

        public Double CalculateFitnessFunction()
        {
            Double length = 0;

            for(Int16 i = 0; i < _route.Count - 1; ++i)
            {
                length += Math.Sqrt(Math.Pow((Route[i].X - Route[i + 1].X), 2) + Math.Pow((Route[i].Y - Route[i + 1].Y), 2));
            }

            return length;
        }

        public Boolean SequenceEqual(Chromosome c)
        {
            Int32 k = 0;

            try
            {
                for (Int32 i = 0; i < _route.Count; ++i)
                {
                    if (_route[i].Equals(c.Route[i]))
                        ++k;
                }
            }
            catch(NullReferenceException)
            {
                return false;
            }           

            return k == _route.Count;
        }

        //---------------- Debug only
        public void PrintRoute()
        {
            foreach (var q in Route)
            {
                Console.Write($"{q.Id}-");
            }
            Console.WriteLine();
        }
    }
}
