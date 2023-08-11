using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.MVC.Data;
using NeftViewer.MVC.Models;

namespace NeftViewer.MVC.Controllers
{
    public class ActionController : Controller
    {
        private readonly IActionService _actionService;
        private readonly IMapper _mapper;

        public ActionController(IActionService actionService, IMapper mapper)
        {
            _actionService = actionService;
            _mapper= mapper;
        }

        // GET: Action
        public async Task<IActionResult> Index()
        {
            var actions = await _actionService.GetActions();
            var actionViewModels = _mapper.Map<List<ActionViewModel>>(actions);
            return View(actionViewModels);
        }

        // GET: Action/Details/5
        public async Task<IActionResult> Details(int id)
        {
          
            var action = await _actionService.FindActionAsync(id);

            if (action == null)
            {
                return NotFound();
            }
            var result = _mapper.Map<ActionViewModel>(action);
           

            return View(result);
        }

        // GET: Action/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Action/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] ActionViewModel actionViewModel)
        {
            if (ModelState.IsValid)
            {
                var success = await _actionService.AddAction(new NeftViewer.Data.Models.Action
                {
                    Id = actionViewModel.Id,
                    Name = actionViewModel.Name,
                    Description = actionViewModel.Description
                });

                if (success)
                {
                    await _actionService.CommitChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(actionViewModel);
        }


        // GET: Action/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
        

            var action = await _actionService.FindActionAsync(id);

            if (action == null)
            {
                return NotFound();
            }
            var result = _mapper.Map<ActionViewModel>(action);

            return View(result);
        }

        // POST: Action/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description")] ActionViewModel actionViewModel)
        {
            if (id != actionViewModel.Id.ToString())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var result = _mapper.Map<NeftViewer.Data.Models.Action>(actionViewModel);
                    _actionService.UpdateAction(result);
                    await _actionService.CommitChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActionExists(actionViewModel.Id))
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

            return View(actionViewModel);
        }

        // GET: Action/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
          

            var action = await _actionService.FindActionAsync(id);

            if (action == null)
            {
                return NotFound();
            }
            var result = _mapper.Map<ActionViewModel>(action);
            return View(result);
        }

        // POST: Action/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var action =  _actionService.DeleteAction(id);
            await _actionService.CommitChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ActionExists(int id)
        {
            return _actionService.FindActionAsync(id) != null;
        }
    }
}
