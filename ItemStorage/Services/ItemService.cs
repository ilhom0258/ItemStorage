using ItemStorage.Data;
using ItemStorage.Data.Entities;
using ItemStorage.Extensions;
using ItemStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace ItemStorage.Services;


public interface IItemService
{
    Task AddItemsAsync(IEnumerable<AddItemModel> items);
    Task<IEnumerable<GetItemModel>> GetItemsAsync(IEnumerable<DataFilter>? filters);
}
public class ItemService(DataContext dataContext) : IItemService
{
    public async Task AddItemsAsync(IEnumerable<AddItemModel> items)
    {
        dataContext.Items.RemoveRange(dataContext.Items);
        await  dataContext.SaveChangesAsync();

        await dataContext.Items.AddRangeAsync(items.Select(i => new Item
        {
            Code = i.Code,
            Value = i.Value
        }).OrderBy(i => i.Code));
        
        await dataContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<GetItemModel>> GetItemsAsync(IEnumerable<DataFilter>? filters)
    {
        filters ??= new List<DataFilter>();

        return await dataContext.Items.ApplyFilter(filters).Select(i => new GetItemModel
        {
            Id = i.Id,
            Code = i.Code,
            Value = i.Value
        }).ToListAsync();
    }
}