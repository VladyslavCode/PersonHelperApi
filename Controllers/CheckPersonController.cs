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
    [Route("api/v1/check-person")]
    public class CheckPersonController : ControllerBase
    {
        private readonly ILogger<CheckPersonController> _logger;
        private readonly IRickAndMortyService _rickandmortyservice;
        private readonly InternalDbHandler _internalDbHandler;

        public CheckPersonController(ILogger<CheckPersonController> logger, IRickAndMortyService rickandmortyservice, InternalDbHandler internalDbHandler)
        {
            _logger = logger;
            _rickandmortyservice = rickandmortyservice;
            _internalDbHandler = internalDbHandler;
        }

        /// <summary>
        /// Find person in episode, name/episode validation by rickandmorty api
        /// </summary>
        /// <param name="checkPerson"></param>
        /// <returns>
        /// true  - if finded
        /// false - if not finded
        /// 404   - if person or episode not exists
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CheckPerson checkPerson)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(checkPerson.PersonName) && !string.IsNullOrWhiteSpace(checkPerson.PersonName))
                {
                    //Check in stored InternalDb
                    var InternalDbHandler = _internalDbHandler.ObjectsList.Where(x => x.Request is CheckPerson && x.Response is bool).Select(y => new { request = (CheckPerson)y.Request, response = (bool)y.Response }).Where(z => z.request.EpisodeName.Equals(checkPerson.EpisodeName) && z.request.PersonName.Equals(checkPerson.PersonName));
                    if (InternalDbHandler.Count() > 0)
                        return Ok(InternalDbHandler.FirstOrDefault().response);

                    //Check Episode
                    var AllEpisodes = await _rickandmortyservice.GetAllEpisodes();
                    if (!AllEpisodes.Any(x => x.Name.ToLower().Contains(checkPerson.EpisodeName.ToLower())))
                        return NotFound();

                    //Check Person
                    var Persons = await _rickandmortyservice.FilterCharacters(checkPerson.PersonName);
                    if (!Persons.Any(x => x.Name.ToLower().Contains(checkPerson.PersonName.ToLower())) || Persons.All(x => x.Episode.Count() == 0))
                        return NotFound();

                    //Check Person in Episode
                    var AllPersonsEpisodesURLs = Persons.SelectMany(x => x.Episode).Distinct();
                    var AllEpisodeIds = AllEpisodes.Select(x => x.Id);
                    foreach (var EpisodeURL in AllPersonsEpisodesURLs)
                    {
                        string EpisodeIdFromPath = EpisodeURL.AbsolutePath != null ? EpisodeURL.AbsolutePath.Split("episode/")[1].Trim() : string.Empty;
                        Helper.NumberFromString(EpisodeIdFromPath, out int EpisodeId, out bool IsDigit);
                        if (IsDigit && EpisodeId > 0 && AllEpisodeIds.Any(x => x == EpisodeId))
                        {
                            _internalDbHandler.ObjectsList.Add(new ApiObjectHandler() { Request = checkPerson, Response = true });
                            return Ok(true);
                        }
                    }
                    _internalDbHandler.ObjectsList.Add(new ApiObjectHandler() { Request = checkPerson, Response = false });
                    return Ok(false);
                }
            }
            catch (Exception Ex)
            {
                //Error or episode/person not finded
                _logger.LogWarning($"Something went wrong Message: {Ex.Message}, StackTrace: {Ex.StackTrace}");
                return NotFound();
            }
            return NotFound();
        }
    }
}
