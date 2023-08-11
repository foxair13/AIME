using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.MVC.Data;
using NeftViewer.MVC.Models;

namespace NeftViewer.MVC.Controllers
{
    public class ActionRoleController : Controller
    {
        private readonly IActionRoleService _actionRoleService;
        private readonly IMapper _mapper;

        public ActionRoleController(IActionRoleService actionRoleService, IMapper mapper)
        {
            _actionRoleService = actionRoleService;
            _mapper = mapper;
        }

        // GET: ActionRole
        public async Task<IActionResult> Index()
        {
            var actionRoles = await _actionRoleService.GetActionRoles();
            var actionRoleViewModels = _mapper.Map<List<ActionRoleViewModel>>(actionRoles);
            return View(actionRoleViewModels);
        }

        // GET: ActionRole/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var actionRole = await _actionRoleService.FindActionRoleAsync(id);

            if (actionRole == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<ActionRoleViewModel>(actionRole);
            return View(result);
        }

        // GET: ActionRole/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ActionRole/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ActionId,RoleId")] ActionRoleViewModel actionRoleViewModel)
        {
            var actionRole = await _actionRoleService.FindActionRoleAsync(actionRoleViewModel.Id);

            if (actionRole == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<ActionRoleViewModel>(actionRole);
            return View(result);

            return View(actionRoleViewModel);
        }

        // GET: ActionRole/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var actionRole = await _actionRoleService.FindActionRoleAsync(id);

            if (actionRole == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<ActionRoleViewModel>(actionRole);
            return View(result);
        }

        // POST: ActionRole/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ActionId,RoleId")] ActionRoleViewModel actionRoleViewModel)
        {
            if (id != actionRoleViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var actionRole = _mapper.Map<ActionRole>(actionRoleViewModel);
                _actionRoleService.UpdateActionRole(actionRole);
                await _actionRoleService.CommitChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(actionRoleViewModel);
        }

        // GET: ActionRole/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var actionRole = await _actionRoleService.FindActionRoleAsync(id);

            if (actionRole == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<ActionRoleViewModel>(actionRole);
            return View(result);
        }

        // POST: ActionRole/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actionRole =  _actionRoleService.DeleteActionRole(id.ToString());
            await _actionRoleService.CommitChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ActionRoleViewModelExists(int id)
        {
            return _actionRoleService.FindActionRoleAsync(id) != null;
        }
    }
}
