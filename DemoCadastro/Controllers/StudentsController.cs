﻿
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoCadastro.Data;
using DemoCadastro.Models;
using Microsoft.AspNetCore.Authorization;

namespace DemoCadastro.Controllers;

[Authorize]
[Route("my-students")]
public class StudentsController : Controller
{
    private readonly ApplicationDbContext _context;

    public StudentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return _context.Student != null
            ? View(await _context.Student.ToListAsync()) :
            Problem("Entity set 'AplicationDbCpntext.Aluno 'is null");
    }
    
    [Route("details:/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        if (_context.Student == null)
        {
            return NotFound();
        }

        var student = await _context.Student
            .FirstOrDefaultAsync(m => m.Id == id);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }
    
    [Route("new")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("new")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nome,DataNascimento,Email, EmailConfirmacao,Avaliacao,Ativo")] Student student)
    {
        if (ModelState.IsValid)
        {
            student.DataNascimento = student.DataNascimento.ToUniversalTime();
            _context.Add(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(student);
    }

    [Route("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        if (_context.Student == null)
        {
            return NotFound();
        }

        var student = await _context.Student.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }
        return View(student);
    }

    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,DataNascimento,Email,Avaliacao,Ativo")] Student student)
    {
        if (id != student.Id)
        {
            return NotFound();
        }

        ModelState.Remove("EmailConfirmacao");

        if (ModelState.IsValid)
        {
            try
            {
                student.DataNascimento = student.DataNascimento.ToUniversalTime();
                _context.Update(student);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(student.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(student);
    }

    [Route("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (_context.Student == null)
        {
            return NotFound();
        }

        var student = await _context.Student
            .FirstOrDefaultAsync(m => m.Id == id);

        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    [HttpPost("delete/{id:int}"), ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Student == null)
        {
            return Problem("Entity set 'AplicationDbContext.Studet' is null");
        }

        var student = await _context.Student.FindAsync(id);
        if (student != null)
        {
            _context.Student.Remove(student);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool StudentExists(int id)
    {
        return _context.Student.Any(e => e.Id == id);
    }
}
