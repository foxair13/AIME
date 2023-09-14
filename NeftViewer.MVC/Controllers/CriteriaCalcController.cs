using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NeftViewer.BL.Services;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.MVC.Data;
using NeftViewer.MVC.Models;

namespace NeftViewer.MVC.Controllers
{
    public class CriteriaCalcController : Controller
    {
        private readonly ICriteriaCalcMethodService _criteriaCalcMethodService;
        private readonly ICriteriaService _criteriaService;
        private readonly IMapper _mapper;

        public CriteriaCalcController(ICriteriaCalcMethodService criteriaCalcMethodService, ICriteriaService criteriaService, IMapper mapper)
        {
            _criteriaCalcMethodService = criteriaCalcMethodService;
            _criteriaService = criteriaService;
            _mapper = mapper;
        }

        // GET: CriteriaCalc
        public async Task<IActionResult> Index()
        {
            var criteriaCalcMethods = await _criteriaCalcMethodService.GetCriteriaCalcMethods();

            var criteriaCalcMethodViewModels = new List<CriteriaCalcMethodViewModel>();

            foreach (var actioncriteriaCalcMethodRole in criteriaCalcMethods)
            {

                var criteria = await _criteriaService.FindCriteriaAsync(actioncriteriaCalcMethodRole.CriteriaId);

                var viewModel = new CriteriaCalcMethodViewModel
                {
                    Id = actioncriteriaCalcMethodRole.Id,
                    Name = criteria.Name,
                    CriteriaId = actioncriteriaCalcMethodRole.CriteriaId,
                    СalculationByMax = actioncriteriaCalcMethodRole.СalculationByMax,
                    IsHidden=actioncriteriaCalcMethodRole.IsHidden
                };

                criteriaCalcMethodViewModels.Add(viewModel);
            }
            criteriaCalcMethodViewModels= criteriaCalcMethodViewModels.OrderBy(x => x.Name).ToList();
            return View(criteriaCalcMethodViewModels);
        }


        // GET: CriteriaCalc/Create
        public async Task<IActionResult> Create()
        {
          await PopulateDropdownListsAsync();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CriteriaId,СalculationByMax,IsHidden")] CriteriaCalcMethodViewModel criteriaCalcMethodViewModel)
        {
            if (ModelState.IsValid)
            {
                var criteriaCalcMethod = _mapper.Map<CriteriaCalcMethod>(criteriaCalcMethodViewModel);
                await _criteriaCalcMethodService.AddCriteriaCalcMethod(criteriaCalcMethod);
               
                return RedirectToAction(nameof(Index));
            }

           await PopulateDropdownListsAsync();
            return View(criteriaCalcMethodViewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var criteriaCalc = await _criteriaCalcMethodService.FindCriteriaCalcMethodAsync(id);

            if (criteriaCalc == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<CriteriaCalcMethodViewModel>(criteriaCalc);
            await PopulateDropdownListsUpdateAsync();
            return View(result);
        }

        // POST: ActionRole/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CriteriaId,СalculationByMax,IsHidden")] CriteriaCalcMethodViewModel criteriaCalcMethodViewModel)
        {
            if (id != criteriaCalcMethodViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var criteriaCalc = _mapper.Map<CriteriaCalcMethod>(criteriaCalcMethodViewModel);
                _criteriaCalcMethodService.UpdateCriteriaCalcMethod(criteriaCalc);
                await _criteriaCalcMethodService.CommitChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdownListsUpdateAsync();
            return View(criteriaCalcMethodViewModel);
        }

        // GET: ActionRole/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var criteriaCalc = await _criteriaCalcMethodService.FindCriteriaCalcMethodAsync(id);

            if (criteriaCalc == null)
            {
                return NotFound();
            }
            var criteria = await _criteriaService.FindCriteriaAsync(criteriaCalc.CriteriaId);
            var viewModel = new CriteriaCalcMethodViewModel
            {
                Id = criteriaCalc.Id,
                Name = criteria.Name,
                СalculationByMax = criteriaCalc.СalculationByMax,
                IsHidden = criteriaCalc.IsHidden
            };
            var result = viewModel;
            return View(result);
        }

        // POST: ActionRole/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actionRole = _criteriaCalcMethodService.DeleteCriteriaCalcMethod(id);
            await _criteriaCalcMethodService.CommitChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ActionRoleViewModelExists(int id)
        {
            return _criteriaCalcMethodService.FindCriteriaCalcMethodAsync(id) != null;
        }
        private async Task PopulateDropdownListsAsync()
        {

            var criteria = await _criteriaService.GetCriterias();
            var criteriacalc = await _criteriaCalcMethodService.GetCriteriaCalcMethods();
            var filteredcriteria = from x in criteria
                                   join y in criteriacalc on x.Id equals y.CriteriaId into joined
                                   from suby in joined.DefaultIfEmpty()
                                   where suby == null
                                   select x;

            filteredcriteria = filteredcriteria.OrderBy(x => x.Name);
            ViewBag.CriteriaId = new SelectList(filteredcriteria, "Id", "Name");
        }
        private async Task PopulateDropdownListsUpdateAsync()
        {

            var criteria = await _criteriaService.GetCriterias();



            criteria = criteria.OrderBy(x => x.Name);
            ViewBag.CriteriaId = new SelectList(criteria, "Id", "Name");
        }
    }
}
