using GeneticAlgorithm.Concretes;
using GeneticAlgorithm.Extensions;
using GeneticAlgorithm.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.Concretes 
{
    public class ChromosomePair : ICrossable
    {
        public Chromosome Individual1 { get; set; }
        public Chromosome Individual2 { get; set; }

        public ChromosomePair CrossOver()
        {
            Int32 chromoLen = Individual1.Route.Count;

            Int32 lcp = RandomExtension.Instance.Next(chromoLen - 2); //left crossover point
            Int32 rcp = RandomExtension.Instance.Next(lcp, chromoLen - 1); //right crossover point         

            Chromosome offspring1 = new Chromosome(Individual1, isShuffled: false);
            Chromosome offspring2 = new Chromosome(Individual2, isShuffled: false);

            #region DebugWriteLines
            //    Console.WriteLine("parent1");
            //    Individual1.PrintRoute();
            //    Console.WriteLine("parent2");
            //    Individual2.PrintRoute();
            //    Console.WriteLine("child1");
            //    offspring1.PrintRoute();
            //    Console.WriteLine("child2");
            //    offspring2.PrintRoute();

            //Console.WriteLine();
            //Console.WriteLine($"LeftPoint: {lcp}, Right point: {rcp}");
            #endregion

            IDictionary<City, City> matches = new Dictionary<City, City>();

            List<City> keys = new List<City>();
            List<City> values = new List<City>();

            for (Int32 i = lcp; i <= rcp; ++i)
            {
                keys.Add(offspring1.Route[i]);
                values.Add(offspring2.Route[i]);
            }

            for (Int32 i = 0; i < rcp - lcp + 1; ++i)
            {
                Int32 j = i;

                if(!values.Any(x => x.Id == keys[i].Id))
                {
                    while (keys.Any(k => k.Id == values[j].Id))
                    {
                        j = keys.FindIndex(k => k.Id == values[j].Id);
                    }

                    matches.Add(keys[i], values[j]);
                    //Console.WriteLine($"{keys[i].Id} : {values[j].Id}");
                }                
            }

            offspring1.Route.ExchangeParts(offspring2.Route, lcp, rcp); // swap centrals

            CrossTails(offspring1, matches, 0, lcp);
            CrossTails(offspring1, matches, rcp + 1, chromoLen);

            CrossTails(offspring2, matches, 0, lcp);
            CrossTails(offspring2, matches, rcp + 1, chromoLen);

            #region DebugWriteLines

            //Console.WriteLine();
            //Console.WriteLine("Childs after crossing");
            //Console.WriteLine("child1");
            //offspring1.PrintRoute();
            //Console.WriteLine("child2");
            //offspring2.PrintRoute();

            #endregion            

            ChromosomePair offspring = new ChromosomePair
            {
                Individual1 = offspring1,
                Individual2 = offspring2
            };

            return offspring;
        }

        public void CrossTails(Chromosome chromosome, IDictionary<City,City> matches, Int32 startIndex, Int32 endIndex)
        {
            for (Int32 i = startIndex; i < endIndex; ++i)
            {
                if (matches.Keys.Contains(chromosome.Route[i]))
                {
                    chromosome.Route[i] = matches[chromosome.Route[i]];
                }
                else if (matches.Values.Contains(chromosome.Route[i]))
                {
                    chromosome.Route[i] = matches.FirstOrDefault(x => x.Value.Equals(chromosome.Route[i])).Key;
                }
                else
                {
                    continue;
                }
            }
        }

        public void MutatePair(Double prob = 0.15)
        {
            Double p1 = RandomExtension.Instance.NextDouble();

            if(p1 < prob)
            {
                Individual1.Mutate();                
            }

            Double p2 = RandomExtension.Instance.NextDouble();

            if (p2 < prob)
            {
                Individual2.Mutate();
            }

            //Console.WriteLine($"Mutate probabilities: 1 - {p1}, 2 - {p2}");
        }
    }
}
