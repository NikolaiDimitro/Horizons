using Horizons.Data;
using Horizons.Data.Entities;
using Horizons.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Horizons.Controllers
{
    [Authorize]
    public class DestinationController : Controller
    {
        private readonly ApplicationDbContext _data;
        private readonly UserManager<IdentityUser> _userManager;

        public DestinationController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _data = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var model = _data.Destinations
                .Where(d => !d.IsDeleted)
                .Select(d => new IndexDestinationViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    Terrain = d.Terrain.Name,
                    ImageUrl = d.ImageUrl,
                    FavoritesCount = d.UsersDestinations.Count,
                    IsFavorite = d.UsersDestinations.Any(ud => ud.UserId == _userManager.GetUserId(User)),
                    IsPublisher = d.PublisherId == _userManager.GetUserId(User)
                });

            return View(new AllDestinationsViewModel { Destinations = model.ToList() });
        }

        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var model = _data.Destinations
                .Where(d => d.Id == id)
                .Select(d => new DetailsViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    ImageUrl = d.ImageUrl,
                    Terrain = d.Terrain.Name,
                    Publisher = d.Publisher.UserName,
                    PublishedOn = d.PublishedOn,
                    IsFavorite = d.UsersDestinations.Any(ud => ud.UserId == _userManager.GetUserId(User)),
                    IsPublisher = d.PublisherId == _userManager.GetUserId(User)
                })
                .FirstOrDefault();

            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var model = new AddDestinationViewModel
            {
                Terrains = _data.Terrains
                    .Select(t => new TerrainViewModel
                    {
                        Id = t.Id,
                        Name = t.Name
                    })
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Add(AddDestinationViewModel model)
        {
            string dateTimeString = $"{model.PublishedOn}";

            if (!DateTime.TryParseExact(dateTimeString, "dd-MM-yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime parseDateTime))
            {
                throw new InvalidOperationException("Invalid date format.");
            }

            var userId = _userManager.GetUserId(User);

            if (userId == null)
                return BadRequest();

            var destination = new Destination
            {
                Name = model.Name,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                TerrainId = model.TerrainId,
                PublishedOn = parseDateTime,
                PublisherId = _userManager.GetUserId(User)
            };

            _data.Destinations.Add(destination);
            _data.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var model = _data.Destinations
                .Where(d => d.Id == id)
                .Select(d => new EditDestinationViewModel
                {
                        Name = d.Name,
                        Description = d.Description,
                        ImageUrl = d.ImageUrl,
                        PublishedOn = d.PublishedOn.ToString("dd-MM-yyyy"),
                        PublisherId = d.PublisherId,
                        TerrainId = d.TerrainId,
                        Terrains = _data.Terrains
                            .Select(t => new TerrainViewModel
                            {
                                Id = t.Id,
                                Name = t.Name
                            }).ToList()
                })
                .FirstOrDefault();

            if (model == null)
                return BadRequest();
            
            var userId = _userManager.GetUserId(User);

            if (userId != model.PublisherId)
                return Unauthorized();

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EditDestinationViewModel model)
        {
            var destination = _data.Destinations.FirstOrDefault(d => d.Id == model.Id);

            if (destination == null)
                return BadRequest();

            var userId = _userManager.GetUserId(User);

            if (userId != model.PublisherId)
                return Unauthorized();

            destination.Name = model.Name;
            destination.Description = model.Description;
            destination.ImageUrl = model.ImageUrl;
            destination.TerrainId = model.TerrainId;
            destination.PublishedOn = DateTime.ParseExact(model.PublishedOn, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            _data.SaveChanges();

            return RedirectToAction(nameof(Details), new { id = destination.Id});
        }

        public IActionResult Favorites()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Unauthorized();

            var model = _data.UsersDestinations
                .Where(ud => ud.UserId == userId)
                .Select(ud => new IndexDestinationViewModel
                {
                    Id = ud.Destination.Id,
                    Name = ud.Destination.Name,
                    Terrain = ud.Destination.Terrain.Name,
                    ImageUrl = ud.Destination.ImageUrl,
                    FavoritesCount = ud.Destination.UsersDestinations.Count,
                    IsFavorite = true,
                    IsPublisher = ud.Destination.PublisherId == userId
                });

            return View(new AllDestinationsViewModel { Destinations = model.ToList() });
        }

        [HttpPost]
        public IActionResult AddToFavorites(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Unauthorized();

            var destination = _data.Destinations.FirstOrDefault(d => d.Id == id);
            if (destination == null)
                return BadRequest();

            if (destination.PublisherId == userId)
                return BadRequest();

            var userDestination = new UserDestination
            {
                UserId = userId,
                DestinationId = id
            };

            _data.UsersDestinations.Add(userDestination);
            _data.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult RemoveFromFavorites(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Unauthorized();
            var userDestination = _data.UsersDestinations
                .FirstOrDefault(ud => ud.UserId == userId && ud.DestinationId == id);
            if (userDestination == null)
                return BadRequest();
            _data.UsersDestinations.Remove(userDestination);
            _data.SaveChanges();
            return RedirectToAction(nameof(Favorites));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var model = _data.Destinations
                .Where(d => d.Id == id)
                .Select(d => new DeleteConfirmViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    PublisherId = d.PublisherId,
                    Publisher = d.Publisher.UserName!
                })
                .FirstOrDefault();

            if (model == null)
                return BadRequest();

            var userId = _userManager.GetUserId(User);

            if (userId != model.PublisherId)
                return Unauthorized();

            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id, DeleteConfirmViewModel model)
        {
            var destination = _data.Destinations.FirstOrDefault(d => d.Id == id);
            if (destination == null)
                return BadRequest();

            var userId = _userManager.GetUserId(User);
            if (userId != model.PublisherId)
                return Unauthorized();

            destination.IsDeleted = true;

            _data.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
