namespace ItemStorage.Data.Entities;

public class RequestResponseLog
{
    public int Id { get; set; }
    public DateTime RequestDate { get; set; }
    public long Duration { get; set; }
    public string Request { get; set; }
    public string Response { get; set; }
    public int ResponseCode { get; set; }
}