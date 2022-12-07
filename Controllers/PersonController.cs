using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PersonHelperApi.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RickAndMorty;
using RickAndMorty.Net.Api.Service;

namespace PersonHelperApi.Controllers
{
    [ApiController]
    [Route("api/v1/person")]
    public class PersonController : ControllerBase
    {
        private readonly ILogger<PersonController> _logger;
        private readonly IRickAndMortyService _rickandmortyservice;
        private readonly InternalDbHandler _internalDbHandler;
        public PersonController(ILogger<PersonController> logger, IRickAndMortyService rickandmortyservice, InternalDbHandler internalDbHandler )
        {
            _logger = logger;
            _rickandmortyservice = rickandmortyservice;
            _internalDbHandler = internalDbHandler;
        }

        /// <summary>
        /// Method for finding first(one) person from list by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>
        /// Person - if finded
        /// 404    - if person not finded
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Get(string name)
        {
            try
            {
                var PersonsList = new List<Person>();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    var InternalDbHandler = _internalDbHandler.ObjectsList.Where( x => x.Request is string && x.Response is List<Person>).Select( y => new { request = (string)y.Request, response = (List<Person>)y.Response }).Where(z => z.request.Equals(name));
                    if (InternalDbHandler.Count() > 0)
                        return Ok(InternalDbHandler.FirstOrDefault().response);

                    var Persons = await _rickandmortyservice.FilterCharacters(name);
                    foreach (var person in Persons)
                    {
                        if (person?.Id > 0)
                        {
                            string OriginIdFromPath = person.Origin?.Url?.AbsolutePath != null ? person.Origin.Url.AbsolutePath.Split("location/")[1].Trim() : string.Empty;
                            RickAndMorty.Net.Api.Models.Domain.Location location = null;

                            Helper.NumberFromString(OriginIdFromPath, out int OriginId, out bool IsDigit);

                            if (IsDigit && OriginId > 0)
                                location = await _rickandmortyservice.GetLocation(OriginId);
                            PersonsList.Add((Person)(person, location));
                        }
                    }
                }
                else
                    return NotFound();

                if (PersonsList.Count() > 0)
                {
                    _internalDbHandler.ObjectsList.Add(new ApiObjectHandler() { Request = name, Response = PersonsList });
                    return Ok(PersonsList);
                }
            }
            catch (Exception Ex)
            {
                //Error or person not finded
                _logger.LogWarning($"Something went wrong Message: {Ex.Message}, StackTrace: {Ex.StackTrace}");
                return NotFound();
            }

            return NotFound();
        }
    }
}
