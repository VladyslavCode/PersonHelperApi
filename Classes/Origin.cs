using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonHelperApi.Classes
{
    public class Origin
    {
        public static explicit operator Origin(RickAndMorty.Net.Api.Models.Domain.Location location)
        {
            Origin Origin = null;
            if (location != null && location.Id > 0)
            {
                Origin = new Origin() { Name = location.Name, Type = location.Type, Dimension = location.Dimension };
            }
            return Origin;
        }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Dimension { get; set; }
    }
}
