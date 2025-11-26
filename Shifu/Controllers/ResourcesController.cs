using Microsoft.AspNetCore.Mvc;
using Shifu.Models;
using Shifu.Services;

namespace Shifu.Controllers
{
    public class ResourcesController : Controller
    {
        private readonly ResourceManager _resourceManager;

        public ResourcesController(ResourceManager resourceManager) => _resourceManager = resourceManager;


        public async Task<IActionResult> Index()
        {
            
            var resources = new List<Resource>
            {
                new Resource
                {
                    Title = "Mindfulness for Beginners",
                    Description = "A great introductory video on mindfulness meditation.",
                    Url = "https://www.youtube.com/watch?v=3VJI0ecVO6c",
                    ThumbnailUrl = "https://img.youtube.com/vi/3VJI0ecVO6c/maxresdefault.jpg"
                },
                new Resource
                {
                    Title = "Inspirational Addiction Recovery",
                    Description = "A story of Addiction and Recovery.",
                    Url = "https://www.youtube.com/watch?v=qu7_idmI_sE&list=PLaaJWwIpP_zaHmOwAmEYrEPmJ3nyuGqrc&index=1",
                    ThumbnailUrl = "https://img.youtube.com/vi/qu7_idmI_sE/hqdefault.jpg"
                },
                new Resource
                {
                    Title = "The Benefits of Journaling",
                    Description = "An article explaining the mental health benefits of keeping a journal.",
                    Url = "https://www.healthline.com/health/benefits-of-journaling",
                    ThumbnailUrl = "https://www.healthline.com/hlcmsresource/images/Healthline/benefits-of-journaling-1296x728-feature.jpg"
                }
                
            };

            return View(resources);
        }
    }
}
