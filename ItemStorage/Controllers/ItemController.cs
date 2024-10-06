using ItemStorage.Data;
using ItemStorage.Models;
using ItemStorage.Services;
using Microsoft.AspNetCore.Mvc;

namespace ItemStorage.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ItemController(IItemService itemService) : ControllerBase
{
    [HttpPost]
    public async Task<IEnumerable<GetItemModel>> GetItems([FromBody] IEnumerable<DataFilter>? filters)
    {
        return await itemService.GetItemsAsync(filters);
    }

    [HttpPost]
    public async Task<IActionResult> AddItems([FromBody] KeyValuePair<int, string>[] items)
    {
        var addModels = items.Select(i => new AddItemModel
        {
            Code = i.Key,
            Value = i.Value
        }).ToList();

        await itemService.AddItemsAsync(addModels);

        return Ok();
    }
       
}