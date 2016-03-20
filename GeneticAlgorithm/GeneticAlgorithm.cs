using GeneticAlgorithm.Concretes;
using GeneticAlgorithm.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class GeneticAlgorithm
    {
        private Population p;
        private Population nextGeneration;
        private List<Chromosome> parentsList;

        private Int32 _populationSize;
        private Int16 _citiesAmount;
        private Double _xRange;
        private Double _yRange;
        private Double _pCrossover;
        private Double _pMutate;
        private String _selectionOp;

        public GeneticAlgorithm(Int32 populationSize, Int16 citiesAmount, Double xRange, Double yRange, String selectionOp, Double pCrossover = 0.6, Double pMutate = 0.15)
        {
            _populationSize = populationSize;
            _citiesAmount = citiesAmount;
            _pCrossover = pCrossover;
            _pMutate = pMutate;
            _selectionOp = selectionOp;      

            _xRange = xRange;
            _yRange = yRange;

            p = new Population(populationSize, citiesAmount, xRange, yRange);            
        }

        public void GenerateNewGeneration()
        {
            nextGeneration = new Population(_populationSize, _citiesAmount, _xRange, _yRange, isEmpty: true);

            parentsList = p.SelectParents(_selectionOp);
            Int32 count = parentsList.Count();

            while (nextGeneration.Chromosomes.Count != count)
            {
                if (RandomExtension.Instance.NextDouble() < _pCrossover)
                {
                    ChromosomePair parents = new ChromosomePair
                    {
                        Individual1 = parentsList[RandomExtension.Instance.Next(count)],
                        Individual2 = parentsList[RandomExtension.Instance.Next(count)]
                    };

                    ChromosomePair parentsCrossed = parents.CrossOver();
                    parentsCrossed.MutatePair(_pMutate);

                    ChromosomePair offspring = new ChromosomePair
                    {
                        //Individual1 = parents.Individual1.RouteLength < parents.Individual2.RouteLength ? parents.Individual1 : parents.Individual2,
                        //Individual2 = parentsCrossed.Individual1.RouteLength < parentsCrossed.Individual2.RouteLength ? parentsCrossed.Individual1 : parentsCrossed.Individual2

                        Individual1 = parentsCrossed.Individual1,
                        Individual2 = RandomExtension.Instance.NextDouble() < 0.1 ? new Chromosome(parentsCrossed.Individual2) : parentsCrossed.Individual2
                    };

                    nextGeneration.AddToNextGeneration(offspring);
                }
            }

            p = nextGeneration;
        }

        public Chromosome GetBestSolutionSoFar()
        {
            return p.GetBestSolutionSoFar();
        }
    }
}
