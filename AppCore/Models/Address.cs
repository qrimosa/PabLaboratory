using AppCore.ValueObjects;

namespace AppCore.Models;

public class Address
{
    public int id { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public Country Country { get; set; }
}