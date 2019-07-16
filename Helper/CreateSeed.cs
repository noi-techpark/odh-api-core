using System;
using System.Collections.Generic;
using System.Text;

namespace Helper
{
    /// <summary>
    /// Creates the Seed if there is no Seed defined
    /// </summary>
    public class CreateSeed
    {
        public static int GetSeed(int seed)
        {
            //Kein Seed es wird ein Seed zwischen 0 und 10 erzeugt
            if (seed == 0)
            {
                Random random = new Random();
                int randomNumber = random.Next(10);

                return randomNumber;
            }
            //Wenn der Seed höher 10 ist
            else if (seed > 10)
            {
                while (seed >= 10)
                    seed /= 10;

                return seed;
            }
            //Seed passt, wird zurückgebegeben
            else
                return seed;
        }

        public static string? GetSeed(string seedstring)
        {
            int seed;

            if (seedstring == null)
                return null;

            else if (int.TryParse(seedstring, out seed))
            {

                //Kein Seed es wird ein Seed zwischen 0 und 10 erzeugt
                if (seed == 0)
                {
                    Random random = new Random();
                    int randomNumber = random.Next(1, 50);

                    return randomNumber.ToString();
                }
                //Wenn der Seed höher 10 ist
                else if (seed > 50)
                {
                    while (seed >= 50)
                        seed /= 10;

                    return seed.ToString();
                }
                //Seed passt, wird zurückgebegeben
                else
                    return seed.ToString();
            }
            else
                return null;
        }
    }
}
