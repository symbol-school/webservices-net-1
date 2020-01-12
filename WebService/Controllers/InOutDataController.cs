using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebService.Utilit;

namespace WebService.Controllers
{
    [ApiController]
    [Route("data")]
    public class InOutDataController : ControllerBase
    {
        private readonly ILogger<InOutDataController> _logger;
        private readonly IOptionsMonitor<CheckWords> _checkWords;

        //Пока на памяти храним, потом базенку прикручу
        private List<string> _data = new List<string>();
        
        public InOutDataController(ILogger<InOutDataController> logger,
            IOptionsMonitor<CheckWords> options)
        {
            _logger = logger;
            _checkWords = options;
        }

        /// <summary>
        /// Получение всех слов
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public IEnumerable<string> GetAll() => _data;

        /// <summary>
        /// Добавление слова по индексу
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        [HttpPost("insert/one")]
        public IActionResult InsertItem([FromBody]string word)
        {
            if (!CheckWord(word))
            {
                _logger.LogWarning("Данное слово не приветствие!");
                return BadRequest("Данное слово не приветствие!");
            }

            _data.Add(word.ToLower());
            return Ok();
        }

        /// <summary>
        /// Получение всех слов по индексу
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [HttpGet("from/index")]
        public string Get(int index) => _data.ElementAtOrDefault(index) ?? "Такой индекс отсутствует!";

        /// <summary>
        /// Удаление слова по индексу
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [HttpDelete("from/index")]
        public IActionResult DeleteData(int index)
        {
            if (_data.Count >= index - 1)
            {
                _data.RemoveAt(index);
                return Ok();
            }

            _logger.LogError("Данный индекс отутствует");
            return BadRequest("Данный индекс отутствует");
        }

        /// <summary>
        /// Обновление слова по индексу
        /// </summary>
        /// <param name="index"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        [HttpPut("update")]
        public IActionResult UpdateData(int index, string word)
        {
            if (_data.Count >= index - 1 && CheckWord(word))
            {
                _data[index - 1] = word;
                return Ok();
            }

            return BadRequest("Слово не приветствие, либо такого индекса не существует!");
        }

        /// <summary>
        /// Проверка слова на слова приветствия
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private bool CheckWord(string word) => _checkWords.CurrentValue?.WordList.Any(x => x == word.ToLower()) ?? false;
    }
}
