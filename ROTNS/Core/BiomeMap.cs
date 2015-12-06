using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ROTNS.Core
{
    public class BiomeMap
    {
        Biome[] _Biomes;
        Biome[] _WaterBiomes;

        public BiomeMap(Biome[] Biomes, Biome[] WaterBiomes) { _Biomes = Biomes; _WaterBiomes = WaterBiomes; }

        public Biome Closest(float Height, float Temperature, float Moisture)
        {
            double nearest = Double.MaxValue;
            Biome c = null;
            Biome[] Biomes = Height > 0 ? _Biomes : _WaterBiomes;

            for(int i=0; i<Biomes.Length;++i)
            {
                double d = Biomes[i].Distance(Height, Temperature, Moisture);
                if(d < nearest)
                {
                    nearest = d;
                    c = Biomes[i];
                }
            }
            return c;
        }
    }
}
