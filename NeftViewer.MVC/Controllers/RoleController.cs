using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NeftViewer.MVC.Data;
using NeftViewer.MVC.Models;

namespace NeftViewer.MVC.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET: Role
        public IActionResult Index()
        {
            List<RoleViewModel> roles = _roleManager.Roles
                .Select(r => new RoleViewModel { Id = r.Id, RoleName = r.Name })
                .ToList();

            return View(roles);
        }

        // GET: Role/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return NotFound();
            }

            var roleViewModel = new RoleViewModel { Id = role.Id, RoleName = role.Name };
            return View(roleViewModel);
        }

        // GET: Role/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole(roleViewModel.RoleName);
                await _roleManager.CreateAsync(role);

                return RedirectToAction(nameof(Index));
            }
            return View(roleViewModel);
        }

        // GET: Role/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return NotFound();
            }

            var roleViewModel = new RoleViewModel { Id = role.Id, RoleName = role.Name };
            return View(roleViewModel);
        }

        // POST: Role/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id, RoleName")] RoleViewModel roleViewModel)
        {
            if (id != roleViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(id.ToString());
                if (role == null)
                {
                    return NotFound();
                }

                role.Name = roleViewModel.RoleName;
                await _roleManager.UpdateAsync(role);

                return RedirectToAction(nameof(Index));
            }

            return View(roleViewModel);
        }

        // GET: Role/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return NotFound();
            }

            var roleViewModel = new RoleViewModel { Id = role.Id, RoleName = role.Name };
            return View(roleViewModel);
        }

        // POST: Role/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return NotFound();
            }

            await _roleManager.DeleteAsync(role);

            return RedirectToAction(nameof(Index));
        }
    }
}
