using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.ComponentModel.DataAnnotations;

namespace ToDoBA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        IMongoCollection<Todo> _toDoCollection;
        public TodoController(IMongoClient client)
        {
            var dataBase = client.GetDatabase("ToDoDB");
            _toDoCollection = dataBase.GetCollection<Todo>("ToDo");
        }
        [HttpGet]
        public async Task<IActionResult> GetTodo()
        {
            IEnumerable<Todo> todos = await Task.FromResult(_toDoCollection.Find(p => p.status == true).ToEnumerable());
            todos = todos.OrderByDescending(d=>d.Id);    
            return Ok(todos);
        }
        [HttpPost]
        public async Task<IActionResult> addTodo([FromQuery][Required] string todoName)
        {
            IEnumerable<Todo> todos = _toDoCollection.Find(_ => true).ToEnumerable();
            Todo LastTodo = default;
            if (todos.Any())
            {
                todos = todos.OrderBy(p => p.Id);
                LastTodo = todos.Last();
            }
            else
            {
                LastTodo = new Todo { Id = 0 }; // Create a new Todo with Id 0
            }
            Todo todo = new Todo
            {
                Id = LastTodo.Id + 1,
                Name = todoName,
                createDate = DateTime.Now,
                FinishDate = null,
                status = true
            };
            await _toDoCollection.InsertOneAsync(todo);
            return Ok(todo);
        }
        [HttpPut]
        public async Task<IActionResult> updateTodo([FromQuery][Required]int id, [FromQuery][Required] string todoName)
        {
            var filter = Builders<Todo>.Filter.Eq("_id", id);
            Todo todo = new Todo
            {
                Id = id,
                Name = todoName,
                createDate = DateTime.Now,
                status = true
            };
            var result = await _toDoCollection.ReplaceOneAsync(filter, todo);
            return Ok(result);
        }
        [HttpDelete]
        public async Task<IActionResult> RemoveToDo([FromQuery][Required] int id)
        {
            Todo todo = _toDoCollection.Find(p => p.Id == id).First();
            todo.FinishDate = DateTime.Now;
            todo.status = false;
            await _toDoCollection.ReplaceOneAsync(Builders<Todo>.Filter.Eq("_id", id),todo);
            return Ok();
        }
    }
}
