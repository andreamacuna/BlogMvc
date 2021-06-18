using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogMVC.Data;
using BlogMVC.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BlogMVC.Controllers
{
    public class PostsController : Controller
    {
        private readonly BlogDbContext _context;

        public IWebHostEnvironment Environment { get; }

        public PostsController(BlogDbContext context, IWebHostEnvironment _environment)
        {
            _context = context;
            Environment = _environment;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Posts.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> Index(String parametroBusqueda)
        {
            if (string.IsNullOrWhiteSpace(parametroBusqueda))
            {
                return View(await _context.Posts.ToListAsync());
            }
            var resultadoBusqueda = _context.Posts.Where(p => p.Titulo.ToUpper().Contains(parametroBusqueda.ToUpper()));
            return View(await resultadoBusqueda.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            var categorias = _context.Categorias.ToList();
            var selectListCategoria = new List<SelectListItem>();
            foreach (var categoria in categorias)
            {
                var itemCategoria = new SelectListItem() { Value = categoria.Id.ToString(), Text = categoria.Nombre };
                selectListCategoria.Add(itemCategoria);

            }
            ViewBag.CategoriaId = selectListCategoria;
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Contenido,Imagen,CategoriaId,FechaDeCreacion")] Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Add(post);
                await _context.SaveChangesAsync();

                string wwwPath = this.Environment.WebRootPath;
                string contentPath = this.Environment.ContentRootPath;

                string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }


                foreach (var postedFile in Request.Form.Files)
                {
                    string fileName = Path.GetFileName(postedFile.FileName);
                    var nombreImagen = String.Format("PostId-{0}{1}", post.Id, Path.GetExtension(postedFile.FileName));
                    using (FileStream stream = new FileStream(Path.Combine(path, nombreImagen), FileMode.Create))
                    {
                        postedFile.CopyTo(stream);
                        post.Imagen = nombreImagen;
                    }
                }

                _context.Entry(post).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var categorias = _context.Categorias.ToList();
            var selectListCategoria = new List<SelectListItem>();
            foreach (var categoria in categorias)
            {
                var itemCategoria = new SelectListItem() { Value = categoria.Id.ToString(), Text = categoria.Nombre };
                selectListCategoria.Add(itemCategoria);

            }
            ViewBag.CategoriaId = selectListCategoria;


            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Contenido,Imagen,CategoriaId,FechaDeCreacion")] Post post)
        {

            var categorias = _context.Categorias.ToList();
            var selectListCategoria = new List<SelectListItem>();
            foreach (var categoria in categorias)
            {
                var itemCategoria = new SelectListItem() { Value = categoria.Id.ToString(), Text = categoria.Nombre };
                selectListCategoria.Add(itemCategoria);

            }
            ViewBag.CategoriaId = selectListCategoria;


            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
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
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            var categorias = _context.Categorias.ToList();
            var selectListCategoria = new List<SelectListItem>();
            foreach (var categoria in categorias)
            {
                var itemCategoria = new SelectListItem() { Value = categoria.Id.ToString(), Text = categoria.Nombre };
                selectListCategoria.Add(itemCategoria);

            }
            ViewBag.CategoriaId = selectListCategoria;


            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categorias = _context.Categorias.ToList();
            var selectListCategoria = new List<SelectListItem>();
            foreach (var categoria in categorias)
            {
                var itemCategoria = new SelectListItem() { Value = categoria.Id.ToString(), Text = categoria.Nombre };
                selectListCategoria.Add(itemCategoria);

            }
            ViewBag.CategoriaId = selectListCategoria;


            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
