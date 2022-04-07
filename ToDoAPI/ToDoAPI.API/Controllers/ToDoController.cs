using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToDoAPI.API.Models;
using ToDoAPI.DATA.EF;
using System.Web.Http.Cors;


namespace ToDoAPI.API.Controllers
{
    //GET = READ
    //POST = CREATE
    //PUT = EDIT
    //DELETE = DELETE
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ToDoController : ApiController
    {
        //Create a connection to the db
        ToDoEntities db = new ToDoEntities();

        //GET - /api/TodoItems
        public IHttpActionResult GetToDos()
        {
            //Below we create a list of EF TodoItem objects. In an API, it's best practice to install EF to the API layer when needing to accomplish this task.
            List<ToDoViewModel> ToDos = db.TodoItems.Include("Category").Select(t => new ToDoViewModel()
            {
                //Assign the columns of the ToDos db table to the ResourceViewModel object, so we can use the data (send the data back to requesting app)
                TodoId = t.TodoId,
                Action = t.Action,
                Done = t.Done,
                CategoryID = t.CategoryID,
                Category = new CategoryViewModel()
                {
                    CategoryID = t.Category.CategoryID,
                    Name = t.Category.Name,
                    Description = t.Category.Description
                }
            }).ToList<ToDoViewModel>();

            //Check the results and handle accordingly below
            if (ToDos.Count == 0)
            {
                return NotFound();
            }
            //Everything is good, return the data
            return Ok(ToDos);//ToDos are being passed in the response back to the requesting app.
        }//end GetToDos()

        //GET - api/Todoitems/id
                //ONE TODOITEM
        public IHttpActionResult GetToDo(int id)
        {
            //Create a new TodoModelViewModel object and assign it to the appropriate resource from the db
            ToDoViewModel ToDo = db.TodoItems.Include("Category").Where(t => t.TodoId == id).Select(t =>
                new ToDoViewModel()
                {
                    //Coopy the assignments from the GetResources() and paste below
                    TodoId = t.TodoId,
                    Action = t.Action,
                    Done = t.Done,
                    CategoryID = t.CategoryID,
                    Category = new CategoryViewModel()
                    {
                        CategoryID = t.Category.CategoryID,
                        Name = t.Category.Name,
                        Description = t.Category.Description
                    }
                }).FirstOrDefault();
            //scopeless if - once the return executes the scopes are closed.
            if (ToDo == null)
                return NotFound();

            return Ok(ToDo);

        }//end GetToDo

        //POST - api/TodoItems (HttpPost)
                //ONE TODOITEM
        public IHttpActionResult PostToDo(ToDoViewModel todo)
        {
            //1. Check to validate the object - we need to know that all the data necessary to create a TodoItem is there.
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            TodoItem newToDo = new TodoItem()
            {
                Action = todo.Action,
                Done = todo.Done,
                CategoryID = todo.CategoryID
            };

            //add a record and save changes
            db.TodoItems.Add(newToDo);
            db.SaveChanges();

            return Ok(newToDo);

        }

        //PUT - api/ToDoItems (HTTPPut)
                //ONE TODITEM
        public IHttpActionResult PutToDo(ToDoViewModel todo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            //We get the Todoitems from the db so we can modify it
            TodoItem existingToDo = db.TodoItems.Where(r => r.TodoId == todo.TodoId).FirstOrDefault();

            if (existingToDo != null)
            {
                existingToDo.TodoId = todo.TodoId;
                existingToDo.Action = todo.Action;
                existingToDo.CategoryID = todo.CategoryID;
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }

        //DELETE - api/TodoItems/id (HTTPDelete)
                //ONE TODOITEM
        public IHttpActionResult DeleteToDo(int id)
        {
            //Get TodoItem from the api to make sure theres a Item with this id
            TodoItem toDo = db.TodoItems.Where(t => t.TodoId == id).FirstOrDefault();

            if (toDo != null)
            {
                db.TodoItems.Remove(toDo);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }

        //We use Dispose() below to dispose of any connection to the database after we are done with them - best practice to handle performance - dispose of the instance of the controller and db connection when we are done with it.

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }//end class
}

