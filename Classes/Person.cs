using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonHelperApi.Classes
{
    public class Person
    {
        public static explicit operator Person((RickAndMorty.Net.Api.Models.Domain.Character character, RickAndMorty.Net.Api.Models.Domain.Location location) personinfo)
        {
            var Person = new Person();
            Person.Name = personinfo.character.Name;
            Person.Status = personinfo.character.Status.ToString();
            Person.Species = personinfo.character.Species;
            Person.Type = personinfo.character.Type;
            Person.Gender = personinfo.character.Gender.ToString();
            Person.Origin = personinfo.location != null ? (Origin)personinfo.location : null;
            return Person;
        }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Species { get; set; }
        public string Type { get; set; }
        public string Gender { get; set; }
        public Origin Origin { get; set; }
    }
}
