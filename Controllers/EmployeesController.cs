﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeApp.Data;
using EmployeeApp.Models;
using EmployeeApp.DTO;

namespace EmployeeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public EmployeesController(EmployeeDbContext context)
        {
            _context = context;
        }

        // GET: api/Employees
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesSearchEmployees([FromQuery] string name)
        {
            try {

                if (String.IsNullOrWhiteSpace(name)) {
                    return BadRequest("search term cannot be empty");
                }
                var employees=await _context.Employees.Where(e => e.Name.Contains(name)).ToListAsync();
                if (employees.Any()) { return Ok(employees); }
                else { return NotFound("No employees found with the search term"); }
            }
            catch (Exception ex) {
              return  StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database.");

            }
           
        }
        
        [HttpGet("summary")]
        public async Task<IActionResult> GetEmployeesSummary()
        {
           var summaries = await _context.Employees
                .Where(e=>e.Isactive)
                .GroupBy(e=>e.Department)
                .Select(g=>new DepartmentSummaryDto 
               { 
               Department=g.Key,
               EmployeeCount = g.Count(),
               AverageAge = g.Average(e => e.Age)
               })
               .ToListAsync();
            return Ok(summaries);
        }
        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
