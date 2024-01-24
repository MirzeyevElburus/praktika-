using Landab202.DAL;
using Landab202.Models;
using Landab202.Utilities.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Landab202.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServiceController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ServiceController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {

            List<Services> s = _context.Servicess.ToList();
            return View(s);
        }
        public IActionResult Create()
        {

           
            return View();
        }
        [HttpPost]  
        public async Task<IActionResult>Create(Services s)
        {
            if(!ModelState.IsValid) return View(s);
            if(s.Photo is not null)
            {
                if (!s.Photo.ValidateType("image/"))
                {
                    ModelState.AddModelError("Photo", "File tipi uyqun deyil");
                    return View(s);
                }
                if (!s.Photo.ValidateSize(2*1024))
                {
                    ModelState.AddModelError("Photo", "File tipi uyqun deyil");
                    return View(s);
                }
            }
            string filename = await s.Photo.CreateAsync(_env.WebRootPath, "assets", "img", "services");
            Services services = new Services
            {
                Image=filename,
                Name=s.Name,
                Description=s.Description,  
            };
            await _context.Servicess.AddAsync(services);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult>Update(int id)
        {
            if (id <= 0) return BadRequest();
            Services exist=await _context.Servicess.FirstOrDefaultAsync(s => s.Id == id);   
            if(exist == null) return NotFound();    
            return View(exist);
        }
        [HttpPost]
        public  async Task<IActionResult>Update(int id,Services s)
        {
            if(!ModelState.IsValid) return View(s);
            Services exist = await _context.Servicess.FirstOrDefaultAsync(s => s.Id == id);
            if (exist == null) return NotFound();
            if (s.Photo is not null)
            {
                if (!s.Photo.ValidateType("image/"))
                {
                    ModelState.AddModelError("Photo", "File tipi uyqun deyil");
                    return View(s);
                }
                if (!s.Photo.ValidateSize(2 * 1024))
                {
                    ModelState.AddModelError("Photo", "File tipi uyqun deyil");
                    return View(s);
                }
                string filename = await s.Photo.CreateAsync(_env.WebRootPath, "assets", "img", "services");
                exist.Image.DeleteFile(_env.WebRootPath, "assets", "img", "services");
                exist.Image = filename;
            }
            exist.Name = s.Name;
            exist.Description = s.Description;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult>Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Services exist = await _context.Servicess.FirstOrDefaultAsync(s => s.Id == id);
            if (exist == null) return NotFound();
            exist.Image.DeleteFile(_env.WebRootPath, "assets", "img", "services");
            _context.Remove(exist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
