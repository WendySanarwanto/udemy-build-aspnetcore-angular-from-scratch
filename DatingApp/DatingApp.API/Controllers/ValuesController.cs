using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ValuesController : ControllerBase
  {
    private readonly DataContext _context;
    public ValuesController(DataContext context)
    {
      this._context = context;

    }
    // GET api/values
    [HttpGet]
    public async Task<IActionResult> Get()
    {
      var values = await this._context.Values.ToListAsync();
      return Ok(values);
    }
    // public IActionResult Get()
    // {
    //   var values = this._context.Values.ToList();
    //   return Ok(values);
    // }

    // GET api/values/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Value>> Get(int id)
    {
    //   Value found = this._context.Values.FirstOrDefault<Value>(_value => _value.Id == id);
      Value found = await this._context.FindAsync<Value>(id);
      if (found == null)  {
          NotFound();
      }

      return found;
    }

    // POST api/values
    [HttpPost]
    public ActionResult Post(Value newValue)
    {
        var createdValue = this._context.Add(newValue).Entity;
        this._context.SaveChanges();
        return CreatedAtAction(nameof(Get), createdValue);
    }

    // PUT api/values/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
  }
}
