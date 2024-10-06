using Microsoft.EntityFrameworkCore;

namespace ItemStorage.Data;


public interface IDataContextProvider
{
    DataContext GetContext();
}

public class DataContextProvider: IDataContextProvider
{
    private readonly DbContextOptions<DataContext> _options;
    public DataContextProvider(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        _options = optionsBuilder
            .UseSqlite(connectionString)
            .Options;
    }

    public DataContext GetContext()
    {
        return new DataContext(_options);
    }
}