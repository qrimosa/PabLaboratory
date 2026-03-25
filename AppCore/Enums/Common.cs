namespace AppCore.Enums;

public enum ContactStatus { Active, Inactive, Blocked, Prospect, Lead }
public enum Gender { Male, Female, Other, NotSpecified }
public enum OrganizationType { NGO, PublicInstitution, GovernmentAgency, Association, Foundation, Other }
public enum AddressType { Main, Correspondence, Delivery, Billing }

public class Address
{
    // Adding = string.Empty; satisfies the compiler so 'new()' works
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public AddressType Type { get; set; }
}