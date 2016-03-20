using GeneticAlgorithm.Extensions;
using GeneticAlgorithm.Interfaces;
using GeneticAlgorithm.Concretes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.Concretes
{
    public class Population : ISelectable
    {
        private IList<Chromosome> _chromosomes;
        public IList<Chromosome> Chromosomes { get { return _chromosomes; } }
        public Double TotalFitness { get { return CalcTotalFitness(); } private set { TotalFitness = value; } }

        public Population(Int32 populationSize, Int16 citiesAmount, Double xRange, Double yRange, Boolean isEmpty = false)
        {
            _chromosomes = new List<Chromosome>(populationSize);

            if(!isEmpty)
            {
                Chromosome chromosome = new Chromosome(citiesAmount, xRange, yRange);
                _chromosomes.Add(chromosome);

                for (Int32 i = 0; i < populationSize - 1; ++i)
                {
                    _chromosomes.Add(new Chromosome(chromosome));
                }
            }
        }

        public void AddToNextGeneration(ChromosomePair pair)
        {
            _chromosomes.Add(pair.Individual1);
            _chromosomes.Add(pair.Individual2);
        }

        public List<Chromosome> SelectParents(String selectionOperator)
        {
            if (selectionOperator.Equals("Tournament selection"))
            {
                return TournamentSelect();
            }
            else
            {
                return FitnessProportionateSelect();
            }
        }

        #region Fitness Proportionate Selection

        public List<Chromosome> FitnessProportionateSelect()
        {            
            IDictionary<Chromosome, Double> probabilities = new Dictionary<Chromosome, Double>();            

            Console.WriteLine($"TotalFitness = {TotalFitness}");

            foreach (var c in _chromosomes)
            {
                probabilities.Add(/*c.ChromosomeID, */c, c.RouteLength / TotalFitness);
            }

            probabilities = (from item in probabilities
                             orderby item.Value ascending
                             select item).ToDictionary(pair => pair.Key, pair => pair.Value);

            MapFitnessToProbabilityInterval(probabilities);

            Int32 count = _chromosomes.Count;
            List<Chromosome> intermidiate = new List<Chromosome>();
            
            for (Int32 i = 0; i < count; ++i)
            {
                intermidiate.Add(RunRoulette(probabilities));
            }

            return intermidiate;
        }

        public void MapFitnessToProbabilityInterval(IDictionary<Chromosome, Double> probabilities)
        {

//-- Map fitness values to [0;1] probability interval 

//-- In probability dictionary less probability complies with shorter route,
//-- but it contradicts with roulette selection principle.
//-- According to last one the shortest route should be presented by the largest segment on the roulette
//-- That's why next code is required.

            IList<Chromosome> keys = new List<Chromosome>(probabilities.Keys);

            Double range = probabilities.Values.Max() - probabilities.Values.Min();
            Double b = 0.1 + (0.87 / range) * probabilities.Values.Max();

            foreach (var key in keys)
            {
                probabilities[key] = b - (0.87 / range) * probabilities[key];
            }

            IList<Double> values = new List<Double>(probabilities.Values);
            Double sumProbabilities = 0;

            foreach (var value in values)
            {
                sumProbabilities += value;
            }

            foreach (var key in keys)
            {
                probabilities[key] /= sumProbabilities;
            }
        }

        public Chromosome RunRoulette(IDictionary<Chromosome, Double> probabilities)
        {
            Double value = RandomExtension.Instance.NextDouble();

            foreach (var item in probabilities)
            {
                value -= item.Value;

                if (value <= 0)
                {
                    return (Chromosome)item.Key;
                }
            }

            return null;
        }

        #endregion

        #region Tournament Selection

        public List<Chromosome> TournamentSelect()
        {
            Int32 count = _chromosomes.Count;

            List<Chromosome> intermidiate = new List<Chromosome>(count % 2 == 0 ? count : --count);
            Chromosome c1, c2;  

            while(intermidiate.Count < count)
            {
                c1 = _chromosomes[RandomExtension.Instance.Next(count)];
                c2 = _chromosomes[RandomExtension.Instance.Next(count)];

                intermidiate.Add(c1.RouteLength < c2.RouteLength ? c1 : c2);
            }

            //Console.WriteLine($"Population fitness: {TotalFitness}");

            //double deb = 0;
            //foreach (var item in intermidiate)
            //{
            //    deb += item.RouteLength;
            //    //Console.WriteLine(item.RouteLength);
            //}
            //Console.WriteLine($"Intermediate fitness: {deb}");

            return intermidiate;
        }

        #endregion

        public Double CalcTotalFitness()
        {
            Double tf = 0;

            foreach(Chromosome c in _chromosomes)
            {
                tf += c.RouteLength;
            }

            return tf;
        }

        public Chromosome GetBestSolutionSoFar()
        {
            return Chromosomes.Aggregate((curMin, x) => (curMin == null || x.RouteLength < curMin.RouteLength ? x : curMin));
        }

        //Debug only 
        public void PrintPopulation()
        {
            foreach(var item in _chromosomes)
            {
                item.PrintRoute();
            }
        }
    }
}
