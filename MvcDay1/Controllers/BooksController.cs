using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcDay1.Applications.Books.Commands.CreateBookCommand;
using MvcDay1.Applications.Books.Commands.EditBookCommand;
using MvcDay1.Applications.Books.Queries.GetAllCategoryQuery;
using MvcDay1.Applications.Books.Queries.GetBookDetailQuery;
using MvcDay1.Applications.Books.Queries.GetBookEditQuery;
using MvcDay1.Applications.Books.Queries.GetBookQuery;
using MvcDay1.Data;

namespace MvcDay1.Controllers
{
    [Authorize(Roles = "Moderator")]
    public class BooksController : Controller
    {
        private readonly IMediator _mediator;

        public BooksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(GetBookQuery query)
        {
            var model = await _mediator.Send(query);
            return View(model);
        }


        public async Task<IActionResult> Detail(GetBookDetailQuery query)
        {
            if (!ModelState.IsValid)
            {
                return View("Error");
            }

            var model = await _mediator.Send(query);
            return View(model);
        }


        public async Task<IActionResult> Edit([FromQuery] GetBookEditQuery query)
        {
            if (!ModelState.IsValid)
            {
                return View("Error");
            }

            ViewData["Categories"] = await _mediator.Send(new GetAllCategoryQuery());
            ViewData["Info"] = await _mediator.Send(query);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromQuery] GetBookEditQuery query, EditBookCommand command)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Categories"] = await _mediator.Send(new GetAllCategoryQuery());
                ViewData["Info"] = await _mediator.Send(query);
                return View(command);
            }

            await _mediator.Send(command);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Categories"] = await _mediator.Send(new GetAllCategoryQuery());
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookCommand command)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Categories"] = await _mediator.Send(new GetAllCategoryQuery());
                return View(command);
            }
            await _mediator.Send(command);
            return RedirectToAction("Index");
        }

    }
}
