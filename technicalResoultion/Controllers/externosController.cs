﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using technicalResoultion.Models;

namespace technicalResoultion.Controllers
{
    public class externosController : Controller
    {
        private readonly TechResContext _context;

        public externosController(TechResContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var externos = _context.externos.ToList();
            ViewData["externos"] = externos;
            return View();
        }

        public IActionResult Details(int id)
        {
            var externo = _context.externos.Find(id);
            if (externo == null)
            {
                return NotFound();
            }
            ViewData["externo"] = externo; // Asignar el objeto "externo" al ViewData
            return View();
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(externos externo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(externo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(externo);
        }

        public IActionResult Edit(int id)
        {
            var externo = _context.externos.Find(id);
            if (externo == null)
            {
                return NotFound();
            }
            return View(externo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, externos externo)
        {
            if (id != externo.id_externo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(externo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!externosExists(externo.id_externo))
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
            return View(externo);
        }

        private bool externosExists(int id)
        {
            return _context.externos.Any(e => e.id_externo == id);
        }
    }
}

