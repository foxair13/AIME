using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Identity;
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
        private readonly IActionService _actionService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public ActionRoleController(
            IActionRoleService actionRoleService,
            IActionService actionService,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper)
        {
            _actionRoleService = actionRoleService;
            _actionService = actionService;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        // GET: ActionRole
        public async Task<IActionResult> Index()
        {
            var actionRoles = await _actionRoleService.GetActionRoles();

            var actionRoleViewModels = new List<ActionRoleViewModel>();

            foreach (var actionRole in actionRoles)
            {
                var actionName = await _actionService.FindActionAsync(actionRole.ActionId);
                var roleName = await _roleManager.FindByIdAsync(actionRole.RoleId);

                var viewModel = new ActionRoleViewModel
                {
                    Id = actionRole.Id,
                    ActionName = actionName.Name,
                    RoleName = roleName.Name 
                };

                actionRoleViewModels.Add(viewModel);
            }

            return View(actionRoleViewModels);
        }


        // GET: ActionRole/Create
        public async Task<IActionResult> Create()
        {
            PopulateDropdownListsAsync();
            return View();
        }

        // POST: ActionRole/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ActionId,RoleId")] ActionRoleViewModel actionRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                var actionRole = _mapper.Map<ActionRole>(actionRoleViewModel);
                await _actionRoleService.AddActionRole(actionRole);
                await _actionRoleService.CommitChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateDropdownListsAsync();
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
            PopulateDropdownListsAsync();
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

            PopulateDropdownListsAsync();
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
            var viewModel = new ActionRoleViewModel
            {
                Id = actionRole.Id,
                ActionName = actionRole.Action.Name,
                RoleName = actionRole.AspNetRoles.Name
            };
            var result = viewModel;
            return View(result);
        }

        // POST: ActionRole/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actionRole =  _actionRoleService.DeleteActionRole(id);
            await _actionRoleService.CommitChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ActionRoleViewModelExists(int id)
        {
            return _actionRoleService.FindActionRoleAsync(id) != null;
        }
        private async Task PopulateDropdownListsAsync()
        {
         
            var actionsTask = _actionService.GetActions();
            var roles = _roleManager.Roles;

            var actions = await actionsTask;

            ViewBag.ActionId = new SelectList(actions, "Id", "Name");
            ViewBag.RoleId = new SelectList(roles, "Id", "Name");
        }
    }
}
